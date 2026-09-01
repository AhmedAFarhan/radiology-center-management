using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Adapters;

public class PayrollStaffDirectory : IPayrollStaffDirectory
{
    private readonly IStaffRepository _staffRepository;

    public PayrollStaffDirectory(IStaffRepository staffRepository) => _staffRepository = staffRepository;

    public async Task<bool> ExistsAsync(Guid staffId, CancellationToken ct = default) =>
        await _staffRepository.GetByIdAsync(staffId, ct) is not null;

    public async Task<IReadOnlyList<Guid>> GetActiveStaffIdsAsync(CancellationToken ct = default)
    {
        var spec = new BuildingBlocks.Domain.Specifications.DynamicSpecification<Staff>(staff => staff.IsActive);
        var staff = await _staffRepository.FindAsync(spec, ct);
        return staff.Select(s => s.Id).ToList();
    }
}
