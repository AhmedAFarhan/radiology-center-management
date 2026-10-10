using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Infrastructure.Persistence;
using RadiologyCenter.Cash.Infrastructure.Repositories;

namespace RadiologyCenter.Cash.Infrastructure;

public static class CashInfrastructureRegistration
{
    public static IServiceCollection AddCashInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<CashDbContext>((sp, options) =>
            options.UseSqlServer(sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"))
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())
);

        services.AddScoped<ICashSessionRepository, CashSessionRepository>();
        services.AddScoped<ICashEntryRepository, CashEntryRepository>();
        services.AddScoped<ICashHandoverRepository, CashHandoverRepository>();
        services.AddScoped<ICashUnitOfWork, CashUnitOfWork>();

        return services;
    }
}