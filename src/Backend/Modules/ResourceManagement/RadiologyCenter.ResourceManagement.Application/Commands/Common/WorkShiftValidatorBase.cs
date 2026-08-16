using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Localization;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class WorkShiftValidatorBase<T> : AbstractValidator<T> where T : IWorkShiftFields
{
    protected WorkShiftValidatorBase()
    {
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty();
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithErrorCode(ErrorCodes.WorkShiftEndAfterStart);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
