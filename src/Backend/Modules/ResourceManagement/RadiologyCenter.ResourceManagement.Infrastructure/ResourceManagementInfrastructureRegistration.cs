using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Infrastructure.Repositories;

namespace RadiologyCenter.ResourceManagement.Infrastructure;

public static class ResourceManagementInfrastructureRegistration
{
    public static IServiceCollection AddResourceManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ResourceManagementDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())
);

        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();
        services.AddScoped<ILeaveRepository, LeaveRepository>();
        services.AddScoped<IReferralDoctorRepository, ReferralDoctorRepository>();
        services.AddScoped<IResourceManagementUnitOfWork, ResourceManagementUnitOfWork>();

        return services;
    }
}
