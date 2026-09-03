using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            config.Sources.Clear();
            config.AddInMemoryCollection(overrides);
        });

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });
    }
}
