using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExaminationType;

public static class UpdateExaminationTypeCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        var modality = Modality.FromName<Modality>(command.Modality);

        examinationType.Update(
            command.Code,
            command.Name,
            modality,
            command.BodyPart,
            command.StandardDurationMinutes,
            command.Price,
            command.RequiresPreparation,
            command.RequiresConsent);

        examinationTypeRepository.Update(examinationType);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
