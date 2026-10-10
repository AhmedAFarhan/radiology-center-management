using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.Settings;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Authorization;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Identity.Infrastructure.Repositories;
using RadiologyCenter.Identity.Infrastructure.Services;
using RadiologyCenter.Identity.Infrastructure.Settings;

namespace RadiologyCenter.Identity.Infrastructure;

public static class IdentityInfrastructureRegistration
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<IdentityDbContext>((sp, options) =>
            options.UseSqlServer(sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"))
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())
);

        services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddOptions<JwtOptions>().Bind(configuration.GetSection("Jwt"))
            .Validate(o => Encoding.UTF8.GetByteCount(o.SecretKey) >= 64 && !o.SecretKey.Contains("Development", StringComparison.OrdinalIgnoreCase), "Configure a private randomly generated JWT signing key of at least 64 bytes.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer) && !string.IsNullOrWhiteSpace(o.Audience), "JWT issuer and audience are required.")
            .Validate(o => o.AccessTokenExpirationMinutes is >= 1 and <= 60 && o.RefreshTokenExpirationDays is >= 1 and <= 30, "Token lifetimes are outside the supported bounds.")
            .ValidateOnStart();
        services.Configure<AccountLockoutOptions>(configuration.GetSection("Lockout"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        // The embedded dispatcher performs authentication and permission checks.
        // Core authorization has no dependency on HTTP endpoint routing services.
        services.AddAuthorizationCore();

        return services;
    }
}
