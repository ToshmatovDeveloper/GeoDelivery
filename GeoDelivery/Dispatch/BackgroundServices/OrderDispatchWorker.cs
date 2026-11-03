using Dispatch.Application;
using Dispatch.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dispatch.BackgroundServices;

public class OrderDispatchWorker(
   IServiceScopeFactory scopeFactory,
   ILogger<OrderDispatchWorker> logger) : BackgroundService
{
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      logger.LogInformation("OrderDispatchWorker started");

      while (!stoppingToken.IsCancellationRequested)
      {
         try
         {
            await ProcessPendingOrdersAsync(stoppingToken);
         }
         catch (Exception ex)
         {
            logger.LogError(ex, "An error occurred while processing pending orders for dispatch");
         }

         await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
      }

      logger.LogInformation("OrderDispatchWorker stopped");
   }
   
   private async Task ProcessPendingOrdersAsync(CancellationToken cancellationToken)
   {
      using var scope = scopeFactory.CreateAsyncScope();
   
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var dbContext = scope.ServiceProvider.GetRequiredService<DispatchDbContext>();

      var pendingTasks = await dbContext.DispatchTasks
         .Where(t => !t.IsProcessed)
         .OrderBy(t => t.CreatedAt)
         .Take(10)
         .ToListAsync(cancellationToken);

      if (!pendingTasks.Any())
         return;

      foreach (var task in pendingTasks)
      {
         if (cancellationToken.IsCancellationRequested) break;
      
         logger.LogInformation("Processing dispatch for Order ID: {OrderId}", task.OrderId);
      
         var courierId = await mediator.Send(
            new FindOptimalCourierQuery(task.RestaurantId, task.Latitude, task.Longitude),
            cancellationToken);

         if (courierId.HasValue)
         {
            var success = await mediator.Send(
               new AssignCourierCommand(task.OrderId, task.RestaurantId, courierId.Value),
               cancellationToken);

            if (success)
            {
               task.IsProcessed = true;
               await dbContext.SaveChangesAsync(cancellationToken);

               logger.LogInformation("Courier {CourierId} assigned to Order {OrderId}", courierId.Value, task.OrderId);
            }
            else
            {
               logger.LogWarning("Failed to assign courier to Order {OrderId}", task.OrderId);
            }
         }
         else
         {
            logger.LogWarning("No available courier found for Order {OrderId}. Will retry later.", task.OrderId);
         }
      }
   }
}

