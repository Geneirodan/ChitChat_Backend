using System.Text.Json;
using Ardalis.Specification;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Shared.Redis;

public abstract class CachedRepositoryBase<T>(IDistributedCache cache, ILogger logger)
{
    protected async Task<TResult> InvokeWithCacheAsync<TResult>(
        ISpecification<T> specification,
        Func<Task<TResult>> initialFunc,
        CancellationToken cancellationToken
    )
    {
        if (!specification.CacheEnabled || specification.CacheKey is null)
            return await initialFunc.Invoke().ConfigureAwait(false);

        var value = await cache.GetStringAsync(specification.CacheKey, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(value))
        {
            logger.LogInformation("Cache Hit: {key}", specification.CacheKey);
            return JsonSerializer.Deserialize<TResult>(value)!;
        }
        logger.LogInformation("Cache Miss: {key}", specification.CacheKey);

        var entity = await initialFunc.Invoke().ConfigureAwait(false);

        if (entity is not null)
            await cache.SetStringAsync(specification.CacheKey, JsonSerializer.Serialize(entity), cancellationToken).ConfigureAwait(false);

        return entity;
    }
}