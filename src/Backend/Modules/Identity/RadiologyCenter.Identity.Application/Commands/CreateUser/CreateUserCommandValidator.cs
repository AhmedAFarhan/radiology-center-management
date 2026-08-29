using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.Identity.Application.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithErrorCode(ErrorCodes.UserNameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.UserNameTooLong);
        RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCodes.EmailRequired).EmailAddress().WithErrorCode(ErrorCodes.EmailInvalid).MaximumLength(200).WithErrorCode(ErrorCodes.EmailTooLong);
        RuleFor(x => x.FirstName).NotEmpty().WithErrorCode(ErrorCodes.FirstNameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.FirstNameTooLong);
        RuleFor(x => x.LastName).NotEmpty().WithErrorCode(ErrorCodes.LastNameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.LastNameTooLong);
        RuleFor(x => x.PhoneNumber).NotEmpty().WithErrorCode(ErrorCodes.PhoneNumberRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(ErrorCodes.PhoneNumberTooLong);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.PasswordRequired).StrongPassword();
        RuleFor(x => x.RoleIds).NotEmpty().WithErrorCode(ErrorCodes.AtLeastOneRole)
            .Must(ids => ids is not null && ids.All(id => id != Guid.Empty))
            .WithErrorCode(ErrorCodes.RoleIdsNotEmpty);
    }
}
