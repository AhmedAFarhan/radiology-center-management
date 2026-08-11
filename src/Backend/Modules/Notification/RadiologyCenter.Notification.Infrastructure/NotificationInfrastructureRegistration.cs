using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Infrastructure.Persistence;
using RadiologyCenter.Notification.Infrastructure.Repositories;
using RadiologyCenter.Notification.Infrastructure.Services;

namespace RadiologyCenter.Notification.Infrastructure;

public static class NotificationInfrastructureRegistration
{
    public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<NotificationDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>()));

        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<INotificationMessageRepository, NotificationMessageRepository>();
        services.AddScoped<INotificationUnitOfWork, NotificationUnitOfWork>();

        services.AddScoped<ISmsProvider, LogSmsProvider>();
        services.AddScoped<IEmailProvider, LogEmailProvider>();
        services.AddScoped<IPushProvider, LogPushProvider>();
        services.AddScoped<INotificationSender, NotificationSender>();

        return services;
    }
}