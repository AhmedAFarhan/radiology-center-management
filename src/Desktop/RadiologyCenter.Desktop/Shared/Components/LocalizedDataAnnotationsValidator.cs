using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Shared.Components;

public sealed class LocalizedDataAnnotationsValidator : ComponentBase, IDisposable
{
    private static readonly Dictionary<string, string> TranslationMap = new()
    {
        ["Address must be 300 characters or fewer."] = "validation.addressMaxLength",
        ["Allowance name is required."] = "validation.allowanceNameRequired",
        ["Allowance name must be 100 characters or fewer."] = "validation.allowanceNameMaxLength",
        ["Amount must be greater than zero."] = "validation.amountGreaterThanZero",
        ["Billed amount must be zero or greater."] = "validation.billedAmountNonNegative",
        ["Body is required."] = "validation.bodyRequired",
        ["Body part is required."] = "validation.bodyPartRequired",
        ["Body part must be 200 characters or fewer."] = "validation.bodyPartMaxLength",
        ["Category is required."] = "validation.categoryRequired",
        ["Channel is required."] = "validation.channelRequired",
        ["Clinical indication is required."] = "validation.clinicalIndicationRequired",
        ["Clinical indication must be 1000 characters or fewer."] = "validation.clinicalIndicationMaxLength",
        ["Code is required."] = "validation.codeRequired",
        ["Code must be 100 characters or fewer."] = "validation.codeMaxLength100",
        ["Code must be 20 characters or fewer."] = "validation.codeMaxLength20",
        ["Contact person must be 100 characters or fewer."] = "validation.contactPersonMaxLength",
        ["Counted total must be a valid amount."] = "validation.countedTotalInvalid",
        ["Coverage must be between 0 and 100."] = "validation.coverageRange",
        ["Current password is required."] = "validation.currentPasswordRequired",
        ["Department must be 200 characters or fewer."] = "validation.departmentMaxLength",
        ["Description must be 500 characters or fewer."] = "validation.descriptionMaxLength",
        ["Effective From is required."] = "validation.effectiveFromRequired",
        ["Email is required."] = "validation.emailRequired",
        ["Email must be 200 characters or fewer."] = "validation.emailMaxLength",
        ["Enter a valid email address."] = "validation.emailInvalid",
        ["Estimated amount must be zero or greater."] = "validation.estimatedAmountNonNegative",
        ["First name is required."] = "validation.firstNameRequired",
        ["First name must be 200 characters or fewer."] = "validation.firstNameMaxLength",
        ["Full name is required."] = "validation.fullNameRequired",
        ["Full name must be 300 characters or fewer."] = "validation.fullNameMaxLength",
        ["Full name must include at least a first and last name."] = "validation.fullNameFirstAndLast",
        ["Gender is required."] = "validation.genderRequired",
        ["Hire date is required."] = "validation.hireDateRequired",
        ["Hospital must be 200 characters or fewer."] = "validation.hospitalMaxLength",
        ["Insurance company is required."] = "validation.insuranceCompanyRequired",
        ["Kind is required."] = "validation.kindRequired",
        ["Last name is required."] = "validation.lastNameRequired",
        ["Last name must be 200 characters or fewer."] = "validation.lastNameMaxLength",
        ["Leave type is required."] = "validation.leaveTypeRequired",
        ["License number must be 100 characters or fewer."] = "validation.licenseNumberMaxLength",
        ["Lock until date is required."] = "validation.lockUntilDateRequired",
        ["Lock until time is required."] = "validation.lockUntilTimeRequired",
        ["Modality is required."] = "validation.modalityRequired",
        ["Name is required."] = "validation.nameRequired",
        ["Name must be 100 characters or fewer."] = "validation.nameMaxLength100",
        ["Name must be 200 characters or fewer."] = "validation.nameMaxLength200",
        ["Notes must be 1000 characters or fewer."] = "validation.notesMaxLength1000",
        ["Notes must be 500 characters or fewer."] = "validation.notesMaxLength500",
        ["Opening float must be a valid amount."] = "validation.openingFloatInvalid",
        ["Password is required."] = "validation.passwordRequired",
        ["Password must be at least 8 characters."] = "validation.passwordMinLength",
        ["Password must contain both letters and digits."] = "validation.passwordLettersDigits",
        ["Passwords do not match."] = "validation.passwordMismatch",
        ["Patient is required."] = "validation.patientRequired",
        ["Payment terms must be 200 characters or fewer."] = "validation.paymentTermsMaxLength",
        ["Phone is required."] = "validation.phoneRequired",
        ["Phone must be 30 characters or fewer."] = "validation.phoneMaxLength",
        ["Phone number is required."] = "validation.phoneNumberRequired",
        ["Phone number must be 30 characters or fewer."] = "validation.phoneNumberMaxLength",
        ["Phone number must be a valid Egyptian number (e.g. 01012345678)."] = "validation.phoneEgyptian",
        ["Policy number is required."] = "validation.policyNumberRequired",
        ["Policy number must be 100 characters or fewer."] = "validation.policyNumberMaxLength",
        ["Position is required."] = "validation.positionRequired",
        ["Quantity must be at least 1."] = "validation.quantityMinOne",
        ["Radiologist is required."] = "validation.radiologistRequired",
        ["Reason must be 500 characters or fewer."] = "validation.reasonMaxLength",
        ["Receiving opening float must be a valid amount."] = "validation.receivingOpeningFloatInvalid",
        ["Receiving user ID must be 100 characters or fewer."] = "validation.receivingUserIdMaxLength",
        ["Recipient is required."] = "validation.recipientRequired",
        ["Recipient must be 500 characters or fewer."] = "validation.recipientMaxLength",
        ["Reference must be 100 characters or fewer."] = "validation.referenceMaxLength",
        ["Reorder level cannot be negative."] = "validation.reorderLevelNonNegative",
        ["Reorder quantity cannot be negative."] = "validation.reorderQuantityNonNegative",
        ["Role is required."] = "validation.roleRequired",
        ["Role name is required."] = "validation.roleNameRequired",
        ["Role name must be 256 characters or fewer."] = "validation.roleNameMaxLength",
        ["Scheduled date is required."] = "validation.scheduledDateRequired",
        ["Scheduled time is required."] = "validation.scheduledTimeRequired",
        ["Salary type is required."] = "validation.salaryTypeRequired",
        ["Serial number must be 100 characters or fewer."] = "validation.serialNumberMaxLength",
        ["Specialization must be 200 characters or fewer."] = "validation.specializationMaxLength",
        ["Status is required."] = "validation.statusRequired",
        ["Subject must be 400 characters or fewer."] = "validation.subjectMaxLength",
        ["Supplier is required."] = "validation.supplierRequired",
        ["Tax number must be 50 characters or fewer."] = "validation.taxNumberMaxLength",
        ["Technician is required."] = "validation.technicianRequired",
        ["Unit is required."] = "validation.unitRequired",
        ["Username is required."] = "validation.usernameRequired",
        ["Username must be 256 characters or fewer."] = "validation.usernameMaxLength",
    };

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
        if (message is not null && TranslationMap.TryGetValue(message, out var key))
            return T[key];
        return message ?? string.Empty;
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
