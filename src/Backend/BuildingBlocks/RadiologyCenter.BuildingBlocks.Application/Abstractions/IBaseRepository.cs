using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;

namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface IBaseRepository<TEntity, in TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> FindAsync(ISpecification<TEntity> spec, CancellationToken ct = default);
    Task<TEntity?> FindSingleAsync(ISpecification<TEntity> spec, CancellationToken ct = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<PagedResult<TEntity>> GetPagedAsync(PaginationParams pagination, CancellationToken ct = default);
}
