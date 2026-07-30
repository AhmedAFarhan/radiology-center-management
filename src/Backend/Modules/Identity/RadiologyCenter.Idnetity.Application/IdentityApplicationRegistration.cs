using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RadiologyCenter.Idnetity.Application;

public static class IdentityApplicationRegistration
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(IdentityApplicationRegistration).Assembly);
        return services;
    }
}
