using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.Common;

public static class EntityCommands
{
    public static async Task<Result> SetActiveAsync<TEntity>(
        IBaseRepository<TEntity, Guid> repository,
        IPayrollUnitOfWork unitOfWork,
        Guid id,
        string entityName,
        Action<TEntity> activate,
        CancellationToken ct)
        where TEntity : Entity<Guid>
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure(Error.NotFound(entityName, id));

        activate(entity);
        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public static async Task<Result> DeleteAsync<TEntity>(
        IBaseRepository<TEntity, Guid> repository,
        IPayrollUnitOfWork unitOfWork,
        Guid id,
        string entityName,
        CancellationToken ct)
        where TEntity : Entity<Guid>
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure(Error.NotFound(entityName, id));

        repository.Remove(entity);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public static async Task<Result> UpdateAsync<TEntity>(
        IBaseRepository<TEntity, Guid> repository,
        IPayrollUnitOfWork unitOfWork,
        Guid id,
        string entityName,
        Action<TEntity> update,
        CancellationToken ct)
        where TEntity : Entity<Guid>
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure(Error.NotFound(entityName, id));

        update(entity);
        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public static async Task<Result<TDto>> GetByIdAsync<TEntity, TDto>(
        IBaseRepository<TEntity, Guid> repository,
        Guid id,
        string entityName,
        CancellationToken ct)
        where TEntity : Entity<Guid>
        where TDto : class
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure<TDto>(Error.NotFound(entityName, id));

        return Result.Success(entity.Adapt<TDto>());
    }

    public static async Task<Result<PagedResult<TDto>>> GetPagedAsync<TEntity, TDto>(
        IBaseRepository<TEntity, Guid> repository,
        QueryRequest request,
        CancellationToken ct)
        where TEntity : Entity<Guid>
        where TDto : class
    {
        var paged = await repository.GetPagedAsync(request, ct);
        var dtos = paged.Items.Select(e => e.Adapt<TDto>()).ToList();

        return Result.Success(new PagedResult<TDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}