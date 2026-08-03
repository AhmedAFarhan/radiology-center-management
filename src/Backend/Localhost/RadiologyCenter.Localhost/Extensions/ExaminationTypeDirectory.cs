using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public class ExaminationTypeDirectory : IExaminationTypeDirectory
{
    private readonly IExaminationTypeRepository _examinationTypeRepository;

    public ExaminationTypeDirectory(IExaminationTypeRepository examinationTypeRepository)
        => _examinationTypeRepository = examinationTypeRepository;

    public async Task<bool> ExistsAsync(Guid examinationTypeId, CancellationToken ct = default) =>
        await _examinationTypeRepository.GetByIdAsync(examinationTypeId, ct) is not null;
}