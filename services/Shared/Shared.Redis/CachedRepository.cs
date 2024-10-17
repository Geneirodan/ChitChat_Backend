using Ardalis.Specification;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Shared.Abstractions;

namespace Shared.Redis;

public abstract class CachedRepository<T>(IRepository<T> repository, IDistributedCache cache, ILogger logger)
    : CachedRepositoryBase<T>(cache, logger), IRepository<T>
    where T : class, Abstractions.IEntity<Guid>
{
    public async Task<TResult?> FindAsync<TResult>(ISingleResultSpecification<T, TResult> specification,
        CancellationToken token = default) =>
        await InvokeWithCacheAsync(
            specification,
            () => repository.FindAsync(specification, token),
            token
        ).ConfigureAwait(false);


    public async Task<T?> FindAsync(ISingleResultSpecification<T> spec, CancellationToken token = default) =>
        await InvokeWithCacheAsync(
            spec,
            () => repository.FindAsync(spec, token),
            token
        ).ConfigureAwait(false);

    public Task AddAsync(T entity, CancellationToken token = default) =>
        repository.AddAsync(entity, token);

    public Task UpdateAsync(T entity, CancellationToken token = default) =>
        repository.UpdateAsync(entity, token);

    public Task DeleteAsync(T entity, CancellationToken token = default) =>
        repository.DeleteAsync(entity, token);

    public Task SaveChangesAsync(CancellationToken token = default) =>
        repository.SaveChangesAsync(token);
}