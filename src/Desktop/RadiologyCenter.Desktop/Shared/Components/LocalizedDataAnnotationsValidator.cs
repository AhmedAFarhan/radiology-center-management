using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Shared.Components;

public sealed class LocalizedDataAnnotationsValidator : ComponentBase, IDisposable
{
    private ValidationMessageStore? _messages;
    private bool _disposed;

    [Inject] private AppLocalizer T { get; set; } = default!;

    [CascadingParameter] private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
            throw new InvalidOperationException("LocalizedDataAnnotationsValidator requires a cascading EditContext.");

        _messages = new ValidationMessageStore(CurrentEditContext);
        CurrentEditContext.OnValidationRequested += ValidateModel;
        CurrentEditContext.OnFieldChanged += ValidateField;
    }

    private void ValidateModel(object? sender, ValidationRequestedEventArgs e)
    {
        var context = CurrentEditContext!;
        var results = new List<ValidationResult>();
        ValidateModel(context.Model, results, context, validateAllProperties: true);
        var messages = _messages!;
        messages.Clear();
        foreach (var result in results)
        {
            var memberNames = result.MemberNames.Any() ? result.MemberNames.ToArray() : new[] { string.Empty };
            foreach (var memberName in memberNames)
                messages.Add(context.Field(memberName), Translate(result.ErrorMessage));
        }
        context.NotifyValidationStateChanged();
    }

    private void ValidateField(object? sender, FieldChangedEventArgs e)
    {
        var context = CurrentEditContext!;
        var results = new List<ValidationResult>();
        ValidateField(context.Model, e.FieldIdentifier, results);
        var messages = _messages!;
        messages.Clear(e.FieldIdentifier);
        foreach (var result in results)
        {
            var memberNames = result.MemberNames.Any() ? result.MemberNames.ToArray() : new[] { string.Empty };
            foreach (var memberName in memberNames)
                messages.Add(context.Field(memberName), Translate(result.ErrorMessage));
        }
        context.NotifyValidationStateChanged();
    }

    private string Translate(string? message)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;
        return T[message];
    }

    private static void ValidateModel(object model, List<ValidationResult> results, EditContext editContext, bool validateAllProperties)
    {
        var validationContext = new ValidationContext(model);
        validationContext.MemberName = null;
        Validator.TryValidateObject(model, validationContext, results, validateAllProperties);
    }

    private static void ValidateField(object model, in FieldIdentifier fieldIdentifier, List<ValidationResult> results)
    {
        var propertyInfo = fieldIdentifier.Model.GetType().GetProperty(fieldIdentifier.FieldName);
        if (propertyInfo is not null)
        {
            var validationContext = new ValidationContext(fieldIdentifier.Model) { MemberName = propertyInfo.Name };
            var fieldResults = new List<ValidationResult>();
            Validator.TryValidateProperty(propertyInfo.GetValue(fieldIdentifier.Model), validationContext, fieldResults);
            results.AddRange(fieldResults);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (CurrentEditContext is not null)
        {
            CurrentEditContext.OnValidationRequested -= ValidateModel;
            CurrentEditContext.OnFieldChanged -= ValidateField;
        }
    }
}
