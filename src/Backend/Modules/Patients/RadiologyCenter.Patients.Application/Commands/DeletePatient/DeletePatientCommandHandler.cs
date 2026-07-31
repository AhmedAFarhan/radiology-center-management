using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Patients.Application.Abstractions;

namespace RadiologyCenter.Patients.Application.Commands.DeletePatient;

public static class DeletePatientCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeletePatientCommand command,
        IPatientRepository patientRepository,
        IPatientsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var patient = await patientRepository.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
            return Result.Failure(Error.NotFound("Patient", command.PatientId));

        patientRepository.Remove(patient);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
