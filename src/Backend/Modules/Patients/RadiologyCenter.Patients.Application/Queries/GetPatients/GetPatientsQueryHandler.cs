using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Application.DTOs;

namespace RadiologyCenter.Patients.Application.Queries.GetPatients;

public static class GetPatientsQueryHandler
{
    public static async Task<Result<PagedResult<PatientDto>>> HandleAsync(
        GetPatientsQuery query,
        IPatientRepository patientRepository,
        CancellationToken ct)
    {
        var paged = await patientRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(p => p.Adapt<PatientDto>()).ToList();

        return Result.Success(new PagedResult<PatientDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        ));
    }
}
