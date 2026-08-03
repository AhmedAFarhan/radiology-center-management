using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.SetEquipmentStatus;

public class SetEquipmentStatusCommandValidator : AbstractValidator<SetEquipmentStatusCommand>
{
    public SetEquipmentStatusCommandValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty().IsEnumerationMember<EquipmentStatus, SetEquipmentStatusCommand>("Status");
    }
}
