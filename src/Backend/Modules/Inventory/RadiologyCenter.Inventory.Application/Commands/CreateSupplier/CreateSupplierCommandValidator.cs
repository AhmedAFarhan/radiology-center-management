using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Inventory.Application.Commands.CreateSupplier;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Phone).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.ContactPerson).MaximumLength(100).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.ContactPerson));
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(ErrorCodes.Shared.InvalidEmail).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Address).MaximumLength(300).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Address));
        RuleFor(x => x.TaxNumber).MaximumLength(50).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.TaxNumber));
        RuleFor(x => x.PaymentTerms).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.PaymentTerms));
    }
}
