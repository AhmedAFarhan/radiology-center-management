using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;

public static class UpdateStaffCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateStaffCommand command,
        IStaffRepository staffRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure(Error.NotFound(ErrorCodes.StaffNotFound, "Staff", command.StaffId));

        var position = StaffPosition.FromName<StaffPosition>(command.Position);
        var salaryCalculationRule = string.IsNullOrWhiteSpace(command.SalaryCalculationRule)
            ? SalaryCalculationRule.FixedPlusFees
            : SalaryCalculationRule.FromName<SalaryCalculationRule>(command.SalaryCalculationRule);

        staff.Update(
            command.UserId,
            command.FullName,
            command.PhoneNumber,
            position,
            command.HireDate,
            command.Department,
            command.Specialization,
            command.LicenseNumber,
            salaryCalculationRule);

        staffRepository.Update(staff);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
