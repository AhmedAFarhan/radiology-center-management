using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.Identity.Application.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x.Password).NotEmpty().StrongPassword();
        RuleFor(x => x.RoleIds).NotEmpty().WithErrorCode(ErrorCodes.AtLeastOneRole)
            .Must(ids => ids is not null && ids.All(id => id != Guid.Empty))
            .WithErrorCode(ErrorCodes.RoleIdsNotEmpty);
    }
}
