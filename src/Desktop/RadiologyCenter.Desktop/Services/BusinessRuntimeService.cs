using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RadiologyCenter.Clinic;
using RadiologyCenter.Clinic.Security;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

/// <summary>One in-process business runtime per signed-in SQL identity.</summary>
public sealed class BusinessRuntimeService(ILoggerFactory loggerFactory) : IAsyncDisposable
{
    private readonly SemaphoreSlim _lifecycle = new(1, 1);
    private ClinicRuntime? _runtime;
    private string? _userName;
    private string? _password;
    public ClinicRuntime Runtime => _runtime ?? throw new InvalidOperationException("Sign in to the clinic database first.");
    public bool IsSignedIn => _runtime is not null;
    public bool IsDevelopment => DesktopConfiguration.IsDevelopment;
    public string? PacsAuthorization { get; private set; }

    public static IConfigurationRoot Configuration(string connection, bool directAuthorization = true) =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = connection,
            ["Database:MigrateOnStartup"] = "false", ["DatabaseAuthorization:Required"] = directAuthorization.ToString(),
            ["Jwt:SecretKey"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ["Jwt:Issuer"] = "EGcare.Desktop", ["Jwt:Audience"] = "EGcare.Desktop",
            ["Jwt:AccessTokenExpirationMinutes"] = "30", ["Jwt:RefreshTokenExpirationDays"] = "1",
            ["Clinic:TimeZoneId"] = "Africa/Cairo"
        }).Build();

    public async Task EnsureDevelopmentAsync(CancellationToken ct = default)
    {
        if (!IsDevelopment || _runtime is not null) return;
        await _lifecycle.WaitAsync(ct);
        try
        {
            if (_runtime is not null) return;
            var privateConfig = new ConfigurationBuilder().AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.Development.json")).Build();
            var connection = privateConfig.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Configure the development database connection.");
            var config = new ConfigurationBuilder().AddConfiguration(privateConfig)
                .AddConfiguration(Configuration(connection, false)).Build();
            _runtime = await ClinicRuntime.StartAsync(config, "Development", ct: ct, loggerFactory: loggerFactory);
            await _runtime.CheckReadinessAsync(ct);
        }
        catch { if (_runtime is not null) await _runtime.DisposeAsync(); _runtime = null; throw; }
        finally { _lifecycle.Release(); }
    }

    public Task<TokenResult> SignInAsync(string userName, string password, CancellationToken ct = default)
        => SignInCoreAsync(userName, password, null, ct);

    public async Task<TokenResult> RestoreSessionAsync(AuthTokens saved, string? databasePassword, CancellationToken ct = default)
    {
        if (saved.RefreshTokenExpiresAt <= DateTime.UtcNow || saved.MustChangePassword)
            throw new ApiException(401, "Sign in to continue.");
        if (IsDevelopment)
        {
            await EnsureDevelopmentAsync(ct);
            // A process restart creates a new signing key. Exchange the saved refresh token
            // for a token issued by this runtime before showing any authenticated content.
            var result = await Runtime.Dispatcher.DispatchAsync(new("POST", "auth/refresh",
                System.Text.Json.JsonSerializer.Serialize(new { token = saved.RefreshToken })), ct);
            return ApiClient.Unwrap<TokenResult>(result);
        }
        if (string.IsNullOrEmpty(databasePassword) || saved.ExpiresAt <= DateTime.UtcNow)
            throw new ApiException(401, "Sign in to continue.");
        return await SignInCoreAsync(saved.Username, databasePassword,
            saved.ExpiresAt < saved.RefreshTokenExpiresAt ? saved.ExpiresAt : saved.RefreshTokenExpiresAt, ct);
    }

    private async Task<TokenResult> SignInCoreAsync(string userName, string password, DateTime? sessionExpiresAt, CancellationToken ct)
    {
        await _lifecycle.WaitAsync(ct);
        try
        {
            var profile = DesktopConfiguration.Profile ?? throw new InvalidOperationException("Choose the clinic connection first.");
            _userName=_password=PacsAuthorization=null;
            if (_runtime is not null) { await _runtime.DisposeAsync(); _runtime = null; }
            var config = Configuration(profile.ConnectionString(userName, password));
            config["Pacs:OrthancUrl"] = profile.PacsUrl;
            var candidate = await ClinicRuntime.StartAsync(config, ct: ct, loggerFactory: loggerFactory);
            try
            {
                await candidate.CheckReadinessAsync(ct);
                using var scope = candidate.Services.CreateScope();
                var principal = await scope.ServiceProvider.GetRequiredService<DatabasePrincipalReader>().ReadAsync(ct);
                if (principal.Identity?.IsAuthenticated != true) throw new ApiException(401, "This SQL account is not an active clinic user.");
                var tokens = CreateSession(principal, config, sessionExpiresAt);
                _runtime = candidate; _userName = userName; _password = password;
                return tokens;
            }
            catch { await candidate.DisposeAsync(); throw; }
        }
        catch (SqlException ex) when (ex.Number is 18456 or 18488 or 18487)
        { throw new ApiException(401, "The clinic username or password is invalid, locked or expired. Contact your administrator."); }
        catch (InvalidOperationException ex)
        { throw new ApiException(409, ex.Message); }
        finally { _lifecycle.Release(); }
    }

    public async Task<TokenResult> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken ct)
    {
        if (_userName is null || _password != currentPassword) throw new ApiException(400, "The current password is incorrect.");
        if (!RadiologyCenter.BuildingBlocks.Application.Validation.PasswordPolicyRule.IsStrongPassword(newPassword))
            throw new ApiException(400, "Use at least 12 characters including uppercase, lowercase, a number and a symbol.");
        var profile = DesktopConfiguration.Profile!;
        // The database procedure binds the account to ORIGINAL_LOGIN and cannot select another user.
        await using (var current = new SqlConnection(profile.ConnectionString(_userName,currentPassword)))
        {
            await current.OpenAsync(ct);
            await using var command=current.CreateCommand(); command.CommandText="Security.ChangeDesktopPassword";
            command.CommandType=System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@OldPassword",currentPassword); command.Parameters.AddWithValue("@NewPassword",newPassword);
            command.Parameters.AddWithValue("@PasswordHash",new Microsoft.AspNetCore.Identity.PasswordHasher<RadiologyCenter.Identity.Domain.Entities.User>().HashPassword(null!,newPassword));
            await command.ExecuteNonQueryAsync(ct);
        }
        SqlConnection.ClearAllPools();
        return await SignInAsync(_userName,newPassword,ct);
    }

    public async Task SignOutAsync()
    {
        await _lifecycle.WaitAsync();
        try
        {
            _userName = _password = PacsAuthorization = null;
            if(_runtime is not null)
            {
                var config=_runtime.Services.GetRequiredService<IConfiguration>();
                config["Pacs:UserName"]=config["Pacs:Password"]=null;
            }
            if (_runtime is not null && !IsDevelopment) { await _runtime.DisposeAsync(); _runtime = null; }
        }
        finally { _lifecycle.Release(); }
    }
    public void ConfigurePacsAccount(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) || userName.Contains(':')) throw new InvalidOperationException("Invalid image server username.");
        var config = Runtime.Services.GetRequiredService<IConfiguration>();
        config["Pacs:OrthancUrl"]=PacsService.Instance?.HttpEndpoint;
        config["Pacs:UserName"] = userName; config["Pacs:Password"] = password;
        PacsAuthorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
    }
    private static TokenResult CreateSession(ClaimsPrincipal principal, IConfiguration config, DateTime? sessionExpiresAt = null)
    {
        var expires = sessionExpiresAt ?? DateTime.UtcNow.AddHours(12);
        if (expires <= DateTime.UtcNow) throw new ApiException(401, "Your clinic session expired. Sign in again.");
        var uiClaims=principal.Claims.Select(c=>new Claim(c.Type switch
        { ClaimTypes.NameIdentifier=>"sub",ClaimTypes.Name=>"unique_name",ClaimTypes.Role=>"role",ClaimTypes.Email=>"email",_=>c.Type },c.Value));
        var jwt = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], uiClaims,
            expires: expires, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!)), SecurityAlgorithms.HmacSha512));
        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(jwt), Guid.NewGuid().ToString(), expires, expires,
            principal.HasClaim("mustChangePassword", "true"));
    }
    public async ValueTask DisposeAsync() { await SignOutAsync(); if (_runtime is not null) await _runtime.DisposeAsync(); _lifecycle.Dispose(); }
    public async Task DisposeRuntimeAsync()
    {
        await _lifecycle.WaitAsync();
        try { if(_runtime is not null) await _runtime.DisposeAsync(); _runtime=null; _userName=_password=PacsAuthorization=null; }
        finally { _lifecycle.Release(); }
    }
}
