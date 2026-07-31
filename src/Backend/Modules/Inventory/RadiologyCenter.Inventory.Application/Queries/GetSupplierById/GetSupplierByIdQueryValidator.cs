using FluentValidation;

namespace RadiologyCenter.Inventory.Application.Queries.GetSupplierById;

public class GetSupplierByIdQueryValidator : AbstractValidator<GetSupplierByIdQuery>
{
    public GetSupplierByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
