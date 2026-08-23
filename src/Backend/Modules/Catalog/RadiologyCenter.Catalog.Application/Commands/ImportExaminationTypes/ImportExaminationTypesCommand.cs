using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Catalog.Application.Commands.ImportExaminationTypes;

public record ImportExaminationTypesCommand(byte[] FileContent) : ICommand;
