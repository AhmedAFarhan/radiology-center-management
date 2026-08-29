using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminations;

public class GetExaminationsQueryValidator : AbstractValidator<GetExaminationsQuery>
{
    public GetExaminationsQueryValidator()
    {
        RuleFor(x => x.Request).NotNull().WithErrorCode(ErrorCodes.RequestRequired);
        RuleFor(x => x.Request.Pagination.PageNumber).GreaterThan(0).WithErrorCode(ErrorCodes.PageNumberMustBePositive).When(x => x.Request is not null);
        RuleFor(x => x.Request.Pagination.PageSize).InclusiveBetween(1, 100).WithErrorCode(ErrorCodes.PageSizeMustBeBetween).When(x => x.Request is not null);
    }
}
