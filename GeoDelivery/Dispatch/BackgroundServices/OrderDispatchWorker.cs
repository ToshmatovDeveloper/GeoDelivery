using Dispatch.Application;
using MediatR;
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
      
      // TODO: Fetch unassigned orders from the Order module or database.
      // For demonstration, simulating a pending order payload:
      
      var pendingOrders = new List<(
         Guid OrderId, Guid RestaurantId, double Latitude, double Longitude)> { };

      foreach (var order in pendingOrders)
      {
         if (cancellationToken.IsCancellationRequested) break;
         
         logger.LogInformation("Processing dispatch for Order ID: {OrderId}", order.OrderId);
         
         var courierId = await mediator.Send(
            new FindOptimalCourierQuery(
               order.RestaurantId, order.Latitude, order.Longitude),
            cancellationToken);

         if (courierId.HasValue)
         {
            var success = await mediator.Send(
               new AssignCourierCommand(order.OrderId, order.RestaurantId, courierId.Value),
               cancellationToken);

            if (success)
            {
               logger.LogInformation("Courier {CourierId} assigned to Order {OrderId}", courierId.Value, order.OrderId);
            }
            else
            {
               logger.LogWarning("Failed to assign courier to Order {OrderId}", order.OrderId);
            }
         }
         else
         {
            logger.LogWarning("No available courier found for Order {OrderId}", order.OrderId);
         }
      }
   }
}

