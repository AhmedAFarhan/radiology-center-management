using FluentValidation;
using RadiologyCenter.ResourceManagement.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public abstract class WorkShiftValidatorBase<T> : AbstractValidator<T> where T : IWorkShiftFields
{
    protected WorkShiftValidatorBase()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Date).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.StartTime).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.EndTime).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithErrorCode(ErrorCodes.WorkShiftEndAfterStart);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
