using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Phone).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.ContactPerson).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.ContactPerson));
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(SharedCodes.Shared.InvalidEmail).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Address).MaximumLength(300).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Address));
        RuleFor(x => x.TaxNumber).MaximumLength(50).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.TaxNumber));
        RuleFor(x => x.PaymentTerms).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.PaymentTerms));
    }
}
