using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.BilledAmount).GreaterThanOrEqualTo(0);
    }
}