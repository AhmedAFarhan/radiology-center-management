using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Insurance.Application.Abstractions;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;

namespace RadiologyCenter.Insurance.Infrastructure.Repositories;

public class InsuranceCompanyRepository : BaseRepository<InsuranceCompany, Guid>, IInsuranceCompanyRepository
{
    public InsuranceCompanyRepository(InsuranceDbContext context) : base(context) { }

    public async Task<InsuranceCompany?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(c => c.Name == name, ct);
}