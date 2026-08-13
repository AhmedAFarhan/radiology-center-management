using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Domain;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Settings;

namespace RadiologyCenter.Identity.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options) => _options = options.Value;

    public TokenResult GenerateTokenResult(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

        return new TokenResult(
            GenerateAccessToken(user, expiresAt),
            GenerateRefreshToken(),
            expiresAt,
            DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays),
            user.MustChangePassword);
    }

    private string GenerateAccessToken(User user, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
        };

        foreach (var role in user.AssignedRoles)
            claims.Add(new("role", role.Name!));

        if (user.AssignedRoles.Any(r => r.IsSystem))
        {
            claims.Add(new("isAdmin", "true"));
            foreach (var permission in Permissions.All)
                claims.Add(new("permission", permission.Code));
        }
        else
        {
            foreach (var permission in user.GetEffectivePermissions())
                claims.Add(new("permission", permission));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
