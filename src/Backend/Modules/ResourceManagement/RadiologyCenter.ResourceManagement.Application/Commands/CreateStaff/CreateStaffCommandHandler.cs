using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateStaff;

public static class CreateStaffCommandHandler
{
    public static async Task<Result<StaffDto>> HandleAsync(
        CreateStaffCommand command,
        IStaffRepository staffRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var position = StaffPosition.FromName<StaffPosition>(command.Position);
        var salaryCalculationRule = string.IsNullOrWhiteSpace(command.SalaryCalculationRule)
            ? SalaryCalculationRule.FixedPlusFees
            : SalaryCalculationRule.FromName<SalaryCalculationRule>(command.SalaryCalculationRule);

        var staff = Staff.Create(
            command.UserId,
            command.FullName,
            command.PhoneNumber,
            position,
            command.HireDate,
            command.Department,
            command.Specialization,
            command.LicenseNumber,
            salaryCalculationRule);

        await staffRepository.AddAsync(staff, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(staff.Adapt<StaffDto>());
    }
}
