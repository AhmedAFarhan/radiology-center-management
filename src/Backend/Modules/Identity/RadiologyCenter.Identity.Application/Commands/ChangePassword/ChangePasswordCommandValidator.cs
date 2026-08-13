using FluentValidation;
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
            .WithMessage("New password must be different from the current password.");
    }
}