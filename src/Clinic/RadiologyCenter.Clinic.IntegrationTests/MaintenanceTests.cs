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
    public async Task NewClinicContainsOnlyTheInitialAdministratorAndNoDemoRecords()
    {
        if (!OperatingSystem.IsWindows() || Environment.GetEnvironmentVariable("RADIOLOGY_TEST_CONNECTION_STRING") is not null) return;
        var database = "RadiologyCenter_IntegrationTests_" + Guid.NewGuid().ToString("N");
        var profile = new ClinicConnectionProfile { Server = "(localdb)\\EGcareIntegrationTests", Database = database };
        var connection = new SqlConnectionStringBuilder
        {
            DataSource = profile.Server, InitialCatalog = database,
            IntegratedSecurity = true, TrustServerCertificate = true, Pooling = false
        };
        try
        {
            await ClinicMaintenance.RunCoreAsync(profile, connection.ConnectionString, true, null, TestCredentials.UserName, TestCredentials.Password);
            await using var db = new SqlConnection(connection.ConnectionString);
            await db.OpenAsync();
            await using var command = db.CreateCommand();
            command.CommandText = "SELECT (SELECT COUNT(*) FROM [Identity].[AspNetUsers]), (SELECT COUNT(*) FROM [Patients].[Patients]), (SELECT COUNT(*) FROM [Examinations].[Examinations]), (SELECT COUNT(*) FROM [Security].[DesktopAccounts]);";
            await using (var reader = await command.ExecuteReaderAsync())
            {
                Assert.True(await reader.ReadAsync());
                Assert.Equal(1, reader.GetInt32(0));
                Assert.Equal(0, reader.GetInt32(1));
                Assert.Equal(0, reader.GetInt32(2));
                Assert.Equal(1, reader.GetInt32(3));
            }
            var account = new SqlConnectionStringBuilder(connection.ConnectionString)
            { IntegratedSecurity = false, UserID = profile.LoginPrefix + TestCredentials.UserName, Password = TestCredentials.Password };
            await using var signIn = new SqlConnection(account.ConnectionString);
            await signIn.OpenAsync();
            // Repeating "new clinic" must never overwrite or reset the existing database.
            await Assert.ThrowsAsync<InvalidOperationException>(() => ClinicMaintenance.RunCoreAsync(
                profile, connection.ConnectionString, true, null, TestCredentials.UserName, TestCredentials.Password));
        }
        finally
        {
            SqlConnection.ClearAllPools();
            connection.InitialCatalog = "master";
            await using var cleanup = new SqlConnection(connection.ConnectionString);
            await cleanup.OpenAsync();
            await using var command = cleanup.CreateCommand();
            // The target is generated in this test; it cannot be a configured clinic database.
            command.CommandText = $"IF DB_ID(@name) IS NOT NULL BEGIN ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]; END";
            command.Parameters.AddWithValue("@name", database);
            await command.ExecuteNonQueryAsync();
        }
    }

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
