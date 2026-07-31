namespace RadiologyCenter.Patients.Application.Abstractions;

public interface IPatientCodeGenerator
{
    Task<string> GenerateNextAsync(CancellationToken ct = default);
}
