using Ardalis.Specification;

namespace Shared.Abstractions;

public interface IReadRepository<TEntity>
    where TEntity : class, IEntity<Guid>
{
    Task<TResult?> GetAsync<TResult>(
        ISingleResultSpecification<TEntity, TResult> spec,
        CancellationToken token = default
    );

    Task<TEntity?> GetAsync(
        ISingleResultSpecification<TEntity> spec,
        CancellationToken token = default
    );

    Task<IEnumerable<TResult?>> GetAllAsync<TResult>(
        ISpecification<TEntity, TResult> spec,
        CancellationToken token = default
    );

    Task<IEnumerable<TEntity?>> GetAllAsync(
        ISpecification<TEntity> spec,
        CancellationToken token = default
    );

    Task<PaginatedList<TEntity>> GetAllPaginatedAsync(
        ISpecification<TEntity> spec,
        int page, int perPage,
        CancellationToken token = default);

    Task<PaginatedList<TResult>> GetAllPaginatedAsync<TResult>(
        ISpecification<TEntity, TResult> spec,
        int page, int perPage,
        CancellationToken token = default);
}