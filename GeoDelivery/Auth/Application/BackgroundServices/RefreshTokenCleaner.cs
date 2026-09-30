using Auth.Application.Features;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auth.Application.BackgroundServices;

public class RefreshTokenCleaner(
    IServiceScopeFactory scopeFactory,
    ILogger<RefreshTokenCleaner> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

        try
        {
            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    logger.LogInformation("Starting refresh token cleaner");
                    
                    using var scope = scopeFactory.CreateScope();
                    
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    
                    await mediator.Send(new TokenProvider.CleanOldTokensCommand(), stoppingToken);
                    
                    logger.LogInformation("Token cleanup completed");
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during token cleanup command execution");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Background token cleaning was canceled");
        }
    }
}