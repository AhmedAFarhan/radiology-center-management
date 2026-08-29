using FluentValidation;
using RadiologyCenter.Identity.Application.Localization;
using RadiologyCenter.BuildingBlocks.Application.Validation;

namespace RadiologyCenter.Identity.Application.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ErrorCodes.UserIdRequired);
        RuleFor(x => x.FirstName).NotEmpty().WithErrorCode(ErrorCodes.FirstNameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.FirstNameTooLong);
        RuleFor(x => x.LastName).NotEmpty().WithErrorCode(ErrorCodes.LastNameRequired).MaximumLength(100).WithErrorCode(ErrorCodes.LastNameTooLong);
        RuleFor(x => x.PhoneNumber).IsEgyptianPhoneNumber().When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}
