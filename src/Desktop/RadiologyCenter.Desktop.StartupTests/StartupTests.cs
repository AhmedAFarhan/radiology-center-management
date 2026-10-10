using System.Text;
using Microsoft.Extensions.Logging;
using RadiologyCenter.Clinic;
using RadiologyCenter.Desktop.Security;
using RadiologyCenter.Desktop.Services;
using Xunit;

public sealed class StartupTests
{
    [Fact]
    public void DatabaseNameCasingDoesNotChangeTheClinicSqlAccountPrefix()
        => Assert.Equal(new ClinicConnectionProfile { Database="EGcareClinic" }.LoginPrefix,new ClinicConnectionProfile { Database="egcareclinic" }.LoginPrefix);
    [Theory]
    [InlineData("CorrectClinic1!",true)]
    [InlineData("onlylowercase1!",false)]
    [InlineData("UpperAndLower١!",false)]
    [InlineData("Áonlylowercase1!",false)]
    public void NewPasswordValidationMatchesSqlServerRequirements(string password,bool expected)
        => Assert.Equal(expected,RadiologyCenter.BuildingBlocks.Application.Validation.PasswordPolicyRule.IsStrongPassword(password));
    [Fact]
    public void SignInRetainsEveryPermissionAndRoleFromTheIssuedSession()
    {
        var payload = "{\"sub\":\"user-id\",\"permission\":[\"patients.read\",\"examinations.create\",\"cash.sessions.open\"],\"role\":[\"Reception\",\"Cashier\"]}";
        var claims = JwtClaimsParser.Parse(Token(payload));
        Assert.Equal("user-id", claims.Get("sub"));
        Assert.Equal(new[] { "patients.read", "examinations.create", "cash.sessions.open" }, claims.GetAll("permission"));
        Assert.Equal(new[] { "Reception", "Cashier" }, claims.GetAll("role"));
    }
    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("a.%%%")]
    public void CorruptSessionDataLeavesTheUserUnauthenticated(string? token)
        => Assert.Empty(JwtClaimsParser.Parse(token).GetAll("permission"));

    [Fact]
    public void PublicConnectionFileCanBeSharedWithoutCredentials()
    {
        var directory = TemporaryDirectory();
        try
        {
            var profile = new ClinicConnectionProfile { Server = "ADMIN-PC,1433", Database = "EGcareClinic", PacsUrl = "https://images.clinic.test:8042" };
            var path = Path.Combine(directory, "clinic.json"); profile.Write(path);
            Assert.Equal(profile, ClinicConnectionProfile.Read(path));
            Assert.DoesNotContain("password", File.ReadAllText(path), StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("UserID", File.ReadAllText(path), StringComparison.OrdinalIgnoreCase);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }
    [Fact]
    public void InvalidConnectionFilesProduceARecoverableSetupState()
    {
        var previous = Environment.GetEnvironmentVariable("EGCARE_CLIENT_CONFIG_PATH");
        var directory = TemporaryDirectory();
        try
        {
            var path = Path.Combine(directory, "clinic.json");
            Environment.SetEnvironmentVariable("EGCARE_CLIENT_CONFIG_PATH", path);
            Assert.Null(DesktopConfiguration.Profile);
            File.WriteAllText(path, "broken JSON");
            Assert.Null(DesktopConfiguration.Profile);
            Assert.False(DesktopConfiguration.UseClinicServer);
            new ClinicConnectionProfile { Server = "ADMIN-PC", Database = "EGcareClinic" }.Write(path);
            Assert.True(DesktopConfiguration.UseClinicServer);
        }
        finally { Environment.SetEnvironmentVariable("EGCARE_CLIENT_CONFIG_PATH", previous); Directory.Delete(directory, recursive: true); }
    }
    [Theory]
    [InlineData("localhost")]
    [InlineData("tcp:127.0.0.1,1433")]
    [InlineData("np:\\\\localhost\\pipe\\sql\\query")]
    [InlineData("ADMIN-PC,99999")]
    public void ConnectionProfileCannotPointEveryWorkstationAtItself(string server)
        => Assert.Throws<InvalidOperationException>(() => new ClinicConnectionProfile { Server = server, Database = "EGcareClinic" }.Validate());

    [Fact]
    public void ExceptionDiagnosticsDoNotPersistSensitiveExceptionMessages()
    {
        var directory = TemporaryDirectory();
        try
        {
            using (var provider = new SimpleFileLoggerProvider(directory))
                provider.CreateLogger("ClinicTest").LogError(new InvalidOperationException("PatientPrivate PasswordPrivate"), "UI operation failed.");
            var log = File.ReadAllText(Assert.Single(Directory.GetFiles(directory, "app-*.log")));
            Assert.Contains(nameof(InvalidOperationException), log);
            Assert.DoesNotContain("PatientPrivate", log);
            Assert.DoesNotContain("PasswordPrivate", log);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [Fact]
    public void AnUnavailableDiagnosticFolderDoesNotPreventApplicationStartup()
    {
        var directory = TemporaryDirectory();
        try
        {
            var file = Path.Combine(directory, "not-a-directory");
            File.WriteAllText(file, "existing file");
            using var provider = new SimpleFileLoggerProvider(file);
            provider.CreateLogger("ClinicTest").LogWarning("Application remains usable.");
            Assert.Equal("existing file", File.ReadAllText(file));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [Fact]
    public void DiagnosticFilesRotateByUtf8BytesAndRespectRetention()
    {
        var directory = TemporaryDirectory();
        try
        {
            using (var provider = new SimpleFileLoggerProvider(directory, maxFiles: 2))
            {
                var logger = provider.CreateLogger("ClinicTest");
                for (var i = 0; i < 140; i++) logger.LogWarning("{Message}", new string('ش', 40000));
            }
            var files = Directory.GetFiles(directory, "app-*.log");
            Assert.InRange(files.Length, 1, 2);
            Assert.All(files, file => Assert.InRange(new FileInfo(file).Length, 1, 2 * 1024 * 1024));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }
    private static string Token(string json) => "header." + Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_') + ".signature";
    private static string TemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "EGcareDesktopTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path); return path;
    }
}
