using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Services;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.BuildingBlocks.Infrastructure.Services;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Persistence;

namespace RadiologyCenter.Identity.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _context;
    private readonly DbSet<Role> _roles;

    public RoleRepository(IdentityDbContext context)
    {
        _context = context;
        _roles = context.Roles;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<Role>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        return await _roles.Include(r => r.Permissions)
            .Where(r => idList.Contains(r.Id))
            .ToListAsync(ct);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await _roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Name == name, ct);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct = default) =>
        await _roles.Include(r => r.Permissions).ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
        await _roles.AsNoTracking().AnyAsync(r => r.Name == name, ct);

    public async Task<Role> AddAsync(Role role, CancellationToken ct = default)
    {
        await _roles.AddAsync(role, ct);
        return role;
    }

    public Task UpdateAsync(Role role, CancellationToken ct = default)
    {
        _roles.Update(role);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Role role, CancellationToken ct = default)
    {
        _roles.Remove(role);
        return Task.CompletedTask;
    }

    public async Task<PagedResult<Role>> GetPagedAsync(QueryRequest request, CancellationToken ct = default)
    {
        var spec = new DynamicSpecification<Role>(FilterExpressionBuilder.Build<Role>(request.Filters));

        if (SearchExpressionBuilder.Build<Role>(request.SearchTerm, request.SearchFields) is { } searchCriteria)
            spec.AddCriteria(searchCriteria);

        spec.AddInclude(r => r.Permissions);

        if (SortExpressionBuilder.TryBuildSelector<Role>(request.SortBy, out var sortSelector))
        {
            if (request.SortDescending)
                spec.ApplyOrderByDescending(sortSelector);
            else
                spec.ApplyOrderBy(sortSelector);
        }

        var query = SpecificationEvaluator<Role>.GetQuery(_roles.AsNoTracking(), spec);
        var totalCount = await query.CountAsync(ct);

        spec.ApplyPaging(
            (request.Pagination.PageNumber - 1) * request.Pagination.PageSize,
            request.Pagination.PageSize);

        var items = await SpecificationEvaluator<Role>.GetQuery(_roles.AsNoTracking(), spec).ToListAsync(ct);

        return new PagedResult<Role>(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }
}
