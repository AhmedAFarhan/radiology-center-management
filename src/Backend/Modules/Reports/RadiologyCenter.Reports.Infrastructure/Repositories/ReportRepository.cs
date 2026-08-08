using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.Reports.Domain.Entities;
using RadiologyCenter.Reports.Infrastructure.Persistence;

namespace RadiologyCenter.Reports.Infrastructure.Repositories;

public class ReportRepository : BaseRepository<RadiologyReport, Guid>, IReportRepository
{
    public ReportRepository(ReportsDbContext context) : base(context) { }

    public override async Task<RadiologyReport?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<RadiologyReport?> GetByIdWithVersionsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(r => r.Versions)
                .ThenInclude(v => v.Sections)
            .Include(r => r.Versions)
                .ThenInclude(v => v.Findings)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<RadiologyReport?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default) =>
        await DbSet
            .Include(r => r.Versions)
                .ThenInclude(v => v.Sections)
            .Include(r => r.Versions)
                .ThenInclude(v => v.Findings)
            .FirstOrDefaultAsync(r => r.ExaminationId == examinationId, ct);

    public async Task<bool> HasReportByExaminationAsync(Guid examinationId, CancellationToken ct = default) =>
        await DbSet.AnyAsync(r => r.ExaminationId == examinationId, ct);
}