using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaries;

public class GetSalariesQueryValidator : AbstractValidator<GetSalariesQuery>
{
    public GetSalariesQueryValidator()
    {
        RuleFor(x => x.Request).NotNull().WithErrorCode(ErrorCodes.Shared.FieldRequired);
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).WithErrorCode(ErrorCodes.Shared.ValueMustBePositive).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).WithErrorCode(ErrorCodes.Shared.MustBeBetween).When(x => x.Request is not null);
    }
}