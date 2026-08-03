using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFeeById;

public static class GetExaminationFeeByIdQueryHandler
{
    public static async Task<Result<ExaminationFeeDto>> HandleAsync(
        GetExaminationFeeByIdQuery query,
        IExaminationFeeRepository examinationFeeRepository,
        CancellationToken ct)
    {
        var fee = await examinationFeeRepository.GetByIdAsync(query.Id, ct);
        if (fee is null)
            return Result.Failure<ExaminationFeeDto>(Error.NotFound("ExaminationFee", query.Id));

        return Result.Success(fee.Adapt<ExaminationFeeDto>());
    }
}