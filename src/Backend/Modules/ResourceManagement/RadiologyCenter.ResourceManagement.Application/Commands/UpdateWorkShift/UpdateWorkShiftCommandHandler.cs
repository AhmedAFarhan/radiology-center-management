using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;

public static class UpdateWorkShiftCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateWorkShiftCommand command,
        IWorkShiftRepository workShiftRepository,
        IStaffRepository staffRepository,
        IEquipmentRepository equipmentRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var workShift = await workShiftRepository.GetByIdAsync(command.WorkShiftId, ct);
        if (workShift is null)
            return Result.Failure(Error.NotFound("WorkShift", command.WorkShiftId));

        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure(Error.NotFound("Staff", command.StaffId));

        if (command.EquipmentId is { } equipmentId)
        {
            var equipment = await equipmentRepository.GetByIdAsync(equipmentId, ct);
            if (equipment is null)
                return Result.Failure(Error.NotFound("Equipment", equipmentId));
        }

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
