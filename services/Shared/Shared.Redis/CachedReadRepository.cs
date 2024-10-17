using Ardalis.Specification;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Shared.Redis;

public abstract class CachedReadRepository<T>(IReadRepository<T> repository, IDistributedCache cache, ILogger logger)
    : CachedRepositoryBase<T>(cache, logger), IReadRepository<T>
    where T : class, Abstractions.IEntity<Guid>
{
    public Task<TResult?> GetAsync<TResult>(
        ISingleResultSpecification<T, TResult> spec,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAsync(spec, token), token);

    public Task<T?> GetAsync(
        ISingleResultSpecification<T> spec,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAsync(spec, token), token);

    public Task<IEnumerable<TResult?>> GetAllAsync<TResult>(
        ISpecification<T, TResult> spec,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAllAsync(spec, token), token);

    public Task<IEnumerable<T?>> GetAllAsync(
        ISpecification<T> spec,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAllAsync(spec, token), token);

    public Task<PaginatedList<T>> GetAllPaginatedAsync(
        IPaginatedSpecification<T> spec,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAllPaginatedAsync(spec, token), token);

    public Task<PaginatedList<TResult>> GetAllPaginatedAsync<TResult>(
        IPaginatedSpecification<T, TResult> spec,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAllPaginatedAsync(spec, token), token);
}