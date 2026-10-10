using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.RecordPacsImages;

public static class RecordPacsImagesCommandHandler
{
    public static async Task<Result> HandleAsync(
        RecordPacsImagesCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        IPacsStudyVerifier verifier,
        IPatientInfoResolver patients,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        var patient = await patients.ResolveAsync(examination.PatientId, ct);
        if (patient is null || string.IsNullOrWhiteSpace(examination.AccessionNumber)
            || !string.Equals(command.AccessionNumber, examination.AccessionNumber, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(command.StudyInstanceUID))
            return Result.Failure(Error.Validation("Examinations.PacsIdentityMismatch", "Use this examination's accession and a verified patient study."));
        var verified = await verifier.VerifyAsync(command.StudyInstanceUID, patient.PatientCode, examination.AccessionNumber, ct);
        if (verified.IsFailure) return verified;
        examination.RecordPacsImages(command.StudyInstanceUID, examination.AccessionNumber);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
