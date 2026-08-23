using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class StaffValidatorBase<T> : AbstractValidator<T> where T : IStaffFields
{
    protected StaffValidatorBase()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.FullName).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(300).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.PhoneNumber).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.Position).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).IsEnumerationMember<StaffPosition, T>("Position");
        RuleFor(x => x.HireDate).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.Department).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Department));
        RuleFor(x => x.Specialization).MaximumLength(200).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.LicenseNumber).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.LicenseNumber));
    }
}
