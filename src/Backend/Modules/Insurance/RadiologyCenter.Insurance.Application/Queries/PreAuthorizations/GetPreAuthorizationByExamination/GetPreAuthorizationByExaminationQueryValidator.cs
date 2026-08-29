using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationByExamination;

public class GetPreAuthorizationByExaminationQueryValidator : AbstractValidator<GetPreAuthorizationByExaminationQuery>
{
    public GetPreAuthorizationByExaminationQueryValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
    }
}
