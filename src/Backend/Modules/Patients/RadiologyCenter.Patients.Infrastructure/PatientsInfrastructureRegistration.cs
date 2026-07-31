using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Patients.Infrastructure.Persistence;

namespace RadiologyCenter.Patients.Infrastructure;

public static class PatientsInfrastructureRegistration
{
    public static IServiceCollection AddPatientsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PatientsDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>()));

        services.AddScoped<IUnitOfWork<PatientsDbContext>, UnitOfWork<PatientsDbContext>>();

        return services;
    }
}
