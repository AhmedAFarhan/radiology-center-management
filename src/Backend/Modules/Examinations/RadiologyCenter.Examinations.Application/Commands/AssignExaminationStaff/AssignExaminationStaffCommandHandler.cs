using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.AssignExaminationStaff;

public static class AssignExaminationStaffCommandHandler
{
    public static async Task<Result> HandleAsync(
        AssignExaminationStaffCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        examination.AssignStaff(command.RadiologistId, command.TechnicianId);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
