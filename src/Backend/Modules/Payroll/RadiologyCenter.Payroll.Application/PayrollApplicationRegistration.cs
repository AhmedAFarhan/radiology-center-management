using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Services;

namespace RadiologyCenter.Payroll.Application;

public static class PayrollApplicationRegistration
{
    public static IServiceCollection AddPayrollApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(PayrollApplicationRegistration).Assembly);
        services.AddScoped<IPayslipCalculator, PayrollPayslipCalculator>();
        ApplicationAssemblyRegistry.Register(typeof(PayrollApplicationRegistration).Assembly);
        PayrollMappingConfig.Configure();
        return services;
    }
}