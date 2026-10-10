using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.IntegrationTests.Shared;
using Xunit;

namespace RadiologyCenter.Clinic.IntegrationTests;

[Collection("Direct SQL")]
public sealed class MaintenanceTests
{
    [Fact]
    public async Task UpgradeBacksUpAnExistingDatabaseAndCanBeRunAgainWithoutResettingAccounts()
    {
        // Maintenance is Windows-only. Linux CI verifies the same installer through the direct account suite.
        if (!OperatingSystem.IsWindows() || Environment.GetEnvironmentVariable("RADIOLOGY_TEST_CONNECTION_STRING") is not null) return;
        var previous=Environment.GetEnvironmentVariable("RADIOLOGY_TEST_SQL_SERVER");
        string connection; string database;
        try
        {
            Environment.SetEnvironmentVariable("RADIOLOGY_TEST_SQL_SERVER","(localdb)\\EGcareIntegrationTests");
            using var factory=new CustomWebApplicationFactory();
            connection=factory.Services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")!;
            factory.AssertTestDatabase(connection); database=factory.DatabaseName;
        }
        finally { Environment.SetEnvironmentVariable("RADIOLOGY_TEST_SQL_SERVER",previous); }
        SqlConnection.ClearAllPools();
        var directory=Path.Combine(Path.GetTempPath(),"EGcareMaintenanceTests",Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        var backup=Path.Combine(directory,"before-upgrade.bak");
        var profile=new ClinicConnectionProfile { Server=new SqlConnectionStringBuilder(connection).DataSource,Database=database };
        await ClinicMaintenance.RunCoreAsync(profile,connection,false,backup,TestCredentials.UserName,TestCredentials.Password);
        Assert.True(File.Exists(backup)); Assert.True(new FileInfo(backup).Length>0);
        await using(var db=new SqlConnection(connection))
        {
            await db.OpenAsync(); await using var command=db.CreateCommand();
            command.CommandText="SELECT Version FROM Security.DesktopSchema WHERE Id=1";
            Assert.Equal(1,(int)(await command.ExecuteScalarAsync())!);
        }
        var overwrite=await Assert.ThrowsAsync<SqlException>(()=>ClinicMaintenance.RunCoreAsync(profile,connection,false,backup,null,null));
        Assert.Equal(51004,overwrite.Number);
        SqlConnection.ClearAllPools();
        await ClinicMaintenance.RunCoreAsync(profile,connection,false,Path.Combine(directory,"before-second-upgrade.bak"),null,null);
        var account=new SqlConnectionStringBuilder(connection) { IntegratedSecurity=false,UserID=profile.LoginPrefix+TestCredentials.UserName,Password=TestCredentials.Password };
        await using var existingUser=new SqlConnection(account.ConnectionString); await existingUser.OpenAsync();
        await using var activeClient=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(account.ConnectionString));
        await activeClient.CheckReadinessAsync();
        var busy=await Assert.ThrowsAsync<SqlException>(()=>ClinicMaintenance.RunCoreAsync(profile,connection,false,Path.Combine(directory,"must-not-run.bak"),null,null));
        Assert.Equal(51005,busy.Number); Assert.False(File.Exists(Path.Combine(directory,"must-not-run.bak")));
    }
}
