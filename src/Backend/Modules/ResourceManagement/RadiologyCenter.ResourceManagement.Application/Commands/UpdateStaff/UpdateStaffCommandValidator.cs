using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PhoneNumber).NotEmpty().IsEgyptianPhoneNumber().MaximumLength(30);
        RuleFor(x => x.Position).NotEmpty().Must(IsValidPosition)
            .WithMessage("Position must be one of: Technician, Radiologist, Receptionist, Nurse, Other.");
        RuleFor(x => x.HireDate).NotEmpty();
        RuleFor(x => x.Department).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Department));
        RuleFor(x => x.Specialization).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Specialization));
        RuleFor(x => x.LicenseNumber).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.LicenseNumber));
    }

    private static bool IsValidPosition(string position) =>
        StaffPosition.GetAll<StaffPosition>().Any(p => p.Name.Equals(position, StringComparison.OrdinalIgnoreCase));
}
