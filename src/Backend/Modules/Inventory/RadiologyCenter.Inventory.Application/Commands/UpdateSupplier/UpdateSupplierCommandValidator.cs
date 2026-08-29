using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty().WithErrorCode(ErrorCodes.SupplierIdRequired);
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.SupplierNameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.SupplierNameTooLong);
        RuleFor(x => x.Phone).NotEmpty().WithErrorCode(ErrorCodes.SupplierPhoneRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(ErrorCodes.SupplierPhoneTooLong);
        RuleFor(x => x.ContactPerson).MaximumLength(100).WithErrorCode(ErrorCodes.SupplierContactPersonTooLong).When(x => !string.IsNullOrWhiteSpace(x.ContactPerson));
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(ErrorCodes.SupplierEmailInvalid).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Address).MaximumLength(300).WithErrorCode(ErrorCodes.SupplierAddressTooLong).When(x => !string.IsNullOrWhiteSpace(x.Address));
        RuleFor(x => x.TaxNumber).MaximumLength(50).WithErrorCode(ErrorCodes.SupplierTaxNumberTooLong).When(x => !string.IsNullOrWhiteSpace(x.TaxNumber));
        RuleFor(x => x.PaymentTerms).MaximumLength(200).WithErrorCode(ErrorCodes.SupplierPaymentTermsTooLong).When(x => !string.IsNullOrWhiteSpace(x.PaymentTerms));
    }
}
