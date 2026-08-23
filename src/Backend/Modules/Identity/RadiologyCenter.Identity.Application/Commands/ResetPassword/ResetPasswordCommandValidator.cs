using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.NewPassword).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).StrongPassword();
    }
}