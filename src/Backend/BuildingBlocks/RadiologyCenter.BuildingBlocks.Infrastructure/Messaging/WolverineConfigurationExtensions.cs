using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.SqlServer;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Messaging;

public static class WolverineConfigurationExtensions
{
    public static IHostBuilder ConfigureWolverine(this IHostBuilder hostBuilder, string connectionString) =>
        hostBuilder.UseWolverine(opts =>
        {
            opts.RestoreV5Defaults();
            opts.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);
            opts.UseEntityFrameworkCoreTransactions();
            opts.Durability.MessageStorageSchemaName = "Wolverine";
            opts.PersistMessagesWithSqlServer(connectionString);
        })
        .ConfigureServices(services => services.AddWolverineExtension<WolverineDiscoveryExtension>());
}
