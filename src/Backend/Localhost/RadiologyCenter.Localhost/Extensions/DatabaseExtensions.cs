using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Identity.Infrastructure.Persistence.Seed;

namespace RadiologyCenter.Localhost.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedAsync(this IServiceProvider services, string? resourcesPath = null)
    {
        using var scope = services.CreateScope();

        var dbContextTypes = new[]
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

        foreach (var dbContextType in dbContextTypes)
        {
            var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(dbContextType);
            await dbContext.Database.MigrateAsync();
        }

        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Identity.Domain.Entities.User>>();

        await IdentityDbSeeder.SeedAsync(
            identityDb,
            passwordHasher,
            resourcesPath: resourcesPath);
    }
}
