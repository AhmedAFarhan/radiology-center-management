namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IPacsStudyVerifier
{
    Task<Result> VerifyAsync(string studyUid, string patientCode, string accession, CancellationToken ct);
}
