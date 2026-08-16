using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Patients.Application.Localization;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Application.Commands.UpdatePatient;

public static class UpdatePatientCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdatePatientCommand command,
        IPatientRepository patientRepository,
        IPatientsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var patient = await patientRepository.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
            return Result.Failure(Error.NotFound(ErrorCodes.PatientNotFound, "Patient", command.PatientId));

        var gender = Gender.FromName<Gender>(command.Gender);
        var bloodType = command.BloodType is not null
            ? BloodType.FromName<BloodType>(command.BloodType)
            : null;

        patient.Update(
            command.FullName,
            gender,
            command.DateOfBirth,
            command.Age,
            command.PhoneNumber,
            command.Email,
            command.Address,
            command.NationalId,
            bloodType,
            command.Allergies,
            command.MedicalHistory,
            command.ReferringPhysician);

        patientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
