using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFees;

public class GetReferralFeesQueryValidator : AbstractValidator<GetReferralFeesQuery>
{
    public GetReferralFeesQueryValidator()
    {
        RuleFor(x => x.Request).NotNull().WithErrorCode(ErrorCodes.RequestRequired);
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).WithErrorCode(ErrorCodes.PageNumberMustBePositive).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).WithErrorCode(ErrorCodes.PageSizeMustBeBetween).When(x => x.Request is not null);
    }
}