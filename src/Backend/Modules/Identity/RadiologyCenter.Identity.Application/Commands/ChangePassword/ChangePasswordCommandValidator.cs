using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.Identity.Application.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().StrongPassword();
        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithErrorCode(ErrorCodes.PasswordDifferent);
    }
}