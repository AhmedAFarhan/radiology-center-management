using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateWorkShift;

public static class CreateWorkShiftCommandHandler
{
    public static async Task<Result<WorkShiftDto>> HandleAsync(
        CreateWorkShiftCommand command,
        IWorkShiftRepository workShiftRepository,
        IStaffRepository staffRepository,
        IEquipmentRepository equipmentRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure<WorkShiftDto>(Error.NotFound("Staff", command.StaffId));

        if (command.EquipmentId is { } equipmentId)
        {
            var equipment = await equipmentRepository.GetByIdAsync(equipmentId, ct);
            if (equipment is null)
                return Result.Failure<WorkShiftDto>(Error.NotFound("Equipment", equipmentId));
        }

        var workShift = WorkShift.Create(
            command.StaffId,
            command.Date,
            command.StartTime,
            command.EndTime,
            command.EquipmentId,
            command.Notes);

        await workShiftRepository.AddAsync(workShift, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(workShift.Adapt<WorkShiftDto>());
    }
}
