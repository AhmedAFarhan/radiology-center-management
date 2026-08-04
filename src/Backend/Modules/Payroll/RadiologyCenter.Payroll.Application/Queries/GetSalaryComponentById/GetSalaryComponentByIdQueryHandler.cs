using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponentById;

public static class GetSalaryComponentByIdQueryHandler
{
    public static Task<Result<SalaryComponentDto>> HandleAsync(
        GetSalaryComponentByIdQuery query,
        ISalaryComponentRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetByIdAsync<SalaryComponent, SalaryComponentDto>(
            repository,
            query.Id,
            "SalaryComponent",
            ct);
}