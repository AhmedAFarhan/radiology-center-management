using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(
                FindConnectionString(),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name))
            .Options;

        return new AppDbContext(options);
    }

    private static string FindConnectionString()
    {
        var basePath = Directory.GetCurrentDirectory();
        while (basePath is not null)
        {
            var configPath = Path.Combine(basePath, "appsettings.json");
            if (File.Exists(configPath))
            {
                var connectionString = new ConfigurationBuilder()
                    .AddJsonFile(configPath)
                    .Build()
                    .GetConnectionString("DefaultConnection");

                if (connectionString is not null)
                    return connectionString;
            }

            basePath = Path.GetDirectoryName(basePath);
        }

        return "Server=.;Database=RadiologyCenter;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
