using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteStaff;

public static class DeleteStaffCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteStaffCommand command,
        IStaffRepository staffRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure(Error.NotFound(ErrorCodes.StaffNotFound, "Staff", command.StaffId));

        staffRepository.Remove(staff);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
