using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
    {
        public CreateClaimCommandValidator()
        {
            RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
            RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(ErrorCodes.PatientIdRequired);
            RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(ErrorCodes.PolicyIdRequired);
            RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(ErrorCodes.PreAuthorizationIdRequired);
            RuleFor(x => x.BilledAmount).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.BilledAmountCannotBeNegative);
        }
    }
