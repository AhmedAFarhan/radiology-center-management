using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponentById;

public static class GetSalaryComponentByIdQueryHandler
{
    public static async Task<Result<SalaryComponentDto>> HandleAsync(
        GetSalaryComponentByIdQuery query,
        ISalaryComponentRepository salaryComponentRepository,
        CancellationToken ct)
    {
        var component = await salaryComponentRepository.GetByIdAsync(query.Id, ct);
        if (component is null)
            return Result.Failure<SalaryComponentDto>(Error.NotFound("SalaryComponent", query.Id));

        return Result.Success(component.Adapt<SalaryComponentDto>());
    }
}