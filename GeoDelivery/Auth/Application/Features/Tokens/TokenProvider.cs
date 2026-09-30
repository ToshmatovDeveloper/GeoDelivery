using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Settings;
using Auth.Domain;
using Auth.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Features;

public class TokenProvider(IOptionsMonitor<JwtSettings> options)
{
    public string GenerateAccessToken(User user, IList<string> roles)
    {
        string secretKey = options.CurrentValue.Secret;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(options.CurrentValue.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = options.CurrentValue.Issuer,
            Audience = options.CurrentValue.Audience
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public string GenerateAccessTokenFromRefreshToken(string refreshToken, string refreshSecretKey, IList<string> roles)
    {
        var refreshKey = Encoding.UTF8.GetBytes(refreshSecretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(refreshKey),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        var handler = new JsonWebTokenHandler();

        try
        {
            var validationResult = handler.ValidateToken(refreshToken, validationParameters);

            if (!validationResult.IsValid)
            {
                throw new SecurityTokenException("Invalid or expired refresh token");
            }
            
            var userId = validationResult.ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? validationResult.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new SecurityTokenException("The token is missing the user identifier.");
            }
            
            var accessClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
            };

            foreach (var role in roles)
            {
                accessClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var accessKey = Encoding.UTF8.GetBytes(options.CurrentValue.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(accessClaims),
                Expires = DateTime.UtcNow.AddMinutes(options.CurrentValue.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(accessKey),
                    SecurityAlgorithms.HmacSha256Signature) 
            };

            return handler.CreateToken(tokenDescriptor);
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Error validating or generating token", ex);
        }
    }
    
    public record CleanOldTokensCommand : IRequest<bool>;

    public class CleanOldTokensCommandHandler(AuthDbContext db) : IRequestHandler<CleanOldTokensCommand, bool>
    {
        public async Task<bool> Handle(CleanOldTokensCommand request, CancellationToken cancellationToken)
        {
            var latestTokenIds = await db.RefreshTokens
                .GroupBy(rt => rt.UserId)
                .Select(g => g.OrderByDescending(rt => rt.CreatedOn).Select(rt => rt.Id).FirstOrDefault())
                .ToListAsync(cancellationToken);

            if (latestTokenIds.Count == 0) 
                return false;

            var deletedCount = await db.RefreshTokens
                .Where(rt => !latestTokenIds.Contains(rt.Id))
                .ExecuteDeleteAsync(cancellationToken);

            return true;
        }
    }
}