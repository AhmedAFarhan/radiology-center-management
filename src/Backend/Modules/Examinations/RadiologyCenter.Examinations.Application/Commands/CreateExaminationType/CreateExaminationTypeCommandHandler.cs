using Mapster;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExaminationType;

public static class CreateExaminationTypeCommandHandler
{
    public static async Task<Result<ExaminationTypeDto>> HandleAsync(
        CreateExaminationTypeCommand command,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var modality = Modality.FromName<Modality>(command.Modality);

        var examinationType = ExaminationType.Create(
            command.Code,
            command.Name,
            modality,
            command.BodyPart,
            command.StandardDurationMinutes,
            command.Price,
            command.RequiresPreparation,
            command.RequiresConsent);

        if (command.Items is not null)
        {
            foreach (var item in command.Items)
                examinationType.AddItem(
                    item.ItemId,
                    item.Quantity,
                    item.IsContrast,
                    item.IsRequired,
                    item.Notes);
        }

        await examinationTypeRepository.AddAsync(examinationType, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(examinationType.Adapt<ExaminationTypeDto>());
    }
}
