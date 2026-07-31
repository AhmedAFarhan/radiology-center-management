using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;

namespace RadiologyCenter.BuildingBlocks.Application;

public static class WolverineConfigurationExtensions
{
    public static IHostBuilder ConfigureWolverine(this IHostBuilder hostBuilder) =>
        hostBuilder.UseWolverine(opts =>
        {
            opts.RestoreV5Defaults();
            opts.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);
        })
        .ConfigureServices(services => services.AddWolverineExtension<WolverineDiscoveryExtension>());
}
