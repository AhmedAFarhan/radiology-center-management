using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Domain.Entities;
using RadiologyCenter.Idnetity.Infrastructure.Persistence;

namespace RadiologyCenter.Idnetity.Infrastructure.Repositories;

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

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await _roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Name == name, ct);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct = default) =>
        await _roles.Include(r => r.Permissions).ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
        await _roles.AnyAsync(r => r.Name == name, ct);

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
        var query = _roles.Include(r => r.Permissions).AsNoTracking();
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Role>(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }
}
