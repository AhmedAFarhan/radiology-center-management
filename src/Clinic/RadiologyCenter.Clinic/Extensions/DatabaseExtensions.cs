using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Identity.Infrastructure.Persistence.Seed;

namespace RadiologyCenter.Clinic.Extensions;

public static class DatabaseExtensions
{
    internal static readonly Type[] DbContextTypes =
    {
        typeof(BuildingBlocks.Infrastructure.Persistence.AppDbContext),
        typeof(IdentityDbContext),
        typeof(Patients.Infrastructure.Persistence.PatientsDbContext),
        typeof(Inventory.Infrastructure.Persistence.InventoryDbContext),
        typeof(Catalog.Infrastructure.Persistence.CatalogDbContext),
        typeof(Examinations.Infrastructure.Persistence.ExaminationsDbContext),
        typeof(ResourceManagement.Infrastructure.Persistence.ResourceManagementDbContext),
        typeof(Payroll.Infrastructure.Persistence.PayrollDbContext),
        typeof(Reports.Infrastructure.Persistence.ReportsDbContext),
        typeof(Insurance.Infrastructure.Persistence.InsuranceDbContext),
        typeof(Cash.Infrastructure.Persistence.CashDbContext),
        typeof(Notification.Infrastructure.Persistence.NotificationDbContext)
    };

    public static async Task MigrateAndSeedAsync(this IServiceProvider services, string? resourcesPath = null)
    {
        using var scope = services.CreateScope();



        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var expectedTestDatabase = configuration["Testing:ExpectedDatabase"];
        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        // Validate every context before ANY migration, seeding or connection is opened.
        if (environment.IsEnvironment("Testing"))
        {
            // Resolving these settings performs the durable-store guard before SQL is touched.
            if (!configuration.GetValue<bool>("Clinic:DirectDatabase"))
                _ = scope.ServiceProvider.GetRequiredService<Wolverine.RDBMS.DatabaseSettings>();
            if (string.IsNullOrWhiteSpace(expectedTestDatabase)
                || !expectedTestDatabase.StartsWith("RadiologyCenter_IntegrationTests_", StringComparison.Ordinal))
                throw new InvalidOperationException("Testing requires a uniquely named isolated database.");
            foreach (var type in DbContextTypes)
            {
                var context = (DbContext)scope.ServiceProvider.GetRequiredService(type);
                if (new SqlConnectionStringBuilder(context.Database.GetConnectionString()).InitialCatalog != expectedTestDatabase)
                    throw new InvalidOperationException($"Unsafe test database configured for {type.Name}.");
            }
        }

        if (!environment.IsEnvironment("Testing") && !configuration.GetValue<bool>("Database:MigrateOnStartup")
            && !Environment.GetCommandLineArgs().Contains("--migrate-only", StringComparer.Ordinal))
            return;

        foreach (var dbContextType in DbContextTypes)
        {
            var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(dbContextType);
            await dbContext.Database.MigrateAsync();
        }

        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Identity.Domain.Entities.User>>();

        await IdentityDbSeeder.SeedAsync(
            identityDb,
            passwordHasher,
            resourcesPath: resourcesPath,
            options: configuration.GetSection("Seed").Get<AdministratorSeedOptions>());
    }
}
