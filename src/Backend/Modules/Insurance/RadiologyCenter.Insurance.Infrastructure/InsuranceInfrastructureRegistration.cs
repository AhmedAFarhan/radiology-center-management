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

        services.AddDbContext<InsuranceDbContext>((sp, options) =>
            options.UseSqlServer(sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"))
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())
);

        services.AddOptions<DocumentStorageOptions>().Configure<IConfiguration>((o, config) =>
        {
            var configured = config["Insurance:Storage:RootPath"];
            o.RootPath = string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EGcare", "insurance")
                : configured;
            if (!Path.IsPathFullyQualified(o.RootPath))
                throw new InvalidOperationException("Insurance storage must use an absolute data directory.");
        });

        services.AddScoped<IInsuranceCompanyRepository, InsuranceCompanyRepository>();
        services.AddScoped<IInsurancePolicyRepository, InsurancePolicyRepository>();
        services.AddScoped<IPreAuthorizationRepository, PreAuthorizationRepository>();
        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();
        services.AddScoped<IPolicyDocumentRepository, PolicyDocumentRepository>();
        services.AddScoped<IPreAuthorizationDocumentRepository, PreAuthorizationDocumentRepository>();
        services.AddScoped<IInsuranceDocumentStorage, InsuranceDocumentStorage>();
        services.AddScoped<IInsuranceUnitOfWork, InsuranceUnitOfWork>();

        return services;
    }
}