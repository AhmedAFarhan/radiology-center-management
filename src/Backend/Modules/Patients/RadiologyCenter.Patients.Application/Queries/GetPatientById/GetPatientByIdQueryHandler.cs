using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Patients.Application.Localization;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Application.DTOs;

namespace RadiologyCenter.Patients.Application.Queries.GetPatientById;

public static class GetPatientByIdQueryHandler
{
    public static async Task<Result<PatientDto>> HandleAsync(
        GetPatientByIdQuery query,
        IPatientRepository patientRepository,
        CancellationToken ct)
    {
        var patient = await patientRepository.GetByIdAsync(query.Id, ct);
        if (patient is null)
            return Result.Failure<PatientDto>(Error.NotFound(ErrorCodes.PatientNotFound, "Patient", query.Id));

        return Result.Success(patient.Adapt<PatientDto>());
    }
}
