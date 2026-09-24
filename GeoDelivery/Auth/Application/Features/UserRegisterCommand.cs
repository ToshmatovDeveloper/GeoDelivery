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

public record UserRegisterCommand(
    string UserName,
    string Email,
    string Password) : IRequest<UserRegisterResponse>;

public record UserRegisterResponse(
    string AccessToken,
    string RefreshToken,
    string Message) : IRequest<UserRegisterResponse>;
   
public class UserRegisterCommandHandler(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    AuthDbContext dbContext,
    IOptionsMonitor<JwtSettings> optionsMonitor,
    TokenProvider tokenProvider,
    ILogger<UserRegisterCommandHandler> logger) : IRequestHandler<UserRegisterCommand, UserRegisterResponse>
{
    public async Task<UserRegisterResponse> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling user registration request");

        var user = new User(request.UserName, request.Email);

        var userNameIsNotVacant =
            await dbContext.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);

        if (userNameIsNotVacant)
        {
            logger.LogWarning("User name is not vacant");

            throw new UserNameIsAlreadyInUseException("User name is not vacant");
        }
        
        var emailIsNotValid = await dbContext.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsNotValid)
        {
            logger.LogWarning("Email is already in use");
            
            throw new EmailIsAlreadyInUseException("Email is already in use.");
        }
        
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            logger.LogError("Failed to create user");
            
            throw new UserCreateFailedException("Failed to create user.");
        }

        var role = new Role("User");

        if (!await roleManager.RoleExistsAsync(role.Name!))
        {
            await roleManager.CreateAsync(role);
        }

        var roleAddResult = await userManager.AddToRoleAsync(user, role.Name!);

        if (!roleAddResult.Succeeded)
        {
            logger.LogError("Failed to add role to user");
            throw new FailedAddUserRoleException("Failed to add role to user.");
        }

        var roles = await userManager.GetRolesAsync(user);

        var accessToken = tokenProvider.GenerateAccessToken(user, roles);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(optionsMonitor.CurrentValue.RefreshTokenExpirationInDays)
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("User successfully registered");

        return new UserRegisterResponse(accessToken, refreshToken.Token, "Welcome to GeoDelivery");

    }
}
    