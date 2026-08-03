using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;

public static class UpdateWorkShiftCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateWorkShiftCommand command,
        IWorkShiftRepository workShiftRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var workShift = await workShiftRepository.GetByIdAsync(command.WorkShiftId, ct);
        if (workShift is null)
            return Result.Failure(Error.NotFound("WorkShift", command.WorkShiftId));

        workShift.Update(
            command.StaffId,
            command.Date,
            command.StartTime,
            command.EndTime,
            command.EquipmentId,
            command.Notes);

        workShiftRepository.Update(workShift);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
