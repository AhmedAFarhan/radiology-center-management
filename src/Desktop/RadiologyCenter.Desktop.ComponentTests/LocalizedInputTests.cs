using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using RadiologyCenter.Desktop;
using RadiologyCenter.Desktop.Shared.Components;
using Xunit;

public sealed class LocalizedInputTests
{
    [Fact]
    public void BothLanguagesProvideTheSameNonEmptyValidationMessages()
    {
        using var english = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Localization", "en.json")));
        using var arabic = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Localization", "ar.json")));
        var en = english.RootElement.GetProperty("validation").EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString());
        var ar = arabic.RootElement.GetProperty("validation").EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString());
        Assert.Equal(en.Keys.Order(StringComparer.Ordinal), ar.Keys.Order(StringComparer.Ordinal));
        foreach (var pair in en.Concat(ar))
            Assert.False(string.IsNullOrWhiteSpace(pair.Value), $"Missing validation message: {pair.Key}");
    }

    [Theory]
    [InlineData("en")]
    [InlineData("ar")]
    public void AppLocalizerSupportsValidationKeysAndPreservesMudBlazorFallback(string culture)
    {
        var localizer = new AppLocalizer();
        localizer.SetCulture(culture);
        var mud = new AppMudLocalizer(localizer);
        var translated = mud["validation.examinationTypeRequired"];
        Assert.False(translated.ResourceNotFound);
        Assert.Equal(localizer["validation.examinationTypeRequired"], translated.Value);
        Assert.True(mud["Unknown_MudBlazor_Resource"].ResourceNotFound);
        Assert.True(mud["Unknown_MudBlazor_Resource", 1, 2].ResourceNotFound);
        Assert.False(mud["MudDataGridPager_InfoFormat", 1, 10, 100].ResourceNotFound);
        Assert.DoesNotContain("{0}", mud["MudDataGridPager_InfoFormat", 1, 10, 100].Value);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("ar")]
    public async Task NativeControlValidationAndEditContextBothUseTranslatedMessages(string culture)
    {
        await using var context = CreateContext(culture);
        var cut = context.Render<LocalizedInputsHarness>();
        var localizer = context.Services.GetRequiredService<AppLocalizer>();
        await cut.InvokeAsync(() => cut.Instance.Context.Validate());
        var autocomplete = cut.FindComponent<LocalizedAutocomplete<string>>();
        var text = cut.FindComponent<LocalizedTextField<string>>();
        var select = cut.FindComponent<LocalizedSelect<string>>();
        var number = cut.FindComponent<LocalizedNumericField<decimal>>();
        var date = cut.FindComponent<LocalizedDatePicker>();
        var time = cut.FindComponent<LocalizedTimePicker>();
        foreach (var validate in new Func<Task>[] { autocomplete.Instance.ValidateAsync, text.Instance.ValidateAsync,
                     select.Instance.ValidateAsync, number.Instance.ValidateAsync, date.Instance.ValidateAsync, time.Instance.ValidateAsync })
            await cut.InvokeAsync(validate);

        Assert.Contains(localizer["validation.examinationTypeRequired"], autocomplete.Instance.ValidationErrors);
        Assert.Contains(localizer["validation.clinicalIndicationRequired"], text.Instance.ValidationErrors);
        Assert.Contains(localizer["validation.genderRequired"], select.Instance.ValidationErrors);
        Assert.Contains(localizer["validation.amountGreaterThanZero"], number.Instance.ValidationErrors);
        Assert.Contains(localizer["validation.scheduledDateRequired"], date.Instance.ValidationErrors);
        Assert.Contains(localizer["validation.scheduledTimeRequired"], time.Instance.ValidationErrors);
        Assert.DoesNotContain("validation.", cut.Markup);
        Assert.All(cut.Instance.Context.GetValidationMessages(), message => Assert.DoesNotContain("validation.", message));
    }

    [Theory]
    [InlineData("en")]
    [InlineData("ar")]
    public async Task LengthErrorsAreTranslatedAndClearWhenTheUserCorrectsTheField(string culture)
    {
        await using var context = CreateContext(culture);
        var cut = context.Render<LocalizedInputsHarness>();
        var input = cut.FindComponent<LocalizedTextField<string>>();
        cut.Find("#indication").Input(new string('a', 1001));
        await cut.InvokeAsync(input.Instance.ValidateAsync);
        Assert.Contains(context.Services.GetRequiredService<AppLocalizer>()["validation.clinicalIndicationMaxLength"], input.Instance.ValidationErrors);
        Assert.DoesNotContain("validation.", cut.Markup);
        cut.Find("#indication").Input("Clinical indication");
        await cut.InvokeAsync(input.Instance.ValidateAsync);
        Assert.Empty(input.Instance.ValidationErrors);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("ar")]
    public async Task ReturningFromQuickRegistrationDoesNotKeepErrorsFromThePreviousForm(string culture)
    {
        await using var context = CreateContext(culture);
        var model = new LocalizedInputsHarness.ValidationProbe();
        var editContext = new EditContext(model);
        var cut = context.Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(p => p.Value, editContext)
            .AddChildContent<LocalizedDataAnnotationsValidator>());
        await cut.InvokeAsync(() => Assert.False(editContext.Validate()));
        Assert.NotEmpty(editContext.GetValidationMessages());

        // Quick registration temporarily removes the visit form but preserves its draft.
        cut.Render(parameters => parameters.Add(p => p.Value, editContext)
            .Add(p => p.ChildContent, (RenderFragment)(_ => { })));
        Assert.Empty(editContext.GetValidationMessages());
        model.Examination = "CT";
        model.Indication = "Clinical indication";
        model.Gender = "Male";
        model.Amount = 100;
        model.Date = DateTime.Today;
        model.Time = TimeSpan.FromHours(9);

        cut.Render(parameters => parameters.Add(p => p.Value, editContext)
            .AddChildContent<LocalizedDataAnnotationsValidator>());
        await cut.InvokeAsync(() => Assert.True(editContext.Validate()));
        Assert.Empty(editContext.GetValidationMessages());
    }

    private static BunitContext CreateContext(string culture)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddMudServices();
        var localizer = new AppLocalizer();
        localizer.SetCulture(culture);
        context.Services.AddSingleton(localizer);
        context.Services.AddSingleton<MudLocalizer, AppMudLocalizer>();
        context.Render<MudPopoverProvider>();
        return context;
    }
}
