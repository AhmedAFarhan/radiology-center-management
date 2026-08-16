using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Application.Localization;
using RadiologyCenter.Insurance.Application.DTOs;

namespace RadiologyCenter.Insurance.Application.Queries.PreAuthorizations.GetPreAuthorizationByExamination;

public static class GetPreAuthorizationByExaminationQueryHandler
{
    public static async Task<Result<PreAuthorizationDto>> HandleAsync(
        GetPreAuthorizationByExaminationQuery query,
        IPreAuthorizationRepository preAuthorizationRepository,
        CancellationToken ct)
    {
        var preAuthorization = await preAuthorizationRepository.GetByExaminationIdAsync(query.ExaminationId, ct);
        return preAuthorization is null
            ? Result.Failure<PreAuthorizationDto>(Error.NotFound(ErrorCodes.PreAuthorizationNotFound, "PreAuthorization", query.ExaminationId))
            : Result.Success(preAuthorization.ToDto());
    }
}