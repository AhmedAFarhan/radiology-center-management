using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.FirstName).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.LastName).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.PhoneNumber).IsEgyptianPhoneNumber().When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}
