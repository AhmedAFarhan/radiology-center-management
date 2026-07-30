using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;

public class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public BaseRepository(DbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default) =>
        await DbSet.FindAsync([id], ct);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.ToListAsync(ct);

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(ISpecification<TEntity> spec, CancellationToken ct = default) =>
        await ApplySpecification(spec).ToListAsync(ct);

    public virtual async Task<TEntity?> FindSingleAsync(ISpecification<TEntity> spec, CancellationToken ct = default) =>
        await ApplySpecification(spec).FirstOrDefaultAsync(ct);

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await DbSet.AddAsync(entity, ct);
        return entity;
    }

    public virtual void Update(TEntity entity) =>
        DbSet.Update(entity);

    public virtual void Remove(TEntity entity) =>
        DbSet.Remove(entity);

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking();
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        return PagedResult<TEntity>.Create(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec) =>
        SpecificationEvaluator<TEntity>.GetQuery(DbSet.AsQueryable(), spec);
}
