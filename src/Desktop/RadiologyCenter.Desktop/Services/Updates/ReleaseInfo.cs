using System.Reflection;
using System.Text.Json;

namespace RadiologyCenter.Desktop.Services.Updates;

public static class ReleaseInfo
{
    private static string? Metadata(string name) => typeof(ReleaseInfo).Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == name)?.Value;
    public static string Version => Metadata("ReleaseVersion") ?? "1.0.0";
    public static string FeedUrl => Metadata("UpdateFeedUrl") ?? "";

    public static UpdateFeed ReadFeed(string profilePath, string defaultFeedUrl)
    {
        // Update access must work even when the SQL connection or schema needs repair.
        // An explicit clinic share wins; an unavailable share never falls back to the internet.
        string? directory = null;
        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(profilePath));
            foreach (var property in document.RootElement.EnumerateObject())
                if (property.Name.Equals("UpdatesDirectory", StringComparison.OrdinalIgnoreCase))
                    directory = property.Value.ValueKind == JsonValueKind.Null ? null : property.Value.GetString();
        }
        catch (FileNotFoundException) { }
        catch (DirectoryNotFoundException) { }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException or InvalidOperationException)
        { throw new UpdateFailure(UpdateFailureReason.Configuration); }
        if (!string.IsNullOrWhiteSpace(directory))
        {
            if (!Path.IsPathFullyQualified(directory)) throw new UpdateFailure(UpdateFailureReason.Configuration);
            return new UpdateFeed(Path.GetFullPath(directory), true);
        }
        if (!Uri.TryCreate(defaultFeedUrl, UriKind.Absolute, out var uri) || uri.Scheme != "https"
            || uri.UserInfo.Length != 0 || uri.Query.Length != 0 || uri.Fragment.Length != 0)
            throw new UpdateFailure(UpdateFailureReason.Configuration);
        return new UpdateFeed(uri.AbsoluteUri.TrimEnd('/'), false);
    }
}

public sealed record UpdateFeed(string Location, bool IsDirectory);
public sealed record UpdateOffer(string CurrentVersion, string Version);
public enum UpdateFailureReason { Configuration, NotInstalled, SourceChanged, NotDownloaded }
public sealed class UpdateFailure(UpdateFailureReason reason) : InvalidOperationException(reason.ToString())
{
    public UpdateFailureReason Reason { get; } = reason;
}

public interface IAppUpdates
{
    UpdateOffer? Available { get; }
    event Action? Changed;
    Task<UpdateOffer?> CheckAsync(CancellationToken cancellationToken = default);
    Task DownloadAsync(UpdateOffer offer, Action<int>? progress, CancellationToken cancellationToken = default);
    bool IsDownloaded(UpdateOffer offer);
    Task RestartAsync(UpdateOffer offer, CancellationToken cancellationToken = default);
}

public interface IUpdateClient
{
    bool IsInstalled { get; }
    string CurrentVersion { get; }
    Task<string?> CheckAsync(CancellationToken cancellationToken);
    Task DownloadAsync(string version, Action<int>? progress, CancellationToken cancellationToken);
    void ApplyAndRestart(string version);
}
