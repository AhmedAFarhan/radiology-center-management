using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Inventory.Application;

public static class InventoryApplicationRegistration
{
    public static IServiceCollection AddInventoryApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(InventoryApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(InventoryApplicationRegistration).Assembly);
        InventoryMappingConfig.Configure();
        return services;
    }
}
