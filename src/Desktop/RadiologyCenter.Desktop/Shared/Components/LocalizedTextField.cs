using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Shared.Components;

/// <summary>Translates MudBlazor's own annotation errors before they reach the EditContext.</summary>
public class LocalizedTextField<T> : MudTextField<T>
{
    [Inject] private AppLocalizer TextLocalizer { get; set; } = default!;

    protected override void ValidateWithAttribute(ValidationAttribute attribute, T? value, List<string> errors)
    {
        var firstNewError = errors.Count;
        base.ValidateWithAttribute(attribute, value, errors);
        ValidationLocalization.Translate(TextLocalizer, errors, firstNewError);
    }
}
