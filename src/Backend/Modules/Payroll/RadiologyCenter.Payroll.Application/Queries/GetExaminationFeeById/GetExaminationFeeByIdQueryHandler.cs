using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFeeById;

public static class GetExaminationFeeByIdQueryHandler
{
    public static Task<Result<ExaminationFeeDto>> HandleAsync(
        GetExaminationFeeByIdQuery query,
        IExaminationFeeRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetByIdAsync<ExaminationFee, ExaminationFeeDto>(
            repository,
            query.Id,
            "ExaminationFee",
            ct);
}