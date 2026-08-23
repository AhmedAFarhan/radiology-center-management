using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.CreatePreAuthorization;

public class CreatePreAuthorizationCommandValidator : AbstractValidator<CreatePreAuthorizationCommand>
{
    public CreatePreAuthorizationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.EstimatedAmount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
    }
}