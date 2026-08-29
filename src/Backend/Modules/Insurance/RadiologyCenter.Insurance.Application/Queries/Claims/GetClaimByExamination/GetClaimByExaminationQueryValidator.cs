using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimByExamination;

public class GetClaimByExaminationQueryValidator : AbstractValidator<GetClaimByExaminationQuery>
{
    public GetClaimByExaminationQueryValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
    }
}
