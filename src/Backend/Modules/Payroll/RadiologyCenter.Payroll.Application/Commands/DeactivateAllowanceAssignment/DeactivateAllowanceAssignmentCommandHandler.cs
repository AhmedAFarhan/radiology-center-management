using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateAllowanceAssignment;

public static class DeactivateAllowanceAssignmentCommandHandler
{
    public static Task<Result> HandleAsync(
        DeactivateAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            repository,
            unitOfWork,
            command.Id,
            "AllowanceAssignment",
            assignment => assignment.Deactivate(),
            ct);
}