using FluentValidation;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;

public class UpdateWorkShiftCommandValidator : AbstractValidator<UpdateWorkShiftCommand>
{
    public UpdateWorkShiftCommandValidator()
    {
        RuleFor(x => x.WorkShiftId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty();
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
