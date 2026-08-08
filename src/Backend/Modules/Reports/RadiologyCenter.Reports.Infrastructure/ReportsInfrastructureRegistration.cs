using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Infrastructure.Persistence;
using RadiologyCenter.Reports.Infrastructure.Repositories;

namespace RadiologyCenter.Reports.Infrastructure;

public static class ReportsInfrastructureRegistration
{
    public static IServiceCollection AddReportsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ReportsDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>()));

        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IReportTemplateRepository, ReportTemplateRepository>();
        services.AddScoped<IReportsUnitOfWork, ReportsUnitOfWork>();

        return services;
    }
}