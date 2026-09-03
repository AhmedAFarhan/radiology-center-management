using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RadiologyCenter.Localhost;

namespace RadiologyCenter.IntegrationTests.Shared;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string TestConnectionString =
        "Server=.;Database=RadiologyCenter_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = TestConnectionString,
            };

            config.AddInMemoryCollection(overrides);
        });
    }
}
