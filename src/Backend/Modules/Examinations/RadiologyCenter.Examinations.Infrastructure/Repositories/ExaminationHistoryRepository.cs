using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Infrastructure.Persistence;

namespace RadiologyCenter.Examinations.Infrastructure.Repositories;

public class ExaminationHistoryRepository : BaseRepository<ExaminationHistory, Guid>, IExaminationHistoryRepository
{
    public ExaminationHistoryRepository(ExaminationsDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ExaminationHistory>> GetByCompletedRangeAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var query = DbSet.AsQueryable();

        if (from is not null)
            query = query.Where(h => h.CompletedAt >= from);

        if (to is not null)
            query = query.Where(h => h.CompletedAt <= to);

        return await query.ToListAsync(ct);
    }

    public async Task<ExaminationHistory?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(h => h.ExaminationId == examinationId, ct);
}
