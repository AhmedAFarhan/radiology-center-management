using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class StaffValidatorBase<T> : AbstractValidator<T> where T : IStaffFields
{
    protected StaffValidatorBase()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.FullName).ContainsAtLeastTwoNameParts();
        RuleFor(x => x.PhoneNumber).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x.Position).NotEmpty().IsEnumerationMember<StaffPosition, T>("Position");
        RuleFor(x => x.HireDate).NotEmpty();
        RuleFor(x => x.Department).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Department));
        RuleFor(x => x.Specialization).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.LicenseNumber).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.LicenseNumber));
    }
}
