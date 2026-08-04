using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateAllowanceAssignment;

public static class UpdateAllowanceAssignmentCommandHandler
{
    public static Task<Result> HandleAsync(
        UpdateAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.UpdateAsync(
            allowanceAssignmentRepository,
            unitOfWork,
            command.AllowanceAssignmentId,
            "AllowanceAssignment",
            assignment =>
            {
                var frequency = string.IsNullOrWhiteSpace(command.Frequency)
                    ? null
                    : Frequency.FromName<Frequency>(command.Frequency);

                assignment.Update(
                    command.Name,
                    command.Amount,
                    command.EffectiveDate,
                    frequency,
                    command.EndDate,
                    command.IsPerWorkDay);
            },
            ct);
}