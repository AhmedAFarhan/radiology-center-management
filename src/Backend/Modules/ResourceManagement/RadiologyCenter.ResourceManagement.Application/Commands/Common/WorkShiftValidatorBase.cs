using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Localization;
using ErrorCodes = RadiologyCenter.ResourceManagement.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class WorkShiftValidatorBase<T> : AbstractValidator<T> where T : IWorkShiftFields
{
    protected WorkShiftValidatorBase()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(ErrorCodes.StaffIdRequired);
        RuleFor(x => x.Date).NotEmpty().WithErrorCode(ErrorCodes.DateRequired);
        RuleFor(x => x.StartTime).NotEmpty().WithErrorCode(ErrorCodes.StartTimeRequired);
        RuleFor(x => x.EndTime).NotEmpty().WithErrorCode(ErrorCodes.EndTimeRequired);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithErrorCode(ErrorCodes.WorkShiftEndAfterStart);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
