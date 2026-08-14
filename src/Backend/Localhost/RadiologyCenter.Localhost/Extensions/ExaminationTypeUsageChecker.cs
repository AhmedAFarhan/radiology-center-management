using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public class ExaminationTypeUsageChecker : IExaminationTypeUsageChecker
{
    private readonly IExaminationRepository _examinationRepository;

    public ExaminationTypeUsageChecker(IExaminationRepository examinationRepository)
        => _examinationRepository = examinationRepository;

    public async Task<bool> HasActiveExaminationsAsync(Guid examinationTypeId, CancellationToken ct = default)
        => await _examinationRepository.HasActiveExaminationsByTypeAsync(examinationTypeId, ct);
}