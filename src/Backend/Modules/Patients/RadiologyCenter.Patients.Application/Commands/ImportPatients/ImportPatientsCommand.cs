using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Patients.Application.Commands.ImportPatients;

public record ImportPatientsCommand(byte[] FileContent) : ICommand;
