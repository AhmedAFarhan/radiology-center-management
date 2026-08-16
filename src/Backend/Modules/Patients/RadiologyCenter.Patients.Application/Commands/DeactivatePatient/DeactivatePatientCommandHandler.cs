using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Patients.Application.Localization;
using RadiologyCenter.Patients.Application.Abstractions;

namespace RadiologyCenter.Patients.Application.Commands.DeactivatePatient;

public static class DeactivatePatientCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivatePatientCommand command,
        IPatientRepository patientRepository,
        IPatientsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var patient = await patientRepository.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
            return Result.Failure(Error.NotFound(ErrorCodes.PatientNotFound, "Patient", command.PatientId));

        patient.Deactivate();
        patientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
