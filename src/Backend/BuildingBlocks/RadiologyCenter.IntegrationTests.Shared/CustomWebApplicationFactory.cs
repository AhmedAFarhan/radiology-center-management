using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using RadiologyCenter.Clinic;
using RadiologyCenter.Clinic.Dispatching;

namespace RadiologyCenter.IntegrationTests.Shared;

// The compatibility name keeps existing test fixtures readable. This host is
// a generic in-process business runtime, with no ASP.NET server or listening port.
public sealed class CustomWebApplicationFactory : IDisposable
{
    public string DatabaseName { get; } = "RadiologyCenter_IntegrationTests_" + Guid.NewGuid().ToString("N");
    public string StorageRoot { get; } = Path.Combine(Path.GetTempPath(), "RadiologyCenterTests", Guid.NewGuid().ToString("N"));
    private readonly ClinicRuntime _runtime;
    public IServiceProvider Services => _runtime.Services;
    public CustomWebApplicationFactory() : this(false) { }
    public static CustomWebApplicationFactory CreateContained() => new(true);
    private CustomWebApplicationFactory(bool contained)
    {
        var connection = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("RADIOLOGY_TEST_CONNECTION_STRING")
            ?? "Server=.;Integrated Security=True;TrustServerCertificate=True");
        connection.DataSource = Environment.GetEnvironmentVariable("RADIOLOGY_TEST_SQL_SERVER") ?? connection.DataSource;
        connection.InitialCatalog = DatabaseName;
        if(contained)
        {
            AssertTestDatabase(connection.ConnectionString);
            var master=new SqlConnectionStringBuilder(connection.ConnectionString) { InitialCatalog="master" };
            using var db=new SqlConnection(master.ConnectionString); db.Open();
            using var command=db.CreateCommand();
            command.CommandText=$"CREATE DATABASE [{DatabaseName}] CONTAINMENT=PARTIAL";
            command.ExecuteNonQuery();
        }
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
        {
            ["ConnectionStrings:DefaultConnection"] = connection.ConnectionString,
            ["Testing:ExpectedDatabase"] = DatabaseName,
            ["Seed:AdminUserName"] = TestCredentials.UserName,
            ["Seed:AdminPassword"] = TestCredentials.Password,
            ["Seed:AdminEmail"] = "integration-admin@radiology.test",
            ["Seed:AdminPhone"] = "01000000000",
            ["Jwt:SecretKey"] = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            ["Jwt:Issuer"] = "RadiologyCenter", ["Jwt:Audience"] = "RadiologyCenter",
            ["Jwt:AccessTokenExpirationMinutes"] = "15", ["Jwt:RefreshTokenExpirationDays"] = "7",
            ["Insurance:Storage:RootPath"] = StorageRoot,
            ["DefaultTimeZone"] = "Africa/Cairo",
            ["Lockout:MaxFailedAccessAttempts"] = "5", ["Lockout:LockoutDurationMinutes"] = "5"
        }).Build();
        _runtime = ClinicRuntime.StartAsync(config,"Testing").GetAwaiter().GetResult();
    }
    public HttpClient CreateClient() => new(new TestTransport(_runtime)) { BaseAddress = new Uri("http://in-process/") };
    public static HttpClient CreateClientFor(ClinicRuntime runtime) => new(new TestTransport(runtime)) { BaseAddress = new Uri("http://in-process/") };
    public void AssertTestDatabase(string? connectionString)
    {
        if (new SqlConnectionStringBuilder(connectionString).InitialCatalog != DatabaseName
            || !DatabaseName.StartsWith("RadiologyCenter_IntegrationTests_",StringComparison.Ordinal))
            throw new InvalidOperationException("The resolved connection is not the isolated test database.");
    }
    public void Dispose() => _runtime.DisposeAsync().AsTask().GetAwaiter().GetResult();

    private sealed class TestTransport(ClinicRuntime runtime) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
        {
            if (request.RequestUri!.AbsolutePath.Trim('/') == "health")
            {
                await runtime.CheckReadinessAsync(ct);
                return new(HttpStatusCode.OK) { Content = new StringContent("Healthy") };
            }
            string? body = null;
            ClinicUpload? upload = null;
            var fields = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            if (request.Content is MultipartFormDataContent multipart)
            {
                foreach (var part in multipart)
                {
                    var disposition = part.Headers.ContentDisposition!;
                    var name = disposition.Name?.Trim('"') ?? "file";
                    var fileName = disposition.FileNameStar ?? disposition.FileName?.Trim('"');
                    if (fileName is not null) upload = new(name,fileName,part.Headers.ContentType?.MediaType ?? "application/octet-stream",await part.ReadAsByteArrayAsync(ct));
                    else fields[name] = await part.ReadAsStringAsync(ct);
                }
            }
            else if (request.Content is not null) body = await request.Content.ReadAsStringAsync(ct);
            var result = await runtime.Dispatcher.DispatchAsync(new ClinicRequest(request.Method.Method,request.RequestUri.PathAndQuery,body,fields,upload,
                request.Headers.Authorization?.Parameter,request.Headers.AcceptLanguage.FirstOrDefault()?.Value ?? "en"),ct);
            HttpContent content;
            if (result.Value is ClinicFile file)
            {
                content = new ByteArrayContent(file.Content);
                content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = file.FileName };
            }
            else content = new StringContent(JsonSerializer.Serialize(result.Value,ClinicDispatcher.JsonOptions),Encoding.UTF8,"application/json");
            return new((HttpStatusCode)result.StatusCode) { Content = content };
        }
    }
}
