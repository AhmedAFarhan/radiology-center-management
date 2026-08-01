using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Examinations.Application;

public static class ExaminationsApplicationRegistration
{
    public static IServiceCollection AddExaminationsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ExaminationsApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(ExaminationsApplicationRegistration).Assembly);
        ExaminationsMappingConfig.Configure();
        return services;
    }
}
