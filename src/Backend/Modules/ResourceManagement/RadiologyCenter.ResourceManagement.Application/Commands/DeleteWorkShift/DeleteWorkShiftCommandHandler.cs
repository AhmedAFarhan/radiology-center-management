using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteWorkShift;

public static class DeleteWorkShiftCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteWorkShiftCommand command,
        IWorkShiftRepository workShiftRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var workShift = await workShiftRepository.GetByIdAsync(command.WorkShiftId, ct);
        if (workShift is null)
            return Result.Failure(Error.NotFound(ErrorCodes.WorkShiftNotFound, "WorkShift", command.WorkShiftId));

        workShiftRepository.Remove(workShift);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
