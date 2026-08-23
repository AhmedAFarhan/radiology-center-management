using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationByExamination;

public class GetPreAuthorizationByExaminationQueryValidator : AbstractValidator<GetPreAuthorizationByExaminationQuery>
{
    public GetPreAuthorizationByExaminationQueryValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}