using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Reports.Application;

public static class ReportsApplicationRegistration
{
    public static IServiceCollection AddReportsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ReportsApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(ReportsApplicationRegistration).Assembly);
        ReportsMappingConfig.Configure();
        return services;
    }
}