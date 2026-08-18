namespace RadiologyCenter.Examinations.Application.Commands.RecordPacsImages;

public record RecordPacsImagesCommand(
    Guid ExaminationId,
    string? StudyInstanceUID,
    string? AccessionNumber = null) : ICommand;