using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.Notification.Application;

public static class NotificationApplicationRegistration
{
    public static IServiceCollection AddNotificationApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(NotificationApplicationRegistration).Assembly);
        ApplicationAssemblyRegistry.Register(typeof(NotificationApplicationRegistration).Assembly);
        return services;
    }
}