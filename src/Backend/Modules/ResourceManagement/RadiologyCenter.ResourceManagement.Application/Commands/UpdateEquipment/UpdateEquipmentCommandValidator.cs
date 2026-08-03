using FluentValidation;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateEquipment;

public class UpdateEquipmentCommandValidator : AbstractValidator<UpdateEquipmentCommand>
{
    public UpdateEquipmentCommandValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Modality).NotEmpty().Must(IsValidModality)
            .WithMessage("Modality must be one of: XRay, CT, MRI, Ultrasound, Mammography, Fluoroscopy, DEXA, Other.");
        RuleFor(x => x.SerialNumber).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.SerialNumber));
    }

    private static bool IsValidModality(string modality) =>
        EquipmentModality.GetAll<EquipmentModality>().Any(m => m.Name.Equals(modality, StringComparison.OrdinalIgnoreCase));
}
