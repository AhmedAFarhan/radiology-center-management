using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.SqlServer;
using RadiologyCenter.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Wolverine.Persistence;
using Wolverine.RDBMS;
using Wolverine.Runtime;
using Wolverine.SqlServer.Persistence;
using Wolverine.RDBMS.Sagas;
using Wolverine.Persistence.Durability;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Messaging;

public static class WolverineConfigurationExtensions
{
    public static IHostBuilder ConfigureWolverine(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureServices((context, services) => services.AddWolverine(opts =>
        {
            opts.RestoreV5Defaults();
            opts.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);
            opts.UseEntityFrameworkCoreTransactions();
            if (context.Configuration.GetValue<bool>("Clinic:DirectDatabase"))
            {
                opts.Durability.Mode = DurabilityMode.MediatorOnly;
                return;
            }
            opts.Durability.MessageStorageSchemaName = "Wolverine";
            // This connection is only a registration placeholder. Final settings below
            // are resolved after host overrides; an unconfigured application fails there.
            var initialConnection = context.Configuration.GetConnectionString("DefaultConnection");
            opts.PersistMessagesWithSqlServer(string.IsNullOrWhiteSpace(initialConnection)
                ? "Server=.;Database=EGcare_NotConfigured;Integrated Security=True;TrustServerCertificate=True"
                : initialConnection);
            opts.AutoBuildMessageStorageOnStartup = JasperFx.AutoCreate.None;
            opts.Policies.UseDurableLocalQueues();
        }))
        .ConfigureServices((context, services) =>
        {
            services.AddWolverineExtension<WolverineDiscoveryExtension>();
            if (context.Configuration.GetValue<bool>("Clinic:DirectDatabase")) return;
            // Resolve the final configuration in DI, just as the module DbContexts do.
            // Host/test configuration overrides must apply to the outbox too.
            services.Replace(ServiceDescriptor.Singleton<DatabaseSettings>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connection = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("A database connection is required.");
                if (string.IsNullOrWhiteSpace(connection))
                    throw new InvalidOperationException("Provision the clinic server database connection before starting the API.");
                if (sp.GetRequiredService<IHostEnvironment>().IsEnvironment("Testing"))
                {
                    var expected = configuration["Testing:ExpectedDatabase"];
                    if (expected is null || !expected.StartsWith("RadiologyCenter_IntegrationTests_", StringComparison.Ordinal)
                        || new SqlConnectionStringBuilder(connection).InitialCatalog != expected)
                        throw new InvalidOperationException("Unsafe test database configured for the durable message store.");
                }
                return new DatabaseSettings { ConnectionString = connection, SchemaName = "Wolverine" };
            }));
            services.Replace(ServiceDescriptor.Singleton<IMessageStore>(sp => new SqlServerMessageStore(
                sp.GetRequiredService<DatabaseSettings>(),
                sp.GetRequiredService<IWolverineRuntime>().DurabilitySettings,
                sp.GetRequiredService<ILogger<SqlServerMessageStore>>(),
                sp.GetServices<SagaTableDefinition>())));
        });
}
