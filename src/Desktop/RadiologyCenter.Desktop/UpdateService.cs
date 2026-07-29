using Velopack;
using Velopack.Sources;

namespace RadiologyCenter.Desktop;

public sealed class UpdateService
{
    private const string GithubRepo = "https://github.com/AhmedAFarhan/radiology-center-management";
    private UpdateManager? _mgr;

    public void Init()
    {
        VelopackApp.Build()
            .SetAutoApplyOnStartup(true)
            .Run();
    }

    public UpdateManager GetManager()
    {
        _mgr ??= new UpdateManager(new GithubSource(GithubRepo, null, false));
        return _mgr;
    }

    public async Task<UpdateInfo?> CheckForUpdatesAsync()
    {
        try
        {
            return await GetManager().CheckForUpdatesAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DownloadUpdateAsync(UpdateInfo update)
    {
        try
        {
            await GetManager().DownloadUpdatesAsync(update);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void ApplyAndRestart(UpdateInfo update)
    {
        GetManager().ApplyUpdatesAndRestart(update);
    }
}
