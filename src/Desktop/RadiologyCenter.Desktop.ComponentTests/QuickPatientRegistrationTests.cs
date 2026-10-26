using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using RadiologyCenter.Desktop.Features.Patients.Components;
using RadiologyCenter.Desktop.Features.Patients.Models;
using RadiologyCenter.Desktop.Models;
using Xunit;

public sealed class QuickPatientRegistrationTests
{
    [Theory]
    [InlineData("Ahmed Hassan", "Ahmed Hassan", "")]
    [InlineData("01012345678", "", "01012345678")]
    [InlineData("PAT-001", "", "")]
    public async Task SearchSeedsTheAppropriateField(string search, string name, string phone)
    {
        await using var context = CreateContext("en");
        var cut = context.Render<QuickPatientRegistration>(p => p.Add(x => x.SearchText, search));
        Assert.Equal(name, cut.Instance.GetDraft().FullName);
        Assert.Equal(phone, cut.Instance.GetDraft().PhoneNumber);
        Assert.Single(cut.FindAll("form"));
        Assert.Single(cut.FindAll(".quick-patient-age input:not([type=hidden])"));
    }

    [Theory]
    [InlineData("en")]
    [InlineData("ar")]
    public async Task InvalidRegistrationShowsTranslatedErrorsWithoutSaving(string culture)
    {
        await using var context = CreateContext(culture);
        var calls = 0;
        var cut = context.Render<QuickPatientRegistration>(p => p
            .Add(x => x.GenderOptions, Genders)
            .Add(x => x.CreatePatient, _ => { calls++; return Task.FromResult<PatientDto?>(Patient); }));
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Equal(0, calls);
        Assert.Contains(context.Services.GetRequiredService<AppLocalizer>()["validation.fullNameRequired"], cut.Find("[role=alert]").TextContent);
        Assert.DoesNotContain("validation.", cut.Markup);
        Assert.DoesNotContain("visitDialog.", cut.Markup);
    }

    [Fact]
    public async Task SuccessNormalizesInputAndReturnsThePatientOnlyOnce()
    {
        await using var context = CreateContext("en");
        PatientInput? saved = null;
        PatientDto? selected = null;
        var calls = 0;
        var states = new List<bool>();
        var draft = ValidDraft();
        draft.FullName = "  Ahmed   Hassan  ";
        draft.PhoneNumber = " 01012345678 ";
        var cut = context.Render<QuickPatientRegistration>(p => p
            .Add(x => x.Draft, draft).Add(x => x.GenderOptions, Genders)
            .Add(x => x.CreatePatient, input => { calls++; saved = input; return Task.FromResult<PatientDto?>(Patient); })
            .Add(x => x.Registered, patient => selected = patient)
            .Add(x => x.SavingChanged, saving => states.Add(saving)));
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Equal(1, calls);
        Assert.Same(Patient, selected);
        Assert.Equal("Ahmed Hassan", saved!.FullName);
        Assert.Equal("01012345678", saved.PhoneNumber);
        Assert.Equal(32, saved.Age);
        Assert.Null(saved.DateOfBirth);
        Assert.Equal(new[] { true, false }, states);
    }

    [Fact]
    public async Task FailedSaveKeepsValuesAndAllowsRetry()
    {
        await using var context = CreateContext("en");
        var calls = 0;
        PatientDto? selected = null;
        var cut = context.Render<QuickPatientRegistration>(p => p
            .Add(x => x.Draft, ValidDraft()).Add(x => x.GenderOptions, Genders)
            .Add(x => x.CreatePatient, _ => Task.FromResult<PatientDto?>(++calls == 1 ? null : Patient))
            .Add(x => x.Registered, patient => selected = patient));
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Null(selected);
        Assert.Contains("Patient registration failed", cut.Find("[role=alert]").TextContent);
        Assert.Equal("Ahmed Hassan", cut.Instance.GetDraft().FullName);
        Assert.Equal(32, cut.Instance.GetDraft().Age);
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Equal(2, calls);
        Assert.Same(Patient, selected);
    }

    [Fact]
    public async Task SavingPreventsDuplicateRequestsAndDisablesFields()
    {
        await using var context = CreateContext("en");
        var pending = new TaskCompletionSource<PatientDto?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        var cut = context.Render<QuickPatientRegistration>(p => p
            .Add(x => x.Draft, ValidDraft()).Add(x => x.GenderOptions, Genders)
            .Add(x => x.CreatePatient, _ => { calls++; return pending.Task; }));
        var save = cut.InvokeAsync(cut.Instance.RegisterAsync);
        cut.WaitForAssertion(() => Assert.True(cut.Find("fieldset").HasAttribute("disabled")));
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Equal(1, calls);
        pending.SetResult(Patient);
        await save;
    }

    [Fact]
    public async Task SwitchingToDateRequiresADateEvenWhenAgeWasAlreadyEntered()
    {
        await using var context = CreateContext("en");
        var calls = 0;
        var cut = context.Render<QuickPatientRegistration>(p => p
            .Add(x => x.Draft, ValidDraft()).Add(x => x.GenderOptions, Genders)
            .Add(x => x.CreatePatient, _ => { calls++; return Task.FromResult<PatientDto?>(Patient); }));
        cut.FindAll(".quick-patient-age-mode button")[1].Click();
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Equal(0, calls);
        Assert.NotEmpty(cut.FindAll("[role=alert]"));
        Assert.DoesNotContain("validation.", cut.Markup);
        cut.FindAll(".quick-patient-age-mode button")[0].Click();
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task ReturningToAnUnfinishedDraftKeepsEnteredDetails()
    {
        await using var context = CreateContext("en");
        var cut = context.Render<QuickPatientRegistration>(p => p.Add(x => x.Draft, ValidDraft()).Add(x => x.GenderOptions, Genders));
        cut.Find("input[type=tel]").Input("01112345678");
        var draft = cut.Instance.GetDraft();
        cut.Dispose();
        var reopened = context.Render<QuickPatientRegistration>(p => p.Add(x => x.Draft, draft).Add(x => x.GenderOptions, Genders));
        Assert.Equal("01112345678", reopened.Find("input[type=tel]").GetAttribute("value"));
        Assert.Equal("Ahmed Hassan", reopened.Instance.GetDraft().FullName);
        Assert.Equal(32, reopened.Instance.GetDraft().Age);
        Assert.Equal("Male", reopened.Instance.GetDraft().Gender);
    }

    [Fact]
    public async Task DateOfBirthIsSubmittedWithoutAnAge()
    {
        await using var context = CreateContext("en");
        var draft = ValidDraft();
        draft.DateOfBirth = new DateTime(1994, 6, 15);
        PatientInput? saved = null;
        var cut = context.Render<QuickPatientRegistration>(p => p
            .Add(x => x.Draft, draft).Add(x => x.GenderOptions, Genders)
            .Add(x => x.CreatePatient, input => { saved = input; return Task.FromResult<PatientDto?>(Patient); }));
        await cut.InvokeAsync(cut.Instance.RegisterAsync);
        Assert.Equal(draft.DateOfBirth, saved!.DateOfBirth);
        Assert.Null(saved.Age);
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

    private static PatientInput ValidDraft() => new() { FullName = "Ahmed Hassan", PhoneNumber = "01012345678", Gender = "Male", Age = 32 };
    private static readonly EnumOptionDto[] Genders = [new("Male", "Male"), new("Female", "Female")];
    private static readonly PatientDto Patient = new("test-patient", "TEST-001", "Ahmed", null, "Hassan", "Ahmed Hassan", null, 32, "Male", "01012345678", null, null, null, null, null, null, true, DateTime.UtcNow, "Male");
}
