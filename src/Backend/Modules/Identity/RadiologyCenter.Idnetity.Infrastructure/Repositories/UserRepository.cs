using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Domain.Entities;
using RadiologyCenter.Idnetity.Infrastructure.Persistence;

namespace RadiologyCenter.Idnetity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;
    private readonly DbSet<User> _users;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
        _users = context.Users;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _users.Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _users.Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default) =>
        await _users.Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).FirstOrDefaultAsync(u => u.UserName == userName, ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await _users.Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).ToListAsync(ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        await _users.AnyAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default) =>
        await _users.AnyAsync(u => u.UserName == userName, ct);

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        await _users.AddAsync(user, ct);
        return user;
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken ct = default)
    {
        _users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<PagedResult<User>> GetPagedAsync(QueryRequest request, CancellationToken ct = default)
    {
        var query = _users.Include(u => u.AssignedRoles).AsNoTracking();
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(ct);

        return new PagedResult<User>(items, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);
    }
}
