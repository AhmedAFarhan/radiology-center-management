using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Persistence;

namespace RadiologyCenter.Identity.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(IdentityDbContext context)
        : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.AsSplitQuery().Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await DbSet.AsSplitQuery().Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default) =>
        await DbSet.AsSplitQuery().Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).FirstOrDefaultAsync(u => u.UserName == userName, ct);

    public override async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.AsSplitQuery().Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions).ToListAsync(ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        await DbSet.AsNoTracking().AnyAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default) =>
        await DbSet.AsNoTracking().AnyAsync(u => u.UserName == userName, ct);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default) =>
        await DbSet.AsSplitQuery().Include(u => u.AssignedRoles).ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken), ct);

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        DbSet.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken ct = default)
    {
        DbSet.Remove(user);
        return Task.CompletedTask;
    }

    protected override void ApplyIncludes(DynamicSpecification<User> spec) =>
        spec.AddInclude(u => u.AssignedRoles);
}