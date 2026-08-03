using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.ActivateStaff;

public static class ActivateStaffCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateStaffCommand command,
        IStaffRepository staffRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure(Error.NotFound("Staff", command.StaffId));

        staff.Activate();
        staffRepository.Update(staff);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
