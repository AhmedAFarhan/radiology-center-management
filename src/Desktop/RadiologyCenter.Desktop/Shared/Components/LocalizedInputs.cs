using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Shared.Components;

// MudBlazor validates annotations separately from the EditContext validator.
// Translate at that boundary so both inline errors and summaries receive text.
internal static class ValidationLocalization
{
    public static void Translate(AppLocalizer localizer, List<string> errors, int start)
    {
        for (var i = start; i < errors.Count; i++)
            errors[i] = localizer.Get(errors[i]);
    }
}

public class LocalizedAutocomplete<T> : MudAutocomplete<T>
{
    [Inject] private AppLocalizer TextLocalizer { get; set; } = default!;

    protected override void ValidateWithAttribute(ValidationAttribute attribute, T? value, List<string> errors)
    {
        var start = errors.Count;
        base.ValidateWithAttribute(attribute, value, errors);
        ValidationLocalization.Translate(TextLocalizer, errors, start);
    }
}

public class LocalizedSelect<T> : MudSelect<T>
{
    [Inject] private AppLocalizer TextLocalizer { get; set; } = default!;

    protected override void ValidateWithAttribute(ValidationAttribute attribute, T? value, List<string> errors)
    {
        var start = errors.Count;
        base.ValidateWithAttribute(attribute, value, errors);
        ValidationLocalization.Translate(TextLocalizer, errors, start);
    }
}

public class LocalizedNumericField<T> : MudNumericField<T>
{
    [Inject] private AppLocalizer TextLocalizer { get; set; } = default!;

    protected override void ValidateWithAttribute(ValidationAttribute attribute, T? value, List<string> errors)
    {
        var start = errors.Count;
        base.ValidateWithAttribute(attribute, value, errors);
        ValidationLocalization.Translate(TextLocalizer, errors, start);
    }
}

public class LocalizedDatePicker : MudDatePicker
{
    [Inject] private AppLocalizer TextLocalizer { get; set; } = default!;

    protected override void ValidateWithAttribute(ValidationAttribute attribute, DateTime? value, List<string> errors)
    {
        var start = errors.Count;
        base.ValidateWithAttribute(attribute, value, errors);
        ValidationLocalization.Translate(TextLocalizer, errors, start);
    }
}

public class LocalizedTimePicker : MudTimePicker
{
    [Inject] private AppLocalizer TextLocalizer { get; set; } = default!;

    protected override void ValidateWithAttribute(ValidationAttribute attribute, TimeSpan? value, List<string> errors)
    {
        var start = errors.Count;
        base.ValidateWithAttribute(attribute, value, errors);
        ValidationLocalization.Translate(TextLocalizer, errors, start);
    }
}
