using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Identity.Application;

public static class IdentityApplicationRegistration
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(IdentityApplicationRegistration).Assembly);
        services.AddScoped<RadiologyCenter.Identity.Application.Abstractions.IIdentityAccessProvisioner, RadiologyCenter.Identity.Application.Abstractions.NoIdentityAccessProvisioner>();
        ApplicationAssemblyRegistry.Register(typeof(IdentityApplicationRegistration).Assembly);
        IdentityMappingConfig.Configure();
        return services;
    }
}
