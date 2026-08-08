using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Infrastructure.Persistence;
using RadiologyCenter.Insurance.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Infrastructure.Services;

namespace RadiologyCenter.Insurance.Infrastructure;

public static class InsuranceInfrastructureRegistration
{
    public static IServiceCollection AddInsuranceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var storageRoot = configuration.GetSection("Insurance:Storage")["RootPath"] ?? "App_Data/Insurance";

        services.AddDbContext<InsuranceDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>()));

        services.Configure<DocumentStorageOptions>(o => o.RootPath = storageRoot);

        services.AddScoped<IInsuranceCompanyRepository, InsuranceCompanyRepository>();
        services.AddScoped<IInsurancePolicyRepository, InsurancePolicyRepository>();
        services.AddScoped<IPreAuthorizationRepository, PreAuthorizationRepository>();
        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<IPolicyDocumentRepository, PolicyDocumentRepository>();
        services.AddScoped<IPreAuthorizationDocumentRepository, PreAuthorizationDocumentRepository>();
        services.AddScoped<IInsuranceDocumentStorage, InsuranceDocumentStorage>();
        services.AddScoped<IInsuranceUnitOfWork, InsuranceUnitOfWork>();

        return services;
    }
}