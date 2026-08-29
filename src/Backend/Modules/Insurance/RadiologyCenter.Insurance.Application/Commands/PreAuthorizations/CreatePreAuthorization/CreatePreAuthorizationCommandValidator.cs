using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.CreatePreAuthorization;

public class CreatePreAuthorizationCommandValidator : AbstractValidator<CreatePreAuthorizationCommand>
{
    public CreatePreAuthorizationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired);
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(ErrorCodes.PolicyIdRequired);
        RuleFor(x => x.EstimatedAmount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.EstimatedAmountCannotBeNegative);
    }
}
