namespace RadiologyCenter.Patients.Application.Commands.DeletePatient;

public record DeletePatientCommand(Guid PatientId) : ICommand;
