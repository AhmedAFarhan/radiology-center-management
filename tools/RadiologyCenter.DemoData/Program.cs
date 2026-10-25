using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RadiologyCenter.Clinic;
using RadiologyCenter.Clinic.Dispatching;

// This developer tool is deliberately separate from application startup and publishing.
// It never provisions, migrates, resets accounts, or clears a database.
if (!args.Contains("--apply"))
{
    Console.WriteLine("Add DEMO training records to an existing, empty LOCAL development clinic.\n" +
        "Usage: dotnet run --project tools/RadiologyCenter.DemoData -- --apply --config <appsettings.Development.json> --database <expected-name> --output <private-journal-folder> [--admin admin]");
    return;
}

string Option(string name, string? fallback = null)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : fallback ?? throw new ArgumentException($"Missing {name}.");
}

var configPath = Path.GetFullPath(Option("--config"));
if (!Path.GetFileName(configPath).Equals("appsettings.Development.json", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Use an explicit private development settings file.");
var privateConfig = new ConfigurationBuilder().AddJsonFile(configPath).Build();
var sql = new SqlConnectionStringBuilder(privateConfig.GetConnectionString("DefaultConnection"));
var expectedDatabase = Option("--database");
var host = sql.DataSource.Split('\\')[0].Split(',')[0].Replace("tcp:", "", StringComparison.OrdinalIgnoreCase);
if (!sql.IntegratedSecurity || !new[] { ".", "(local)", "localhost", Environment.MachineName }.Contains(host, StringComparer.OrdinalIgnoreCase)
    || !sql.InitialCatalog.Equals(expectedDatabase, StringComparison.Ordinal) || new[] { "master", "model", "msdb", "tempdb" }.Contains(expectedDatabase, StringComparer.OrdinalIgnoreCase))
    throw new InvalidOperationException("This tool requires local Windows-authenticated SQL and an exact non-system database name.");

var output = Path.GetFullPath(Option("--output"));
Directory.CreateDirectory(output);
var manifestPath = Path.Combine(output, "manifest.json");
var jsonOptions = new JsonSerializerOptions(ClinicDispatcher.JsonOptions) { WriteIndented = true };
var manifest = File.Exists(manifestPath)
    ? JsonSerializer.Deserialize<SeedJournal>(File.ReadAllText(manifestPath), jsonOptions) ?? throw new InvalidOperationException("Invalid journal.")
    : new SeedJournal { Server = sql.DataSource, Database = expectedDatabase, StartedAt = DateTime.UtcNow,
        CairoDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"))) };
if (manifest.Server != sql.DataSource || manifest.Database != expectedDatabase)
    throw new InvalidOperationException("The journal belongs to another database.");

await using var connection = new SqlConnection(sql.ConnectionString);
await connection.OpenAsync();
async Task<object?> Scalar(string command, params (string, object)[] parameters)
{
    await using var query = connection.CreateCommand();
    query.CommandText = command;
    foreach (var (name, value) in parameters) query.Parameters.AddWithValue(name, value);
    var result = await query.ExecuteScalarAsync();
    return result is DBNull ? null : result;
}
void SaveJournal()
{
    var temporary = manifestPath + ".tmp";
    File.WriteAllText(temporary, JsonSerializer.Serialize(manifest, jsonOptions));
    File.Move(temporary, manifestPath, overwrite: true);
}

if (Convert.ToInt32(await Scalar("SELECT IS_SRVROLEMEMBER('sysadmin')")) != 1)
    throw new InvalidOperationException("Run as a local SQL administrator so a verified backup can be taken first.");
var adminName = Option("--admin", "admin");
var adminId = await Scalar("SELECT u.Id FROM [Identity].[AspNetUsers] u WHERE u.UserName=@name AND u.IsActive=1 AND EXISTS(SELECT 1 FROM [Identity].[AspNetUserRoles] ur JOIN [Identity].[AspNetRoles] r ON r.Id=ur.AssignedRolesId WHERE ur.UserId=u.Id AND r.Name=N'Administrator')", ("@name", adminName));
if (adminId is not Guid administratorId) throw new InvalidOperationException("The existing active administrator was not found.");
async Task<string> AdminFingerprint()
{
    var snapshot = Convert.ToString(await Scalar("SELECT CONCAT(PasswordHash,'|',SecurityStamp,'|',ConcurrencyStamp,'|',IsActive,'|',MustChangePassword,'|',AccessFailedCount,'|',LockoutEnd) FROM [Identity].[AspNetUsers] WHERE Id=@id", ("@id", administratorId)))!;
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(snapshot)));
}
var fingerprint = await AdminFingerprint();
if (manifest.AdminFingerprint is not null && manifest.AdminFingerprint != fingerprint)
    throw new InvalidOperationException("The administrator changed since the seed started. Review the journal before resuming.");
manifest.AdminFingerprint = fingerprint;
if (manifest.BackupPath is null)
{
    var existing = Convert.ToInt32(await Scalar("SELECT (SELECT COUNT(*) FROM Patients.Patients)+(SELECT COUNT(*) FROM Examinations.Examinations)+(SELECT COUNT(*) FROM ResourceManagement.Staff)+(SELECT COUNT(*) FROM Inventory.Items)"));
    if (existing != 0) throw new InvalidOperationException("First application requires the previously reviewed empty development clinic. No existing records will be cleared.");
    var backupDirectory = Convert.ToString(await Scalar("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(512))"))!;
    var backupPath = Path.Combine(backupDirectory, $"{expectedDatabase}-before-demo-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.bak");
    await using var backup = connection.CreateCommand();
    backup.CommandTimeout = 180;
    backup.CommandText = "BACKUP DATABASE " + "[" + expectedDatabase.Replace("]", "]]") + "] TO DISK=@path WITH COPY_ONLY,CHECKSUM; RESTORE VERIFYONLY FROM DISK=@path WITH CHECKSUM;";
    backup.Parameters.AddWithValue("@path", backupPath);
    await backup.ExecuteNonQueryAsync();
    manifest.BackupPath = backupPath;
    SaveJournal();
    Console.WriteLine("Pre-seed backup created and verified.");
}

var secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
var runtimeConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:DefaultConnection"] = sql.ConnectionString,
    ["Database:MigrateOnStartup"] = "false", ["DatabaseAuthorization:Required"] = "false",
    ["Jwt:SecretKey"] = secret, ["Jwt:Issuer"] = "EGcare.DemoData", ["Jwt:Audience"] = "EGcare.DemoData",
    ["Jwt:AccessTokenExpirationMinutes"] = "30", ["Jwt:RefreshTokenExpirationDays"] = "1",
    ["Clinic:TimeZoneId"] = "Africa/Cairo"
}).Build();
await using var runtime = await ClinicRuntime.StartAsync(runtimeConfig, "Development");
await runtime.CheckReadinessAsync();
// SQL sysadmin has already verified the existing administrator. This in-memory token
// lets the standalone maintenance tool call the same validated operations as the UI.
var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken("EGcare.DemoData", "EGcare.DemoData",
    [new Claim("sub", administratorId.ToString()), new Claim("unique_name", adminName), new Claim("isAdmin", "true"), new Claim("timezone", "Africa/Cairo")],
    expires: DateTime.UtcNow.AddHours(3), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha512)));
var seed = new DemoSeeder(runtime, connection, token, manifest, SaveJournal, output, args.Contains("--provision-test-sql-accounts"));
try
{
    await seed.RunAsync();
    if (await AdminFingerprint() != fingerprint) throw new InvalidOperationException("Administrator state changed unexpectedly.");
    manifest.CompletedAt = DateTime.UtcNow;
    SaveJournal();
    Console.WriteLine($"DEMO dataset complete. Journal: {manifestPath}");
}
catch (Exception ex)
{
    SaveJournal();
    Console.Error.WriteLine(ex.Message);
    Console.Error.WriteLine("Progress is recorded. Correct the cause and rerun the same command/output folder to resume.");
    Environment.ExitCode = 1;
}

public sealed class SeedJournal
{
    public string Server { get; set; } = "";
    public string Database { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateOnly CairoDate { get; set; }
    public string? BackupPath { get; set; }
    public string? AdminFingerprint { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Dictionary<string, JsonNode?> Steps { get; set; } = new();
}
