using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Email).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).EmailAddress().MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.FirstName).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.LastName).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.PhoneNumber).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).StrongPassword();
        RuleFor(x => x.RoleIds).NotEmpty().WithErrorCode(ErrorCodes.AtLeastOneRole)
            .Must(ids => ids is not null && ids.All(id => id != Guid.Empty))
            .WithErrorCode(ErrorCodes.RoleIdsNotEmpty);
    }
}
