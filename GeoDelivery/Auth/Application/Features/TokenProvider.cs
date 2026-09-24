using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Settings;
using Auth.Domain;
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
}