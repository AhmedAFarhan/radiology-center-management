using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Clinic.Security;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Persistence;

namespace RadiologyCenter.Clinic;

/// <summary>Explicit admin-PC maintenance. Existing databases are backed up and verified before migrations.</summary>
public static class ClinicMaintenance
{
    public static async Task RunAsync(ClinicConnectionProfile profile,string maintenanceConnection,bool createNew,
        string? backupFile,string? administratorUserName,string? administratorPassword,string? legacyDocumentsRoot=null,
        CancellationToken ct=default)
    {
        profile.Validate();
        await RunCoreAsync(profile,maintenanceConnection,createNew,backupFile,administratorUserName,administratorPassword,legacyDocumentsRoot,ct);
    }
    // The test assembly exercises maintenance against isolated LocalDB databases. Public profiles remain LAN addresses.
    internal static async Task RunCoreAsync(ClinicConnectionProfile profile,string maintenanceConnection,bool createNew,
        string? backupFile,string? administratorUserName,string? administratorPassword,string? legacyDocumentsRoot=null,
        CancellationToken ct=default)
    {
        var source=new SqlConnectionStringBuilder(maintenanceConnection);
        if(source.DataSource!=profile.Server || source.InitialCatalog!=profile.Database)
            throw new InvalidOperationException("Maintenance must target the selected clinic database.");
        if(!source.IntegratedSecurity) throw new InvalidOperationException("Use your Windows database administrator identity for maintenance.");
        var master=new SqlConnectionStringBuilder(source.ConnectionString) { InitialCatalog="master" };
        await using(var server=new SqlConnection(master.ConnectionString))
        {
            await server.OpenAsync(ct);
            await using var check=server.CreateCommand(); check.CommandText="SELECT DB_ID(@name)"; check.Parameters.AddWithValue("@name",profile.Database);
            var exists=await check.ExecuteScalarAsync(ct) is not DBNull;
            if(createNew && exists) throw new InvalidOperationException("That database already exists. Select upgrade and backup instead.");
            if(!createNew && !exists) throw new InvalidOperationException("The selected database does not exist.");
            if(createNew)
            {
                // This is an explicit admin setup action. Ordinary clients never change server configuration.
                await Execute(server,"EXEC sp_configure 'contained database authentication',1; RECONFIGURE;",ct);
            }
            if(createNew)
            {
                ValidatePassword(administratorPassword);
                if(string.IsNullOrWhiteSpace(administratorUserName)) throw new InvalidOperationException("Enter the initial clinic administrator username.");
                await Execute(server,$"CREATE DATABASE {DesktopSecurityInstaller.Quote(profile.Database)} CONTAINMENT=PARTIAL",ct);
            }
        }
        await using var connection=new SqlConnection(source.ConnectionString); await connection.OpenAsync(ct);
        await Execute(connection,"DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'EGcare.SchemaMaintenance',@LockOwner=N'Session',@LockMode=N'Exclusive',@LockTimeout=0; IF @r<0 THROW 51005,'Close EGcare on every workstation before maintenance.',1;",ct);
        try
        {
        if(!createNew)
        {
            if(string.IsNullOrWhiteSpace(backupFile) || !Path.IsPathFullyQualified(backupFile))
                throw new InvalidOperationException("Enter an absolute backup file path on the SQL Server machine.");
            // COPY_ONLY preserves the clinic's existing backup chain. A new filename is mandatory.
            await using(var backup=connection.CreateCommand())
            {
                backup.CommandTimeout=600;
                backup.CommandText=$"DECLARE @files TABLE(FileExists int,IsDirectory int,ParentExists int); INSERT @files EXEC master.dbo.xp_fileexist @path; IF EXISTS(SELECT 1 FROM @files WHERE FileExists=1) THROW 51004,'Use a new backup filename; existing backups are never overwritten.',1; BACKUP DATABASE {DesktopSecurityInstaller.Quote(profile.Database)} TO DISK=@path WITH COPY_ONLY,CHECKSUM,NOINIT; RESTORE VERIFYONLY FROM DISK=@path WITH CHECKSUM";
                backup.Parameters.AddWithValue("@path",backupFile); await backup.ExecuteNonQueryAsync(ct);
            }
            await using(var server=new SqlConnection(master.ConnectionString))
            { await server.OpenAsync(ct); await Execute(server,"EXEC sp_configure 'contained database authentication',1; RECONFIGURE;",ct); }
            await Execute(connection,$"IF (SELECT containment FROM sys.databases WHERE name=DB_NAME())=0 ALTER DATABASE {DesktopSecurityInstaller.Quote(profile.Database)} SET CONTAINMENT=PARTIAL",ct);
        }
        var config=new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
        {
            ["ConnectionStrings:DefaultConnection"]=source.ConnectionString,
            ["Jwt:SecretKey"]=Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            ["Jwt:Issuer"]="EGcare",["Jwt:Audience"]="EGcare",["Jwt:AccessTokenExpirationMinutes"]="30",["Jwt:RefreshTokenExpirationDays"]="1",
            ["Seed:AdminUserName"]=administratorUserName??"admin",["Seed:AdminPassword"]=administratorPassword??""
        }).Build();
        await using var runtime=await ClinicRuntime.StartAsync(config,provision:true,ct:ct);
        await ImportDocumentsAsync(connection,legacyDocumentsRoot,ct);
        await DesktopSecurityInstaller.InstallAsync(connection,profile.LoginPrefix,ct);
        if(!string.IsNullOrWhiteSpace(administratorUserName) && !string.IsNullOrEmpty(administratorPassword))
        {
            await using var scope=runtime.Services.CreateAsyncScope();
            var context=scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var user=await context.Users.Include(u=>u.AssignedRoles).FirstOrDefaultAsync(u=>u.UserName==administratorUserName,ct);
            if(user is null || !user.IsActive || user.LockoutEnd>DateTimeOffset.UtcNow
                || !user.AssignedRoles.Any(r=>r.IsSystem && r.Name=="Administrator" && r.IsActive)
                )
                throw new InvalidOperationException("Use the current password of an active clinic administrator. Maintenance never resets or unlocks existing accounts.");
            await using(var binding=connection.CreateCommand())
            {
                binding.CommandText="SELECT PrincipalName FROM [Security].[DesktopAccounts] WHERE UserId=@id";
                binding.Parameters.AddWithValue("@id",user.Id);
                if(await binding.ExecuteScalarAsync(ct) is string principal)
                {
                    // Once bound, SQL Server is the password authority. Existing SQL passwords are preserved.
                    var account=new SqlConnectionStringBuilder(source.ConnectionString) { IntegratedSecurity=false,UserID=principal,Password=administratorPassword };
                    await using var authenticate=new SqlConnection(account.ConnectionString); await authenticate.OpenAsync(ct);
                }
                else if(new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash!,administratorPassword)==PasswordVerificationResult.Failed)
                    throw new InvalidOperationException("The current clinic administrator password is incorrect.");
            }
            await using var command=connection.CreateCommand();
            command.CommandText="IF NOT EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] WHERE UserId=@id) EXEC [Security].[ProvisionDesktopUser] @id,@password";
            command.Parameters.AddWithValue("@id",user.Id); command.Parameters.AddWithValue("@password",administratorPassword);
            await command.ExecuteNonQueryAsync(ct);
        }
        await runtime.CheckReadinessAsync(ct);
        }
        finally
        {
            // Session locks survive pooled SqlConnection disposal until the connection is reused.
            // Explicit release lets workstations sign in immediately after maintenance.
            if(connection.State==ConnectionState.Open)
                await Execute(connection,"EXEC sys.sp_releaseapplock @Resource=N'EGcare.SchemaMaintenance',@LockOwner=N'Session';",CancellationToken.None);
        }
    }
    public static void ValidatePassword(string? password)
    {
        if(!RadiologyCenter.BuildingBlocks.Application.Validation.PasswordPolicyRule.IsStrongPassword(password))
            throw new InvalidOperationException("Use 12–128 characters with uppercase, lowercase, a number and a symbol.");
    }
    private static async Task ImportDocumentsAsync(SqlConnection connection,string? root,CancellationToken ct)
    {
        foreach(var table in new[]{"PolicyDocuments","PreAuthorizationDocuments"})
        {
            var documents=new List<(Guid Id,string Path)>();
            await using(var read=connection.CreateCommand())
            {
                read.CommandText=$"SELECT Id,StoredPath FROM [Insurance].{DesktopSecurityInstaller.Quote(table)} WHERE StoredPath NOT LIKE 'db:%'";
                await using var reader=await read.ExecuteReaderAsync(ct);
                while(await reader.ReadAsync(ct)) documents.Add((reader.GetGuid(0),reader.GetString(1)));
            }
            if(documents.Count>0 && string.IsNullOrWhiteSpace(root))
                throw new InvalidOperationException("Select the existing insurance documents folder before upgrading this database.");
            foreach(var document in documents)
            {
                var directory=Path.GetFullPath(root!).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
                var path=Path.GetFullPath(Path.Combine(directory,document.Path));
                if(!path.StartsWith(directory,StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
                    throw new InvalidOperationException("A legacy insurance document is missing from the selected folder. Restore it before continuing.");
                if(new FileInfo(path).Length>20*1024*1024) throw new InvalidOperationException("A legacy document exceeds 20 MB. Resolve it before importing.");
                var bytes=await File.ReadAllBytesAsync(path,ct); var id=Guid.NewGuid();
                await using var transaction=(SqlTransaction)await connection.BeginTransactionAsync(ct);
                await using var copy=connection.CreateCommand(); copy.Transaction=transaction;
                copy.CommandText=$"INSERT [Insurance].[DocumentContents](Id,Content) VALUES(@contentId,@content); UPDATE [Insurance].{DesktopSecurityInstaller.Quote(table)} SET StoredPath=@path WHERE Id=@id";
                copy.Parameters.AddWithValue("@contentId",id); copy.Parameters.Add("@content",SqlDbType.VarBinary,-1).Value=bytes;
                copy.Parameters.AddWithValue("@path","db:"+id.ToString("D")); copy.Parameters.AddWithValue("@id",document.Id);
                await copy.ExecuteNonQueryAsync(ct); await transaction.CommitAsync(ct);
            }
        }
    }
    private static async Task Execute(SqlConnection connection,string sql,CancellationToken ct)
    { await using var command=connection.CreateCommand(); command.CommandText=sql; command.CommandTimeout=120; await command.ExecuteNonQueryAsync(ct); }
}
