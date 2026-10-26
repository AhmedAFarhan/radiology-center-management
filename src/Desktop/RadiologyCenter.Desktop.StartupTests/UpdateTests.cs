using RadiologyCenter.Desktop.Services.Updates;
using Xunit;

namespace RadiologyCenter.Desktop.StartupTests;

public sealed class UpdateTests
{
    private static readonly UpdateFeed Web = new("https://github.com/clinic/app/releases/latest/download", false);

    [Fact]
    public void GitHubUpdatesWorkBeforeClinicSetup()
    {
        Assert.Equal(Web, ReleaseInfo.ReadFeed(Path.Combine(Path.GetTempPath(), Guid.NewGuid()+".json"), Web.Location));
    }

    [Fact]
    public void ExplicitShareWinsEvenWhenTheDatabaseProfileIsIncomplete()
    {
        var path = Path.GetTempFileName();
        try
        {
            var directory = Path.Combine(Path.GetTempPath(), "missing-clinic-share");
            File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(new { Server = "", UpdatesDirectory = directory }));
            Assert.Equal(new UpdateFeed(directory, true), ReleaseInfo.ReadFeed(path, Web.Location));
            File.WriteAllText(path, "{\"UpdatesDirectory\":\"relative/share\"}");
            Assert.Equal(UpdateFailureReason.Configuration, Assert.Throws<UpdateFailure>(() => ReleaseInfo.ReadFeed(path, Web.Location)).Reason);
        }
        finally { File.Delete(path); }
    }

    [Theory]
    [InlineData("invalid json")]
    [InlineData("{\"UpdatesDirectory\":42}")]
    public void UnreadableProfileNeverSilentlySwitchesToGitHub(string json)
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, json);
            Assert.Equal(UpdateFailureReason.Configuration, Assert.Throws<UpdateFailure>(() => ReleaseInfo.ReadFeed(path, Web.Location)).Reason);
        }
        finally { File.Delete(path); }
    }

    [Theory]
    [InlineData("http://updates.test/feed")]
    [InlineData("https://user:secret@updates.test/feed")]
    [InlineData("https://updates.test/feed?token=secret")]
    public void UnsafeWebFeedsAreRejected(string feed)
    {
        Assert.Throws<UpdateFailure>(() => ReleaseInfo.ReadFeed(Path.Combine(Path.GetTempPath(), Guid.NewGuid()+".json"), feed));
    }

    [Fact]
    public async Task DevelopmentBuildDoesNotContactTheFeed()
    {
        var client = new FakeClient { IsInstalled = false };
        var service = new AppUpdateService(() => Web, _ => client, () => Task.CompletedTask);
        Assert.Equal(UpdateFailureReason.NotInstalled, (await Assert.ThrowsAsync<UpdateFailure>(() => service.CheckAsync())).Reason);
        Assert.Equal(0, client.Checks);
    }

    [Fact]
    public async Task RestartRequiresDownloadAndDrainsTheBusinessRuntimeFirst()
    {
        var client = new FakeClient();
        var service = new AppUpdateService(() => Web, _ => client, () => { client.Events.Add("drain"); return Task.CompletedTask; });
        var offer = (await service.CheckAsync())!;
        Assert.Equal(UpdateFailureReason.NotDownloaded, (await Assert.ThrowsAsync<UpdateFailure>(() => service.RestartAsync(offer))).Reason);
        await service.DownloadAsync(offer, null);
        await service.RestartAsync(offer);
        Assert.Equal(new[] { "download", "drain", "restart" }, client.Events);
    }

    [Fact]
    public async Task RecheckingTheSameVersionPreservesTheDownloadedOffer()
    {
        var client = new FakeClient();
        var service = new AppUpdateService(() => Web, _ => client, () => Task.CompletedTask);
        var offer = (await service.CheckAsync())!;
        await service.DownloadAsync(offer, null);
        Assert.Same(offer, await service.CheckAsync());
        Assert.True(service.IsDownloaded(offer));
    }

    [Fact]
    public async Task ChangingTheFeedPreventsInstallingAnOldOffer()
    {
        var feed = Web; var client = new FakeClient(); var drained = false;
        var service = new AppUpdateService(() => feed, _ => client, () => { drained = true; return Task.CompletedTask; });
        var offer = (await service.CheckAsync())!;
        await service.DownloadAsync(offer, null);
        feed = new UpdateFeed(Path.GetTempPath(), true);
        Assert.Equal(UpdateFailureReason.SourceChanged, (await Assert.ThrowsAsync<UpdateFailure>(() => service.RestartAsync(offer))).Reason);
        Assert.False(drained);
        Assert.DoesNotContain("restart", client.Events);
    }

    [Fact]
    public async Task CancelledDownloadCannotBeInstalledAndCanBeRetried()
    {
        var client = new FakeClient { FailDownload = true };
        var service = new AppUpdateService(() => Web, _ => client, () => Task.CompletedTask);
        var offer = (await service.CheckAsync())!;
        await Assert.ThrowsAsync<OperationCanceledException>(() => service.DownloadAsync(offer, null));
        Assert.False(service.IsDownloaded(offer));
        client.FailDownload = false;
        await service.DownloadAsync(offer, null);
        Assert.True(service.IsDownloaded(offer));
    }

    [Fact]
    public async Task NewerOfferInvalidatesAnEarlierDownload()
    {
        var client = new FakeClient();
        var service = new AppUpdateService(() => Web, _ => client, () => Task.CompletedTask);
        var old = (await service.CheckAsync())!;
        await service.DownloadAsync(old, null);
        client.Version = "1.2.0";
        var current = (await service.CheckAsync())!;
        Assert.Equal("1.2.0", current.Version);
        Assert.False(service.IsDownloaded(old));
        Assert.Equal(UpdateFailureReason.SourceChanged, (await Assert.ThrowsAsync<UpdateFailure>(() => service.RestartAsync(old))).Reason);
    }

    [Fact]
    public async Task FailedRuntimeDrainNeverStartsTheInstaller()
    {
        var client = new FakeClient();
        var service = new AppUpdateService(() => Web, _ => client, () => throw new IOException("busy runtime"));
        var offer = (await service.CheckAsync())!;
        await service.DownloadAsync(offer, null);
        await Assert.ThrowsAsync<IOException>(() => service.RestartAsync(offer));
        Assert.DoesNotContain("restart", client.Events);
    }

    private sealed class FakeClient : IUpdateClient
    {
        public bool IsInstalled { get; set; } = true;
        public string CurrentVersion => "1.0.0";
        public string Version { get; set; } = "1.1.0";
        public bool FailDownload { get; set; }
        public int Checks { get; private set; }
        public List<string> Events { get; } = [];
        public Task<string?> CheckAsync(CancellationToken cancellationToken) { Checks++; return Task.FromResult<string?>(Version); }
        public Task DownloadAsync(string version, Action<int>? progress, CancellationToken cancellationToken)
        {
            if (FailDownload) throw new OperationCanceledException();
            Events.Add("download"); progress?.Invoke(100); return Task.CompletedTask;
        }
        public void ApplyAndRestart(string version) => Events.Add("restart");
    }
}
