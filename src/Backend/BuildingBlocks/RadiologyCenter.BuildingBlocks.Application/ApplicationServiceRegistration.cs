using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RadiologyCenter.BuildingBlocks.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
        return services;
    }
}
