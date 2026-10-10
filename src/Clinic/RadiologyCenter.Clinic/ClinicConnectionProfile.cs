using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace RadiologyCenter.Clinic;

/// <summary>Portable clinic addresses. This document never contains credentials.</summary>
public sealed record ClinicConnectionProfile
{
    public string Server { get; init; } = "";
    public string Database { get; init; } = "";
    public string? PacsUrl { get; init; }
    public string? UpdatesDirectory { get; init; }
    public string LoginPrefix => "egc_" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Database.ToUpperInvariant()))).ToLowerInvariant()[..10] + "_";
    public void Validate()
    {
        if(string.IsNullOrWhiteSpace(Server)) throw new InvalidOperationException("Enter the admin PC SQL Server address.");
        var host=Server.Split(',')[0].Split('\\')[0].Replace("tcp:","",StringComparison.OrdinalIgnoreCase).Trim();
        if (!System.Text.RegularExpressions.Regex.IsMatch(Server, @"^(?:tcp:)?[a-zA-Z0-9][a-zA-Z0-9.-]*(?:\\[a-zA-Z0-9_-]+)?(?:,[0-9]{1,5})?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            || (Server.Contains(',') && (!int.TryParse(Server.Split(',')[^1], out var port) || port is < 1 or > 65535)))
            throw new InvalidOperationException("Use a SQL Server network name or IPv4 address, with an optional instance or TCP port.");
        if(host.ToLowerInvariant() is "." or "(local)" or "(localdb)" or "localhost" or "127.0.0.1" or "::1")
            throw new InvalidOperationException("Use the admin PC's network name or IP so every workstation reaches the same database.");
        if (string.IsNullOrWhiteSpace(Server) || Server.Length > 200 || Server.Contains(';')
            || string.IsNullOrWhiteSpace(Database) || Database.Length > 100
            || Database.Any(c => !(char.IsAsciiLetterOrDigit(c) || c == '_')))
            throw new InvalidOperationException("Enter the SQL Server address and a database name containing letters, numbers or underscores.");
        if (!string.IsNullOrWhiteSpace(PacsUrl) && (!Uri.TryCreate(PacsUrl, UriKind.Absolute, out var uri)
            || uri.Scheme != "https" || uri.IsLoopback || !string.IsNullOrEmpty(uri.UserInfo) || uri.AbsolutePath != "/"
            || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment)))
            throw new InvalidOperationException("The image server address must be an HTTPS origin.");
        if (!string.IsNullOrWhiteSpace(UpdatesDirectory) && !Path.IsPathFullyQualified(UpdatesDirectory))
            throw new InvalidOperationException("The update folder must be an absolute local path or a clinic network share.");
    }
    public string ConnectionString(string? userName = null, string? password = null, bool maintenance = false)
    {
        Validate();
        var c = new SqlConnectionStringBuilder { DataSource = Server, InitialCatalog = Database,
            IntegratedSecurity = maintenance, Encrypt = SqlConnectionEncryptOption.Mandatory,
            TrustServerCertificate = false, ConnectTimeout = 10, ApplicationName = "EGcare", PersistSecurityInfo = false, MultipleActiveResultSets = true };
        if (!c.IntegratedSecurity)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length > 80
                || userName.Any(ch => !(char.IsAsciiLetterOrDigit(ch) || ch is '_' or '-' or '.')))
                throw new InvalidOperationException("Enter a valid clinic username.");
            c.UserID = LoginPrefix + userName; c.Password = password ?? "";
        }
        return c.ConnectionString;
    }
    public static ClinicConnectionProfile Read(string path)
    {
        var profile = JsonSerializer.Deserialize<ClinicConnectionProfile>(File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidOperationException("Invalid clinic connection file.");
        profile.Validate(); return profile;
    }
    public void Write(string path)
    {
        Validate(); Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporary, path, overwrite: true);
    }
}
