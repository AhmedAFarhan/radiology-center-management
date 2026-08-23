using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
    {
        public CreateClaimCommandValidator()
        {
            RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
            RuleFor(x => x.PatientId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
            RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
            RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
            RuleFor(x => x.BilledAmount).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        }
    }