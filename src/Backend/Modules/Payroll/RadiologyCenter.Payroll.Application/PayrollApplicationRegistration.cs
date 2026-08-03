using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Payroll.Application;

public static class PayrollApplicationRegistration
{
    public static IServiceCollection AddPayrollApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(PayrollApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(PayrollApplicationRegistration).Assembly);
        return services;
    }
}