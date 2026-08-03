using FluentValidation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.SetEquipmentStatus;

public class SetEquipmentStatusCommandValidator : AbstractValidator<SetEquipmentStatusCommand>
{
    public SetEquipmentStatusCommandValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty().Must(IsValidStatus)
            .WithMessage("Status must be one of: Operational, UnderMaintenance, OutOfService, Retired.");
    }

    private static bool IsValidStatus(string status) =>
        EquipmentStatus.GetAll<EquipmentStatus>().Any(s => s.Name.Equals(status, StringComparison.OrdinalIgnoreCase));
}
