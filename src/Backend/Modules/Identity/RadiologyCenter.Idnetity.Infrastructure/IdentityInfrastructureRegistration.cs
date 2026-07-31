using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Domain.Entities;
using RadiologyCenter.Idnetity.Infrastructure.Authorization;
using RadiologyCenter.Idnetity.Infrastructure.Persistence;
using RadiologyCenter.Idnetity.Infrastructure.Repositories;
using RadiologyCenter.Idnetity.Infrastructure.Services;
using RadiologyCenter.Idnetity.Infrastructure.Settings;

namespace RadiologyCenter.Idnetity.Infrastructure;

public static class IdentityInfrastructureRegistration
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        services.AddDbContext<IdentityDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>()));

        services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            };
        });

        services.AddAuthorization();

        return services;
    }
}
