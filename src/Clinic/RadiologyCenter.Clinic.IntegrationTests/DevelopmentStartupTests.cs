using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.IntegrationTests.Shared;
using Xunit;

namespace RadiologyCenter.Clinic.IntegrationTests;

[Collection("Direct SQL")]
public sealed class DevelopmentStartupTests(DirectSqlFixture fixture)
{
    [Fact]
    public async Task DevelopmentHostValidatesServicesAndRunsBusinessWithoutHttpRouting()
    {
        var config = (IConfiguration)DirectSqlFixture.Configuration(fixture.MaintenanceConnection);
        config["DatabaseAuthorization:Required"] = "false";
        await using var runtime = await ClinicRuntime.StartAsync(config, "Development");
        await runtime.CheckReadinessAsync();
        var login = await runtime.Dispatcher.DispatchAsync(new("POST", "auth/login",
            JsonSerializer.Serialize(new { userName = TestCredentials.UserName, password = TestCredentials.Password })));
        Assert.True(login.IsSuccess);
        using var envelope = JsonDocument.Parse(JsonSerializer.Serialize(login.Value, ClinicDispatcher.JsonOptions));
        var token = envelope.RootElement.GetProperty("data").GetProperty("accessToken").GetString();
        Assert.True((await runtime.Dispatcher.DispatchAsync(new("POST", "patients/all", "{}", AccessToken: token))).IsSuccess);
    }
}
