using Auth.Application.CustomExceptions;
using Auth.Application.Settings;
using Auth.Domain;
using Auth.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.Application.Features;

public record UserLoginCommand(string Email, string Password) : IRequest<UserLoginResponse>;
public record UserLoginResponse(string AccessToken, string RefreshToken, Guid UserId, string Email);

public class UserLoginCommandHandler(
    AuthDbContext dbContext,
    UserManager<User> userManager,
    IOptionsMonitor<JwtSettings> options,
    TokenProvider tokenProvider,
    ILogger<UserLoginCommandHandler> logger) : IRequestHandler<UserLoginCommand, UserLoginResponse>
{
    public async Task<UserLoginResponse> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to log in user with email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("Login failed: user with email {Email} not found", request.Email);
            throw new UnauthorizedException("Invalid login or password.");
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            logger.LogWarning("Login failed: invalid password for user with email {Email}", request.Email);
            throw new UnauthorizedException("Invalid login or password.");
        }
        
        logger.LogInformation("User {UserId} successfully authenticated. Fetching roles...", user.Id);
        var roles = await userManager.GetRolesAsync(user);
        
        logger.LogInformation("Generating access and refresh tokens for user {UserId}", user.Id);
        var accessToken = tokenProvider.GenerateAccessToken(user, roles);
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(options.CurrentValue.RefreshTokenExpirationInDays)
        };
        
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken); 
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("User {UserId} successfully logged in", user.Id);

        return new UserLoginResponse(accessToken, refreshToken.Token, user.Id, user.Email!);
    }
}