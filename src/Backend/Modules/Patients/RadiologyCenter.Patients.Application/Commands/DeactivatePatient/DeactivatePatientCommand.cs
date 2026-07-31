namespace RadiologyCenter.Patients.Application.Commands.DeactivatePatient;

public record DeactivatePatientCommand(Guid PatientId) : ICommand;
