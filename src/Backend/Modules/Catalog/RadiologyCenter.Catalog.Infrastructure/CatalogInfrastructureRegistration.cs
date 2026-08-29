using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Infrastructure.Persistence;
using RadiologyCenter.Catalog.Infrastructure.Repositories;

namespace RadiologyCenter.Catalog.Infrastructure;

public static class CatalogInfrastructureRegistration
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CatalogDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>()));

        services.AddScoped<IExaminationTypeRepository, ExaminationTypeRepository>();
        services.AddScoped<ICatalogUnitOfWork, CatalogUnitOfWork>();

        return services;
    }
}