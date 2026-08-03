using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryById;

public static class GetSalaryByIdQueryHandler
{
    public static async Task<Result<SalaryDto>> HandleAsync(
        GetSalaryByIdQuery query,
        ISalaryRepository salaryRepository,
        CancellationToken ct)
    {
        var salary = await salaryRepository.GetByIdAsync(query.Id, ct);
        if (salary is null)
            return Result.Failure<SalaryDto>(Error.NotFound("Salary", query.Id));

        return Result.Success(salary.Adapt<SalaryDto>());
    }
}