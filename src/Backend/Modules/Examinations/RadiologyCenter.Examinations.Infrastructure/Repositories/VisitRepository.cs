using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Infrastructure.Persistence;

namespace RadiologyCenter.Examinations.Infrastructure.Repositories;

public class VisitRepository : BaseRepository<Visit, Guid>, IVisitRepository
{
    public VisitRepository(ExaminationsDbContext context) : base(context) { }

    public async Task<Visit?> GetWithExaminationsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(v => v.Examinations)
                .ThenInclude(e => e.Items)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public override async Task<Visit?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(v => v.Id == id, ct);
}
