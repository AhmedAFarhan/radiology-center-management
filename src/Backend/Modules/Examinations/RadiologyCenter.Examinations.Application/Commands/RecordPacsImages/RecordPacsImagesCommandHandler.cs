using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.RecordPacsImages;

public static class RecordPacsImagesCommandHandler
{
    public static async Task<Result> HandleAsync(
        RecordPacsImagesCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        examination.RecordPacsImages(command.StudyInstanceUID, command.AccessionNumber);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}