using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class StaffValidatorBase<T> : AbstractValidator<T> where T : IStaffFields
{
    protected StaffValidatorBase()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ErrorCodes.UserIdRequired);
        RuleFor(x => x.FullName).NotEmpty().WithErrorCode(ErrorCodes.FullNameRequired).MaximumLength(300).WithErrorCode(ErrorCodes.FullNameTooLong);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.PhoneNumber).NotEmpty().WithErrorCode(ErrorCodes.PhoneNumberRequired).IsEgyptianPhoneNumber().MaximumLength(30).WithErrorCode(ErrorCodes.PhoneNumberTooLong);
        RuleFor(x => x.Position).NotEmpty().WithErrorCode(ErrorCodes.PositionRequired).IsEnumerationMember<StaffPosition, T>("Position");
        RuleFor(x => x.HireDate).NotEmpty().WithErrorCode(ErrorCodes.HireDateRequired);
        RuleFor(x => x.Department).MaximumLength(200).WithErrorCode(ErrorCodes.DepartmentTooLong).When(x => !string.IsNullOrWhiteSpace(x.Department));
        RuleFor(x => x.Specialization).MaximumLength(200).WithErrorCode(ErrorCodes.SpecializationTooLong).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.LicenseNumber).MaximumLength(100).WithErrorCode(ErrorCodes.LicenseNumberTooLong).When(x => !string.IsNullOrWhiteSpace(x.LicenseNumber));
    }
}
