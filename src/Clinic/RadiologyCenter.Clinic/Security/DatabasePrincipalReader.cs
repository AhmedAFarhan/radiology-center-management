using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace RadiologyCenter.Clinic.Security;

/// <summary>Reads authority from SQL Server's authenticated principal, never from client supplied claims.</summary>
public sealed class DatabasePrincipalReader(IConfiguration configuration)
{
    public async Task<ClaimsPrincipal> ReadAsync(CancellationToken ct = default)
    {
        await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync(ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "Identity.ReadDesktopPrincipal";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        await using var reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return new(new ClaimsIdentity());
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, reader.GetGuid(reader.GetOrdinal("UserId")).ToString()),
            new(ClaimTypes.Name, reader.GetString(reader.GetOrdinal("UserName"))),
            new("firstName", reader.GetString(reader.GetOrdinal("FirstName"))),
            new("lastName", reader.GetString(reader.GetOrdinal("LastName"))),
            new("timezone", reader.GetString(reader.GetOrdinal("TimeZoneId"))),
            new("sqlCredentialVersion",reader.GetString(reader.GetOrdinal("CredentialVersion"))),
            new("email", reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")))
        };
        var administrator=reader.GetBoolean(reader.GetOrdinal("IsAdministrator"));
        if (administrator) claims.Add(new("isAdmin", "true"));
        if (reader.GetBoolean(reader.GetOrdinal("MustChangePassword"))) claims.Add(new("mustChangePassword", "true"));
        if (await reader.NextResultAsync(ct))
            while (await reader.ReadAsync(ct))
            {
                var code=reader.GetString(0);
                // Account/role mutations are privileged operations under direct SQL access.
                if(!administrator && (code.StartsWith("users.") || code.StartsWith("roles.")) && !code.EndsWith(".read")) continue;
                claims.Add(new("permission",code));
            }
        if (await reader.NextResultAsync(ct))
            while (await reader.ReadAsync(ct)) claims.Add(new(ClaimTypes.Role, reader.GetString(0)));
        return new(new ClaimsIdentity(claims, "SqlServer"));
    }
}
