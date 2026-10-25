using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using RadiologyCenter.Desktop.Features.ReadingRoom.Components;
using RadiologyCenter.Desktop.Localization;
using RadiologyCenter.Desktop.Models;
using Xunit;

public sealed class ReadingRoomFindingsListTests
{
    [Theory]
    [InlineData("en", true)]
    [InlineData("en", false)]
    [InlineData("ar", true)]
    [InlineData("ar", false)]
    public async Task FinalizedReportKeepsFindingsVisibleAndHidesEditingControls(string culture, bool hasFindings)
    {
        await using var context = CreateContext(culture);
        var cut = context.Render<ReadingRoomFindingsList>(parameters => parameters
            .Add(p => p.CanEdit, false)
            .Add(p => p.Findings, hasFindings ? Findings : [])
            .Add(p => p.SeverityOptions, Severities));

        Assert.Empty(cut.FindAll("input,textarea"));
        Assert.Empty(cut.FindAll("button"));
        if (hasFindings)
        {
            Assert.Contains("Training finding", cut.Markup);
            Assert.Contains("Mild", cut.Find(".rr-finding-severity").TextContent);
        }
        else Assert.Contains(context.Services.GetRequiredService<AppLocalizer>().ReadingRoom.NoFindings, cut.Markup);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("ar")]
    public async Task FinalizingADraftRerendersTheSameComponentWithoutLosingFindings(string culture)
    {
        await using var context = CreateContext(culture);
        var cut = context.Render<ReadingRoomFindingsList>(parameters => parameters
            .Add(p => p.CanEdit, true)
            .Add(p => p.Findings, Findings)
            .Add(p => p.SeverityOptions, Severities));
        Assert.NotEmpty(cut.FindAll("input"));

        cut.Render(parameters => parameters.Add(p => p.CanEdit, false));

        Assert.Contains("Training finding", cut.Markup);
        Assert.Single(cut.FindAll(".rr-finding-severity"));
        Assert.Empty(cut.FindAll("input,textarea,button"));
    }

    private static BunitContext CreateContext(string culture)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddMudServices();
        var localizer = new AppLocalizer();
        localizer.SetCulture(culture);
        context.Services.AddSingleton(localizer);
        context.Render<MudPopoverProvider>();
        return context;
    }

    private static readonly ReportFindingDto[] Findings = [new("finding-id", "Training region", "Training finding", "Mild", 0, "Mild")];
    private static readonly EnumOptionDto[] Severities = [new("None", "None"), new("Mild", "Mild")];
}
