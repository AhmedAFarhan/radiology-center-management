using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationById;

public static class GetExaminationByIdQueryHandler
{
    public static async Task<Result<ExaminationDto>> HandleAsync(
        GetExaminationByIdQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetWithItemsAsync(query.Id, ct);
        if (examination is null)
            return Result.Failure<ExaminationDto>(Error.NotFound("Examination", query.Id));

        var type = await examinationTypeRepository.GetByIdAsync(examination.ExaminationTypeId, ct);

        return Result.Success(examination.ToDto(type?.Name ?? string.Empty));
    }
}
