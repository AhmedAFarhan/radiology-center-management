using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryById;

public static class GetSalaryByIdQueryHandler
{
    public static Task<Result<SalaryDto>> HandleAsync(
        GetSalaryByIdQuery query,
        ISalaryRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetByIdAsync<Salary, SalaryDto>(
            repository,
            query.Id,
            "Salary",
            ct);
}