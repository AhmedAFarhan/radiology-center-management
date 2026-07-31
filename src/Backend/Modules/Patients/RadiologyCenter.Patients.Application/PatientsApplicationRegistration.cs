using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Patients.Application;

public static class PatientsApplicationRegistration
{
    public static IServiceCollection AddPatientsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(PatientsApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(PatientsApplicationRegistration).Assembly);
        PatientsMappingConfig.Configure();
        return services;
    }
}
