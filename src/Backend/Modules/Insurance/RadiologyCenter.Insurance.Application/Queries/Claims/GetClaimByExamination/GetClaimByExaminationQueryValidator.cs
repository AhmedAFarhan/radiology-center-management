using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimByExamination;

public class GetClaimByExaminationQueryValidator : AbstractValidator<GetClaimByExaminationQuery>
{
    public GetClaimByExaminationQueryValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}