using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Domain.Entities;
using RadiologyCenter.Reports.Infrastructure.Persistence;

namespace RadiologyCenter.Reports.Infrastructure.Repositories;

public class ReportTemplateRepository : BaseRepository<ReportTemplate, Guid>, IReportTemplateRepository
{
    public ReportTemplateRepository(ReportsDbContext context) : base(context) { }

    public override async Task<ReportTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<ReportTemplate?> GetByIdWithSectionsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(t => t.Sections)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        if (excludeId.HasValue)
            return await DbSet.AnyAsync(t => t.Name == name && t.Id != excludeId.Value, ct);
        return await DbSet.AnyAsync(t => t.Name == name, ct);
    }
}