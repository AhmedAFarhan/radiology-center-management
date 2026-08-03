using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.ResourceManagement.Application;

public static class ResourceManagementApplicationRegistration
{
    public static IServiceCollection AddResourceManagementApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ResourceManagementApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(ResourceManagementApplicationRegistration).Assembly);
        ResourceManagementMappingConfig.Configure();
        return services;
    }
}
