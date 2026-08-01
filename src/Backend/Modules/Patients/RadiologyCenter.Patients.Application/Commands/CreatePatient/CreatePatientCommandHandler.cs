using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Application.DTOs;
using RadiologyCenter.Patients.Domain.Entities;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Application.Commands.CreatePatient;

public static class CreatePatientCommandHandler
{
    public static async Task<Result<PatientDto>> HandleAsync(
        CreatePatientCommand command,
        IPatientRepository patientRepository,
        INumberSequenceGenerator numberSequenceGenerator,
        IPatientsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var gender = Gender.FromName<Gender>(command.Gender);
        var bloodType = command.BloodType is not null
            ? BloodType.FromName<BloodType>(command.BloodType)
            : null;

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var patientCode = await numberSequenceGenerator.GenerateNextAsync(
            "Patient",
            "PTN",
            4,
            transaction.DbTransaction,
            ct);

        var patient = Patient.Create(
            patientCode,
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

        await patientRepository.AddAsync(patient, ct);
        await transaction.CommitAsync(ct);

        return Result.Success(patient.Adapt<PatientDto>());
    }
}
