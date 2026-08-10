using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Cash.Application;

public static class CashApplicationRegistration
{
    public static IServiceCollection AddCashApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CashApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(CashApplicationRegistration).Assembly);
        return services;
    }
}