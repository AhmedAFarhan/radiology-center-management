using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.AssignExaminationStaff;

public class AssignExaminationStaffCommandValidator : AbstractValidator<AssignExaminationStaffCommand>
{
    public AssignExaminationStaffCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.RadiologistId).NotEmpty().WithErrorCode(ErrorCodes.RadiologistIdRequired);
        RuleFor(x => x.TechnicianId).NotEmpty().WithErrorCode(ErrorCodes.TechnicianIdRequired);
        RuleFor(x => x.EquipmentId).NotEmpty().WithErrorCode(ErrorCodes.EquipmentIdRequired);
    }
}
