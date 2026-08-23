using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.NewPassword).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).StrongPassword();
        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithErrorCode(ErrorCodes.PasswordDifferent);
    }
}