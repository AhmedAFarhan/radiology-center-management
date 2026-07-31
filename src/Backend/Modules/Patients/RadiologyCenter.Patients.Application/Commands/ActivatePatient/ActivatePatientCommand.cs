namespace RadiologyCenter.Patients.Application.Commands.ActivatePatient;

public record ActivatePatientCommand(Guid PatientId) : ICommand;
