using Mapster;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Application.DTOs;

namespace RadiologyCenter.Catalog.Application.Queries.GetExaminationTypeById;

public static class GetExaminationTypeByIdQueryHandler
{
    public static async Task<Result<ExaminationTypeDto>> HandleAsync(
        GetExaminationTypeByIdQuery query,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetByIdAsync(query.Id, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationTypeDto>(Error.NotFound("ExaminationType", query.Id));

        return Result.Success(examinationType.Adapt<ExaminationTypeDto>());
    }
}
