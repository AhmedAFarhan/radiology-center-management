using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.DeactivateStaff;

public static class DeactivateStaffCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateStaffCommand command,
        IStaffRepository staffRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure(Error.NotFound("Staff", command.StaffId));

        staff.Deactivate();
        staffRepository.Update(staff);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
