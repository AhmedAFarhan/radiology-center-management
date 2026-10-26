using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using RadiologyCenter.Desktop.Localization;
using RadiologyCenter.Desktop.Services.Updates;
using RadiologyCenter.Desktop.Shared.Components;
using Xunit;

public sealed class UpdateDialogTests
{
    [Theory]
    [InlineData("en")]
    [InlineData("ar")]
    public async Task ShowsTheReleaseVersionWithStableLeftToRightNumbers(string culture)
    {
        await using var context = Context(culture, new FakeUpdates());
        var view = context.Render<ReleaseVersion>();
        Assert.Equal("9.8.7", view.Find("bdi[dir=ltr]").TextContent);
        Assert.Contains(culture == "ar" ? "الإصدار" : "Version", view.Markup);
    }

    [Fact]
    public async Task DownloadNeverRestartsUntilTheUserChoosesInstall()
    {
        var updates = new FakeUpdates();
        await using var context = Context("en", updates);
        var view = context.Render<MudDialogProvider>();
        await view.InvokeAsync(() => context.Services.GetRequiredService<IDialogService>().ShowAsync<ClinicUpdateDialog>("EGcare"));
        view.WaitForAssertion(() => Assert.Contains("1.2.0", view.Markup));
        await view.FindAll("button").Single(x => x.TextContent.Contains("Download update")).ClickAsync(new());
        Assert.Equal(1, updates.Downloads);
        Assert.Equal(0, updates.Restarts);
        await view.FindAll("button").Single(x => x.TextContent.Contains("Install and restart")).ClickAsync(new());
        Assert.Equal(1, updates.Restarts);
    }

    [Theory]
    [InlineData("en", "Install EGcare")]
    [InlineData("ar", "ثبّت EGcare")]
    public async Task ErrorsUseReadableLocalizedText(string culture, string expected)
    {
        await using var context = Context(culture, new FakeUpdates { Unsupported = true });
        var view = context.Render<MudDialogProvider>();
        await view.InvokeAsync(() => context.Services.GetRequiredService<IDialogService>().ShowAsync<ClinicUpdateDialog>("EGcare"));
        view.WaitForAssertion(() => Assert.Contains(expected, view.Markup));
        Assert.DoesNotContain("NotInstalled", view.Markup);
        Assert.DoesNotContain("SourceChanged", view.Markup);
    }

    private static BunitContext Context(string culture, FakeUpdates updates)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddMudServices();
        var localizer = new AppLocalizer(); localizer.SetCulture(culture);
        context.Services.AddSingleton(localizer);
        context.Services.AddSingleton<IAppUpdates>(updates);
        context.Render<MudPopoverProvider>();
        return context;
    }

    private sealed class FakeUpdates : IAppUpdates
    {
        public bool Unsupported { get; init; }
        public int Downloads { get; private set; }
        public int Restarts { get; private set; }
        public UpdateOffer? Available { get; } = new("1.1.0", "1.2.0");
        public event Action? Changed { add { } remove { } }
        public Task<UpdateOffer?> CheckAsync(CancellationToken cancellationToken = default)
            => Unsupported ? throw new UpdateFailure(UpdateFailureReason.NotInstalled) : Task.FromResult(Available);
        public Task DownloadAsync(UpdateOffer offer, Action<int>? progress, CancellationToken cancellationToken = default)
        { Downloads++; progress?.Invoke(100); return Task.CompletedTask; }
        public bool IsDownloaded(UpdateOffer offer) => Downloads > 0;
        public Task RestartAsync(UpdateOffer offer, CancellationToken cancellationToken = default)
        { Restarts++; return Task.CompletedTask; }
    }
}
