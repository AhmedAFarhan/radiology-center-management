using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Insurance.Application;

public static class InsuranceApplicationRegistration
{
    public static IServiceCollection AddInsuranceApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(InsuranceApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(InsuranceApplicationRegistration).Assembly);
        return services;
    }
}