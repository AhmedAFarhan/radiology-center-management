using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class PreAuthorizationRepository : BaseRepository<PreAuthorization, Guid>, IPreAuthorizationRepository
{
    public PreAuthorizationRepository(InsuranceDbContext context) : base(context) { }

    public async Task<PreAuthorization?> GetByExaminationIdAsync(Guid examinationId, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(p => p.ExaminationId == examinationId, ct);
}