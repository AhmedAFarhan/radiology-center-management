using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Infrastructure.Persistence;

namespace RadiologyCenter.Examinations.Infrastructure.Repositories;

public class ExaminationRepository : BaseRepository<Examination, Guid>, IExaminationRepository
{
    public ExaminationRepository(ExaminationsDbContext context) : base(context) { }

    public override async Task<Examination?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<bool> HasActiveExaminationsByTypeAsync(Guid examinationTypeId, CancellationToken ct = default) =>
        await DbSet.AnyAsync(
            e => e.ExaminationTypeId == examinationTypeId
                 && !e.IsDeleted
                 && e.Status != ExaminationStatus.Completed
                 && e.Status != ExaminationStatus.Cancelled,
            ct);
}
