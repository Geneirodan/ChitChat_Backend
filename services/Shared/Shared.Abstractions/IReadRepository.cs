using Ardalis.Specification;
using Shared.Abstractions.Specifications;

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
        IPaginatedSpecification<TEntity> spec,
        CancellationToken token = default);

    Task<PaginatedList<TResult>> GetAllPaginatedAsync<TResult>(
        IPaginatedSpecification<TEntity, TResult> spec,
        CancellationToken token = default);
}