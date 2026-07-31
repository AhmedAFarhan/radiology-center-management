using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.Inventory.Application.Commands.CreateSupplier;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x.ContactPerson).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.ContactPerson));
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Address).MaximumLength(300).When(x => !string.IsNullOrWhiteSpace(x.Address));
        RuleFor(x => x.TaxNumber).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.TaxNumber));
        RuleFor(x => x.PaymentTerms).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.PaymentTerms));
    }
}
