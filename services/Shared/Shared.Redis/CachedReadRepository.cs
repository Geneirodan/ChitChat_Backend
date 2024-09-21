using Ardalis.Specification;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Abstractions;

namespace Shared.Redis;

public abstract class CachedReadRepository<T>(IReadRepository<T> repository, IDistributedCache cache)
    : CachedRepositoryBase<T>(cache), IReadRepository<T>
    where T : class, Abstractions.IEntity<Guid>
{
    public Task<TResult?> GetAsync<TResult>(ISingleResultSpecification<T, TResult> spec, CancellationToken token = default) =>
        InvokeWithCacheAsync(spec, () => repository.GetAsync(spec, token), token);

    public Task<T?> GetAsync(ISingleResultSpecification<T> spec, CancellationToken token = default) =>
        InvokeWithCacheAsync(spec, () => repository.GetAsync(spec, token), token);

    public Task<IEnumerable<TResult?>> GetAllAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken token = default) =>
        InvokeWithCacheAsync(spec, () => repository.GetAllAsync(spec, token), token);

    public Task<IEnumerable<T?>> GetAllAsync(ISpecification<T> spec, CancellationToken token = default) =>
        InvokeWithCacheAsync(spec, () => repository.GetAllAsync(spec, token), token);

    public Task<PaginatedList<T>> GetAllPaginatedAsync(
        ISpecification<T> spec,
        int page, int perPage,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAllPaginatedAsync(spec, page, perPage, token), token);

    public Task<PaginatedList<TResult>> GetAllPaginatedAsync<TResult>(
        ISpecification<T, TResult> spec,
        int page, int perPage,
        CancellationToken token = default
    ) => InvokeWithCacheAsync(spec, () => repository.GetAllPaginatedAsync(spec, page, perPage, token), token);
}