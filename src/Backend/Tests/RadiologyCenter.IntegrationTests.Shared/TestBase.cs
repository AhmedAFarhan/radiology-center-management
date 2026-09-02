using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RadiologyCenter.IntegrationTests.Shared;

public abstract class TestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected CustomWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    protected TestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}
