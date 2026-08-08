using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.CreatePreAuthorization;

public class CreatePreAuthorizationCommandValidator : AbstractValidator<CreatePreAuthorizationCommand>
{
    public CreatePreAuthorizationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.EstimatedAmount).GreaterThanOrEqualTo(0);
    }
}