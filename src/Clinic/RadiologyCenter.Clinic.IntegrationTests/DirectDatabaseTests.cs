using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.Clinic.Security;
using RadiologyCenter.IntegrationTests.Shared;
using Xunit;

namespace RadiologyCenter.Clinic.IntegrationTests;

[CollectionDefinition("Direct SQL",DisableParallelization=true)]
public sealed class DirectSqlCollection : ICollectionFixture<DirectSqlFixture> { }

public sealed class DirectSqlFixture : IAsyncLifetime
{
    public CustomWebApplicationFactory Factory { get; private set; }=null!;
    public string MaintenanceConnection { get; private set; }="";
    public string AdminConnection { get; private set; }="";
    public ClinicRuntime Admin { get; private set; }=null!;
    public string Prefix { get; private set; }="";
    public async Task InitializeAsync()
    {
        var server=Environment.GetEnvironmentVariable("EGCARE_DIRECT_TEST_SQL_SERVER")??"(localdb)\\EGcareIntegrationTests";
        var prior=Environment.GetEnvironmentVariable("RADIOLOGY_TEST_SQL_SERVER");
        try { Environment.SetEnvironmentVariable("RADIOLOGY_TEST_SQL_SERVER",server); Factory=CustomWebApplicationFactory.CreateContained(); }
        finally { Environment.SetEnvironmentVariable("RADIOLOGY_TEST_SQL_SERVER",prior); }
        MaintenanceConnection=Factory.Services.GetService(typeof(IConfiguration)) is IConfiguration config
            ? config.GetConnectionString("DefaultConnection")! : throw new InvalidOperationException();
        Factory.AssertTestDatabase(MaintenanceConnection);
        await using var connection=new SqlConnection(MaintenanceConnection); await connection.OpenAsync();
        Prefix=new ClinicConnectionProfile { Server=server,Database=Factory.DatabaseName }.LoginPrefix;
        await DesktopSecurityInstaller.InstallAsync(connection,Prefix);
        await using(var command=connection.CreateCommand())
        {
            command.CommandText="DECLARE @id uniqueidentifier=(SELECT Id FROM [Identity].[AspNetUsers] WHERE UserName=@user); EXEC [Security].[ProvisionDesktopUser] @id,@password;";
            command.Parameters.AddWithValue("@user",TestCredentials.UserName); command.Parameters.AddWithValue("@password",TestCredentials.Password);
            await command.ExecuteNonQueryAsync();
        }
        AdminConnection=Account(TestCredentials.UserName,TestCredentials.Password);
        await using(var adminConnection=new SqlConnection(AdminConnection))
        {
            await adminConnection.OpenAsync();
            await ChangePassword(adminConnection,TestCredentials.Password,"ClinicInitial1!"+Guid.NewGuid().ToString("N"));
        }
        // Keep the new password private, but obtain its exact value from the helper below instead of test output.
        AdminConnection=Account(TestCredentials.UserName,_changedPassword!);
        Admin=await ClinicRuntime.StartAsync(Configuration(AdminConnection));
        await Admin.CheckReadinessAsync();
    }
    private string? _changedPassword;
    private async Task ChangePassword(SqlConnection connection,string oldPassword,string password)
    {
        await using var command=connection.CreateCommand(); command.CommandText="Security.ChangeDesktopPassword";
        command.CommandType=System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@OldPassword",oldPassword); command.Parameters.AddWithValue("@NewPassword",password);
        await command.ExecuteNonQueryAsync(); _changedPassword=password;
    }
    public string Account(string user,string password)=>new SqlConnectionStringBuilder(MaintenanceConnection)
        { IntegratedSecurity=false,UserID=Prefix+user,Password=password }.ConnectionString;
    public async Task<(string User,string Password,Guid UserId,Guid RoleId)> CreateAccountAsync(IEnumerable<string> permissions)
    {
        var user="staff"+Guid.NewGuid().ToString("N")[..12]; var roleName="Role"+user;
        var role=await Admin.Dispatcher.DispatchAsync(new("POST","roles",System.Text.Json.JsonSerializer.Serialize(new { name=roleName,description="Isolated production permissions" })));
        Assert.True(role.IsSuccess,System.Text.Json.JsonSerializer.Serialize(role.Value));
        await using var connection=new SqlConnection(AdminConnection); await connection.OpenAsync();
        await using var lookup=connection.CreateCommand(); lookup.CommandText="SELECT Id FROM [Identity].[AspNetRoles] WHERE Name=@name";
        lookup.Parameters.AddWithValue("@name",roleName); var roleId=(Guid)(await lookup.ExecuteScalarAsync())!;
        foreach(var permission in permissions)
        {
            var result=await Admin.Dispatcher.DispatchAsync(new("POST",$"roles/{roleId}/permissions",System.Text.Json.JsonSerializer.Serialize(new { permissionCode=permission })));
            Assert.True(result.IsSuccess,System.Text.Json.JsonSerializer.Serialize(result.Value));
        }
        var initial="Initial1!"+Guid.NewGuid().ToString("N");
        var created=await Admin.Dispatcher.DispatchAsync(new("POST","users",System.Text.Json.JsonSerializer.Serialize(new
            { userName=user,email=user+"@clinic.test",firstName="Test",lastName="Staff",phoneNumber="01012345678",password=initial,roleIds=new[]{roleId} })));
        Assert.True(created.IsSuccess,System.Text.Json.JsonSerializer.Serialize(created.Value));
        lookup.CommandText="SELECT Id FROM [Identity].[AspNetUsers] WHERE UserName=@name"; lookup.Parameters["@name"].Value=user;
        var id=(Guid)(await lookup.ExecuteScalarAsync())!;
        var password="Changed1!"+Guid.NewGuid().ToString("N");
        await using var account=new SqlConnection(Account(user,initial)); await account.OpenAsync(); await ChangePassword(account,initial,password);
        return (user,password,id,roleId);
    }
    public static IConfiguration Configuration(string connection)=>new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
    {
        ["ConnectionStrings:DefaultConnection"]=connection,["DatabaseAuthorization:Required"]="true",
        ["Jwt:Issuer"]="EGcare",["Jwt:Audience"]="EGcare",["Jwt:SecretKey"]=Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        ["Jwt:AccessTokenExpirationMinutes"]="30",["Jwt:RefreshTokenExpirationDays"]="1"
    }).Build();
    public static async Task Execute(SqlConnection connection,string sql)
    { await using var command=connection.CreateCommand(); command.CommandText=sql; command.CommandTimeout=120; await command.ExecuteNonQueryAsync(); }
    public async Task DisposeAsync() { if(Admin is not null) await Admin.DisposeAsync(); Factory?.Dispose(); }
}

[Collection("Direct SQL")]
public sealed class DirectDatabaseTests(DirectSqlFixture fixture)
{
    [Fact]
    public async Task SqlPasswordCommandsRejectNullAndOversizedPasswords()
    {
        var account=await fixture.CreateAccountAsync(Array.Empty<string>());
        await using var connection=new SqlConnection(fixture.Account(account.User,account.Password)); await connection.OpenAsync();
        foreach(var value in new object[]{DBNull.Value,"LongPassword1!"+new string('x',128)})
        {
            await using var command=connection.CreateCommand(); command.CommandText="Security.ChangeDesktopPassword";
            command.CommandType=System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@OldPassword",account.Password); command.Parameters.AddWithValue("@NewPassword",value);
            var error=await Assert.ThrowsAsync<SqlException>(()=>command.ExecuteNonQueryAsync()); Assert.Equal(51004,error.Number);
        }
        await using var missing=connection.CreateCommand(); missing.CommandText="Security.ChangeDesktopPassword";
        missing.CommandType=System.Data.CommandType.StoredProcedure;
        missing.Parameters.AddWithValue("@OldPassword",DBNull.Value); missing.Parameters.AddWithValue("@NewPassword","ValidNewPassword1!");
        Assert.Equal(51004,(await Assert.ThrowsAsync<SqlException>(()=>missing.ExecuteNonQueryAsync())).Number);
        await using var admin=new SqlConnection(fixture.AdminConnection); await admin.OpenAsync();
        await using var provision=admin.CreateCommand(); provision.CommandText="Security.ProvisionDesktopUser"; provision.CommandType=System.Data.CommandType.StoredProcedure;
        provision.Parameters.AddWithValue("@UserId",account.UserId); provision.Parameters.AddWithValue("@Password",DBNull.Value);
        Assert.Equal(51004,(await Assert.ThrowsAsync<SqlException>(()=>provision.ExecuteNonQueryAsync())).Number);
    }
    [Fact]
    public async Task CashOwnershipCannotBeChangedToBypassAnotherCashiersSession()
    {
        var first=await fixture.CreateAccountAsync(new[]{"cash.sessions.open","cash.sessions.read","cash.entries.add"});
        var other=await fixture.CreateAccountAsync(new[]{"cash.sessions.open","cash.sessions.read","cash.entries.add"});
        await using var runtime=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(fixture.Account(other.User,other.Password)));
        var created=await runtime.Dispatcher.DispatchAsync(new("POST","cash/sessions","{\"openingFloat\":0}")); Assert.True(created.IsSuccess);
        await using var connection=new SqlConnection(fixture.Account(first.User,first.Password)); await connection.OpenAsync();
        var forbidden=await Assert.ThrowsAsync<SqlException>(()=>DirectSqlFixture.Execute(connection,$"UPDATE [Cash].[CashSessions] SET UserId='{first.UserId}' WHERE UserId='{other.UserId}'"));
        Assert.Equal(33504,forbidden.Number);
    }
    [Fact]
    public async Task PasswordResetRevokesAnAlreadyRunningDesktopSession()
    {
        var account=await fixture.CreateAccountAsync(new[]{"patients.read"});
        await using var runtime=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(fixture.Account(account.User,account.Password)));
        Assert.True((await runtime.Dispatcher.DispatchAsync(new("POST","patients/all","{}"))).IsSuccess);
        var reset=await fixture.Admin.Dispatcher.DispatchAsync(new("POST",$"users/{account.UserId}/reset-password","{\"newPassword\":\"ResetForTestOnly1!\"}"));
        Assert.True(reset.IsSuccess,System.Text.Json.JsonSerializer.Serialize(reset.Value));
        Assert.Equal(401,(await runtime.Dispatcher.DispatchAsync(new("POST","patients/all","{}"))).StatusCode);
    }
    [Fact]
    public async Task AnInterruptedSqlLeaseCannotResumeWritesThroughAnotherPooledConnection()
    {
        var account=await fixture.CreateAccountAsync(new[]{"patients.read"});
        await using var runtime=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(fixture.Account(account.User,account.Password)));
        var guard=runtime.Services.GetRequiredService<ClinicSessionGuard>();
        await using var identify=guard.Lease!.CreateCommand(); identify.CommandText="SELECT @@SPID";
        var session=Convert.ToInt32(await identify.ExecuteScalarAsync());
        fixture.Factory.AssertTestDatabase(fixture.MaintenanceConnection);
        await using var admin=new SqlConnection(fixture.MaintenanceConnection); await admin.OpenAsync();
        await DirectSqlFixture.Execute(admin,$"KILL {session}");
        Assert.Equal(401,(await runtime.Dispatcher.DispatchAsync(new("POST","patients/all","{}"))).StatusCode);
    }
    [Fact]
    public async Task AWorkstationRejectsAnIncompatibleDatabaseContract()
    {
        await using var admin=new SqlConnection(fixture.MaintenanceConnection); await admin.OpenAsync();
        await using var query=admin.CreateCommand(); query.CommandText="SELECT ContractHash FROM Security.DesktopSchema WHERE Id=1";
        var hash=(string)(await query.ExecuteScalarAsync())!;
        try
        {
            await DirectSqlFixture.Execute(admin,"UPDATE Security.DesktopSchema SET ContractHash='incompatible' WHERE Id=1");
            var error=await Assert.ThrowsAsync<InvalidOperationException>(()=>fixture.Admin.CheckReadinessAsync());
            Assert.Contains("incompatible",error.Message);
        }
        finally { await DirectSqlFixture.Execute(admin,$"UPDATE Security.DesktopSchema SET ContractHash='{hash}' WHERE Id=1"); }
    }
    [Fact]
    public async Task PasswordChangeRequiresTheCurrentPasswordEvenOnAnAuthenticatedConnection()
    {
        var account=await fixture.CreateAccountAsync(Array.Empty<string>());
        await using var connection=new SqlConnection(fixture.Account(account.User,account.Password)); await connection.OpenAsync();
        await using var command=connection.CreateCommand(); command.CommandType=System.Data.CommandType.StoredProcedure;
        command.CommandText="Security.ChangeDesktopPassword";
        command.Parameters.AddWithValue("@OldPassword","WrongCurrentPassword1!");
        command.Parameters.AddWithValue("@NewPassword","ShouldNotBecomePassword1!");
        await Assert.ThrowsAsync<SqlException>(()=>command.ExecuteNonQueryAsync());
        await using var unchanged=new SqlConnection(fixture.Account(account.User,account.Password)); await unchanged.OpenAsync();
    }
    [Fact]
    public async Task AuthorityComesFromTheSqlIdentity()
    {
        var principal=await new DatabasePrincipalReader(DirectSqlFixture.Configuration(fixture.AdminConnection)).ReadAsync();
        Assert.Equal(TestCredentials.UserName,principal.Identity!.Name);
        Assert.True(principal.HasClaim("isAdmin","true"));
        var result=await fixture.Admin.Dispatcher.DispatchAsync(new("POST","patients/all","{\"pageNumber\":1,\"pageSize\":10}",AccessToken:"forged.client.claims"));
        Assert.True(result.IsSuccess);
    }
    [Fact]
    public async Task IncorrectPasswordCannotOpenTheDatabase()
    {
        await using var connection=new SqlConnection(fixture.Account(TestCredentials.UserName,"WrongPassword1!"));
        await Assert.ThrowsAsync<SqlException>(()=>connection.OpenAsync());
    }
    [Fact]
    public async Task SignedOutAuthCommandsCannotMintProductionAuthority()
    {
        var result=await fixture.Admin.Dispatcher.DispatchAsync(new("POST","auth/login","{}"));
        Assert.Equal(403,result.StatusCode);
    }
    [Fact]
    public async Task DatabasePoliciesRejectAnUnprivilegedAccountAndSpoofedSessionContext()
    {
        var account=await fixture.CreateAccountAsync(Array.Empty<string>());
        await using var staff=new SqlConnection(fixture.Account(account.User,account.Password)); await staff.OpenAsync();
        await DirectSqlFixture.Execute(staff,"EXEC sys.sp_set_session_context @key=N'userId',@value=N'Administrator'; EXEC sys.sp_set_session_context @key=N'isAdmin',@value=1;");
        await using(var command=staff.CreateCommand()) { command.CommandText="SELECT COUNT(*) FROM [Identity].[AspNetUsers]"; Assert.Equal(0,(int)(await command.ExecuteScalarAsync())!); }
        var mutation=await Assert.ThrowsAsync<SqlException>(()=>DirectSqlFixture.Execute(staff,"INSERT [System].[NumberSequences](Name,Year,LastNumber) VALUES ('Escalation',2026,1)"));
        Assert.Equal(33504,mutation.Number);
        await Assert.ThrowsAsync<SqlException>(()=>DirectSqlFixture.Execute(staff,"UPDATE [Security].[DesktopAccounts] SET IsAdministrator=1"));
        var principal=await new DatabasePrincipalReader(DirectSqlFixture.Configuration(fixture.Account(account.User,account.Password))).ReadAsync();
        Assert.Equal(account.User,principal.Identity!.Name); Assert.False(principal.HasClaim("isAdmin","true")); Assert.False(principal.HasClaim("mustChangePassword","true"));
        await Assert.ThrowsAsync<SqlException>(()=>DirectSqlFixture.Execute(staff,$"EXEC [Security].[ProvisionDesktopUser] @UserId='{account.UserId}',@Password=N'AttemptedReset1!'"));
    }
    [Fact]
    public async Task TwoIndependentClientsShareCommittedPatientsWithoutPrivilegedDatabaseAccounts()
    {
        var account=await fixture.CreateAccountAsync(new[]{"patients.create","patients.read","patients.update","cash.sessions.open","cash.sessions.read","users.read","roles.read"});
        var connection=fixture.Account(account.User,account.Password);
        await using var first=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(connection));
        await using var second=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(connection));
        var created=await first.Dispatcher.DispatchAsync(new("POST","patients",System.Text.Json.JsonSerializer.Serialize(new
            { fullName="Direct Shared Patient",gender="Male",age=35,phoneNumber="01012345678" })));
        Assert.True(created.IsSuccess,System.Text.Json.JsonSerializer.Serialize(created.Value));
        var list=await second.Dispatcher.DispatchAsync(new("POST","patients/all","{\"searchTerm\":\"Direct Shared Patient\"}"));
        Assert.True(list.IsSuccess); Assert.Contains("Direct Shared Patient",System.Text.Json.JsonSerializer.Serialize(list.Value));
        var cash=await first.Dispatcher.DispatchAsync(new("POST","cash/sessions","{\"openingFloat\":20}"));
        Assert.True(cash.IsSuccess,System.Text.Json.JsonSerializer.Serialize(cash.Value));
        var users=await first.Dispatcher.DispatchAsync(new("POST","users/all","{}"));
        Assert.True(users.IsSuccess); Assert.DoesNotContain("passwordHash",System.Text.Json.JsonSerializer.Serialize(users.Value),StringComparison.OrdinalIgnoreCase);
        var roles=await first.Dispatcher.DispatchAsync(new("POST","roles/all","{}")); Assert.True(roles.IsSuccess);
        var adminOnly=await first.Dispatcher.DispatchAsync(new("POST","users","{}")); Assert.Equal(403,adminOnly.StatusCode);
        await using var raw=new SqlConnection(connection); await raw.OpenAsync();
        await using(var permissions=raw.CreateCommand()) { permissions.CommandText="SELECT IS_MEMBER('db_owner'),HAS_PERMS_BY_NAME(DB_NAME(),'DATABASE','ALTER ANY USER')";
            await using var rows=await permissions.ExecuteReaderAsync(); Assert.True(await rows.ReadAsync()); Assert.Equal(0,rows.GetInt32(0)); Assert.Equal(0,rows.GetInt32(1)); }
        await DirectSqlFixture.Execute(raw,"INSERT [System].[NumberSequences](Name,Year,LastNumber) VALUES ('OwnSequence',2026,1)");
        await using var admin=new SqlConnection(fixture.AdminConnection); await admin.OpenAsync();
        await DirectSqlFixture.Execute(admin,$"UPDATE [Identity].[AspNetUsers] SET IsActive=0 WHERE Id='{account.UserId}'");
        var deactivated=await second.Dispatcher.DispatchAsync(new("POST","patients/all","{}")); Assert.Equal(401,deactivated.StatusCode);
        await Assert.ThrowsAsync<SqlException>(()=>DirectSqlFixture.Execute(raw,"INSERT [System].[NumberSequences](Name,Year,LastNumber) VALUES ('RevokedSequence',2026,1)"));
    }
    [Fact]
    public void ClinicProfileRejectsUnsafeAddressesAndContainsNoPassword()
    {
        Assert.Throws<InvalidOperationException>(()=>new ClinicConnectionProfile { Server="clinic;TrustServerCertificate=True",Database="Clinic" }.Validate());
        Assert.Throws<InvalidOperationException>(()=>new ClinicConnectionProfile { Server="clinic",Database="Clinic",PacsUrl="http://clinic:8042" }.Validate());
        var profile=new ClinicConnectionProfile { Server="clinic,1433",Database="Clinic" };
        var connection=new SqlConnectionStringBuilder(profile.ConnectionString("reception","StrongPassword1!"));
        Assert.False(connection.TrustServerCertificate); Assert.Equal(SqlConnectionEncryptOption.Mandatory,connection.Encrypt);
        Assert.DoesNotContain("Password",System.Text.Json.JsonSerializer.Serialize(profile));
    }
}
