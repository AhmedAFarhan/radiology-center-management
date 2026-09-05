using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Infrastructure.Persistence;
using RadiologyCenter.Patients.Infrastructure.Repositories;

namespace RadiologyCenter.Patients.Infrastructure;

public static class PatientsInfrastructureRegistration
{
    public static IServiceCollection AddPatientsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PatientsDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())
);

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IPatientsUnitOfWork, PatientsUnitOfWork>();

        return services;
    }
}
