using JasperFx.Resources;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Infrastructure.Messaging;
using RadiologyCenter.Identity.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using RadiologyCenter.Clinic.Security;

namespace RadiologyCenter.Clinic;

/// <summary>A generic business host embedded in MAUI. Does not create or listen on any HTTP endpoint.</summary>
public sealed class ClinicRuntime : IAsyncDisposable
{
    private readonly IHost _host;
    private SqlConnection? _databaseLease;
    private int _disposed;
    private ClinicRuntime(IHost host) => _host = host;
    public IServiceProvider Services => _host.Services;
    public ClinicDispatcher Dispatcher => Services.GetRequiredService<ClinicDispatcher>();

    public static async Task<ClinicRuntime> StartAsync(IConfiguration configuration, string environment = "Production",
        bool provision = false, CancellationToken ct = default, ILoggerFactory? loggerFactory = null)
    {
        var builder = Host.CreateDefaultBuilder(Array.Empty<string>()).UseEnvironment(environment)
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureAppConfiguration((_, config) => { config.Sources.Clear(); config.AddConfiguration(configuration);
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["Clinic:DirectDatabase"] = "true" }); })
            .ConfigureServices((context, services) => services.AddClinicModules(context.Configuration))
            .ConfigureLogging(logging =>
            {
                if (environment == "Testing") return;
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Warning);
                // SQL command errors and request URLs can contain patient values.
                logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
                logging.AddFilter("Microsoft.EntityFrameworkCore.Update", LogLevel.None);
                logging.AddFilter("System.Net.Http.HttpClient", LogLevel.None);
                // Wolverine's default failure logs serialize the command, including passwords and patient fields.
                logging.AddFilter("Wolverine", LogLevel.None);
                // Invocation loggers also use the message's application/domain namespace as their category.
                logging.AddFilter((category, level) => level >= LogLevel.Warning
                    && !(category?.StartsWith("RadiologyCenter.", StringComparison.Ordinal) == true
                        && (category.Contains(".Application.", StringComparison.Ordinal)
                            || category.Contains(".Domain.", StringComparison.Ordinal))));
                if (loggerFactory is not null) logging.AddProvider(new ForwardingLoggerProvider(loggerFactory));
            })
            .ConfigureWolverine();
        var host = builder.Build();
        try
        {
            Translator.Current = host.Services.GetRequiredService<ITranslator>();
            _ = host.Services.GetRequiredService<IOptions<JwtOptions>>().Value;
            if (provision || environment == "Testing")
            {
                // Provisioning is explicit and is never part of ordinary workstation startup.
                // Configuration is mutable for this newly built private host only.
                host.Services.GetRequiredService<IConfiguration>()["Database:MigrateOnStartup"] = "true";
                await host.Services.MigrateAndSeedAsync(Path.Combine(AppContext.BaseDirectory, "Resources"));
                host.Services.GetRequiredService<IConfiguration>()["Database:MigrateOnStartup"] = "false";
            }
            var runtime=new ClinicRuntime(host);
            try
            {
                if(configuration.GetValue<bool>("DatabaseAuthorization:Required"))
                {
                    var leaseConnection=new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection")) { ConnectRetryCount=0 };
                    runtime._databaseLease=new SqlConnection(leaseConnection.ConnectionString);
                    await runtime._databaseLease.OpenAsync(ct);
                    await using var lease=runtime._databaseLease.CreateCommand();
                    lease.CommandText="DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'EGcare.SchemaMaintenance',@LockOwner=N'Session',@LockMode=N'Shared',@LockTimeout=10000; IF @r<0 THROW 51005,'Database maintenance is in progress. Try again after it finishes.',1;";
                    await lease.ExecuteNonQueryAsync(ct);
                    host.Services.GetRequiredService<ClinicSessionGuard>().Lease=runtime._databaseLease;
                    using var principalScope=host.Services.CreateScope();
                    var principal=await principalScope.ServiceProvider.GetRequiredService<DatabasePrincipalReader>().ReadAsync(ct);
                    host.Services.GetRequiredService<ClinicSessionGuard>().CredentialVersion=principal.FindFirst("sqlCredentialVersion")?.Value;
                }
                await host.StartAsync(ct);
                return runtime;
            }
            catch { await runtime.DisposeAsync(); throw; }
        }
        catch { host.Dispose(); throw; }
    }

    public async Task CheckReadinessAsync(CancellationToken ct = default)
    {
        await using var scope = Services.CreateAsyncScope();
        var knownMigrations=new HashSet<string>(StringComparer.Ordinal);
        var tables=new HashSet<(string Schema,string Table)> { ("System","DesktopEvents"),("Insurance","DocumentContents") };
        string[]? applied=null;
        foreach (var type in DatabaseExtensions.DbContextTypes)
        {
            var context = (DbContext)scope.ServiceProvider.GetRequiredService(type);
            knownMigrations.UnionWith(context.Database.GetMigrations());
            applied ??= (await context.Database.GetAppliedMigrationsAsync(ct)).ToArray();
            foreach(var entity in context.Model.GetEntityTypes())
                if(entity.GetTableName() is {} table) tables.Add((entity.GetSchema()??"dbo",table));
            if ((await context.Database.GetPendingMigrationsAsync(ct)).Any())
                throw new InvalidOperationException("The shared database needs an upgrade. Run maintenance once on the admin PC.");
        }
        if(applied!.Any(id=>!knownMigrations.Contains(id)))
            throw new InvalidOperationException("This database was upgraded by a newer EGcare version. Update this workstation from the clinic update folder.");
        var configuration=Services.GetRequiredService<IConfiguration>();
        if(configuration.GetValue<bool>("DatabaseAuthorization:Required"))
        {
            await using var connection=new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync(ct);
            await using var command=connection.CreateCommand(); command.CommandText="SELECT Version,ContractHash FROM [Security].[DesktopSchema] WHERE Id=1";
            await using var reader=await command.ExecuteReaderAsync(ct);
            if(!await reader.ReadAsync(ct) || reader.GetInt32(0)!=DesktopSecurityInstaller.SchemaVersion
                || reader.GetString(1)!=DesktopSecurityInstaller.ContractHash(tables))
                throw new InvalidOperationException("The desktop and shared database versions are incompatible. Run maintenance on the admin PC.");
        }
    }
    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref _disposed,1)!=0) return;
        // Keep the maintenance lease until every dispatched SQL operation has finished.
        await Services.GetRequiredService<ClinicSessionGuard>().StopAndDrainAsync();
        try { await _host.StopAsync(); }
        finally
        {
            try
            {
                if(_databaseLease?.State==System.Data.ConnectionState.Open)
                {
                    await using var release=_databaseLease.CreateCommand();
                    release.CommandText="EXEC sys.sp_releaseapplock @Resource=N'EGcare.SchemaMaintenance',@LockOwner=N'Session';";
                    await release.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                if(_databaseLease is not null) await _databaseLease.DisposeAsync();
                if (_host is IAsyncDisposable asyncHost) await asyncHost.DisposeAsync(); else _host.Dispose();
            }
        }
    }
    private sealed class ForwardingLoggerProvider(ILoggerFactory factory) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => factory.CreateLogger(categoryName);
        public void Dispose() { }
    }
}
