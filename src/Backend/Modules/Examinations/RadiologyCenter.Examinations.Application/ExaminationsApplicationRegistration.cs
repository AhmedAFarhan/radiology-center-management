using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Adapters;

namespace RadiologyCenter.Examinations.Application;

public static class ExaminationsApplicationRegistration
{
    public static IServiceCollection AddExaminationsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ExaminationsApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(ExaminationsApplicationRegistration).Assembly);
        ExaminationsMappingConfig.Configure();

        services.AddScoped<IPatientInfoResolver, PatientInfoResolver>();

        return services;
    }
}
