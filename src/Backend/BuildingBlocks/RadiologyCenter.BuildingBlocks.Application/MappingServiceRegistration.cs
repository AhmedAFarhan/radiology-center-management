using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace RadiologyCenter.BuildingBlocks.Application;

public static class MappingServiceRegistration
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, Mapper>();
        return services;
    }
}
