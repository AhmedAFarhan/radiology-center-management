using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Catalog.Application;

public static class CatalogApplicationRegistration
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CatalogApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(CatalogApplicationRegistration).Assembly);
        CatalogMappingConfig.Configure();
        return services;
    }
}