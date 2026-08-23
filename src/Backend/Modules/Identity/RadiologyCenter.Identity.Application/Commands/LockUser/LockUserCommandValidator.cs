using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.LockUser;

public class LockUserCommandValidator : AbstractValidator<LockUserCommand>
{
    public LockUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.LockoutEnd).GreaterThan(DateTimeOffset.UtcNow).WithErrorCode(ErrorCodes.LockoutEndMustBeFuture);
    }
}
