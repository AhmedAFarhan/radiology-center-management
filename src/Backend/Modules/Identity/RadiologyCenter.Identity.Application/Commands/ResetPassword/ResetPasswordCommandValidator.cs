using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.Identity.Application.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().StrongPassword();
    }
}