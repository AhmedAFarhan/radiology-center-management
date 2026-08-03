using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateWorkShift;

public static class CreateWorkShiftCommandHandler
{
    public static async Task<Result<WorkShiftDto>> HandleAsync(
        CreateWorkShiftCommand command,
        IWorkShiftRepository workShiftRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
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
