using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.AssignExaminationStaff;

public class AssignExaminationStaffCommandValidator : AbstractValidator<AssignExaminationStaffCommand>
{
    public AssignExaminationStaffCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.RadiologistId).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.TechnicianId).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.EquipmentId).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
    }
}
