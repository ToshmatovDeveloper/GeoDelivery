using Auth.Application.CustomExceptions;
using Auth.Application.Settings;
using Auth.Domain;
using Auth.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.Application.Features;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

public class RefreshTokenCommandHandler(
    AuthDbContext dbContext,
    TokenProvider provider,
    IOptionsMonitor<JwtSettings> options,
    UserManager<User> userManager,
    ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse> 
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to refresh access token using provided refresh token");

        var oldRefreshToken = await dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == command.RefreshToken, cancellationToken);

        if (oldRefreshToken is null)
        {
            logger.LogWarning("Refresh token attempt failed: token not found in the database");
            throw new UnauthorizedException("Invalid or expired refresh token."); 
        }

        if (oldRefreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            logger.LogWarning("Refresh token attempt failed: token for user {UserId} has expired at {ExpiresOnUtc}", 
                oldRefreshToken.UserId, oldRefreshToken.ExpiresOnUtc);
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        logger.LogInformation("Fetching roles for user {UserId} during token refresh", oldRefreshToken.UserId);
        var roles = await userManager.GetRolesAsync(oldRefreshToken.User);
        
        logger.LogInformation("Generating new access token for user {UserId}", oldRefreshToken.UserId);
        string accessToken = provider.GenerateAccessToken(oldRefreshToken.User, roles);
    
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = oldRefreshToken.UserId,
            Token = provider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(options.CurrentValue.RefreshTokenExpirationInDays)
        };
    
        dbContext.RefreshTokens.Remove(oldRefreshToken);
        await dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken); 
    
        await dbContext.SaveChangesAsync(cancellationToken);
    
        logger.LogInformation("Successfully rotated refresh token for user {UserId}", oldRefreshToken.UserId);

        return new RefreshTokenResponse(accessToken, newRefreshToken.Token);
    }
}