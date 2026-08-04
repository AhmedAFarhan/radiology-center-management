using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteAllowanceAssignment;

public static class DeleteAllowanceAssignmentCommandHandler
{
    public static Task<Result> HandleAsync(
        DeleteAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.DeleteAsync(
            repository,
            unitOfWork,
            command.Id,
            "AllowanceAssignment",
            ct);
}