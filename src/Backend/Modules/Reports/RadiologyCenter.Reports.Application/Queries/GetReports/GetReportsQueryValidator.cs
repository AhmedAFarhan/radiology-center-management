using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Queries.GetReports;

public class GetReportsQueryValidator : AbstractValidator<GetReportsQuery>
{
    public GetReportsQueryValidator()
    {
        RuleFor(x => x.Request).NotNull().WithErrorCode(ErrorCodes.Shared.FieldRequired);
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).WithErrorCode(ErrorCodes.Shared.ValueMustBePositive).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).WithErrorCode(ErrorCodes.Shared.MustBeBetween).When(x => x.Request is not null);
    }
}