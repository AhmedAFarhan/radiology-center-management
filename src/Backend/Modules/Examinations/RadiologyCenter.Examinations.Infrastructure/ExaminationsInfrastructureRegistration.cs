using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Infrastructure.Persistence;
using RadiologyCenter.Examinations.Infrastructure.Repositories;

namespace RadiologyCenter.Examinations.Infrastructure;

public static class ExaminationsInfrastructureRegistration
{
    public static IServiceCollection AddExaminationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ExaminationsDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>()));

        services.AddScoped<IExaminationRepository, ExaminationRepository>();
        services.AddScoped<IExaminationHistoryRepository, ExaminationHistoryRepository>();
        services.AddScoped<IExaminationTypeItemRepository, ExaminationTypeItemRepository>();
        services.AddScoped<IExaminationsUnitOfWork, ExaminationsUnitOfWork>();

        return services;
    }
}
