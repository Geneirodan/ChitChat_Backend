using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;

namespace Shared.EntityFramework;

public class EntityFrameworkReadRepository<TEntity>(DbContext context)
    : IReadRepository<TEntity> where TEntity : class, Abstractions.IEntity<Guid>
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TResult?> GetAsync<TResult>(
        ISingleResultSpecification<TEntity, TResult> spec,
        CancellationToken token = default) =>
        await _dbSet.WithSpecification(spec).FirstOrDefaultAsync(token);

    public async Task<TEntity?> GetAsync(
        ISingleResultSpecification<TEntity> spec,
        CancellationToken token = default) =>
        await _dbSet.WithSpecification(spec).FirstOrDefaultAsync(token);

    public async Task<IEnumerable<TResult?>> GetAllAsync<TResult>(
        ISpecification<TEntity, TResult> spec,
        CancellationToken token = default) =>
        await _dbSet.WithSpecification(spec).ToListAsync(token);

    public async Task<IEnumerable<TEntity?>> GetAllAsync(
        ISpecification<TEntity> spec,
        CancellationToken token = default) =>
        await _dbSet.WithSpecification(spec).ToListAsync(token);

    public async Task<PaginatedList<TResult>> GetAllPaginatedAsync<TResult>(
        ISpecification<TEntity, TResult> spec,
        int page, int perPage,
        CancellationToken token = default)
    {
        var query = _dbSet.WithSpecification(spec);

        var items = await query.Skip((page - 1) * perPage).Take(perPage).ToArrayAsync(token);
        var count = await query.CountAsync(token);
        return new PaginatedList<TResult>(items, page, perPage, count);
    }

    public async Task<PaginatedList<TEntity>> GetAllPaginatedAsync(
        ISpecification<TEntity> spec,
        int page, int perPage,
        CancellationToken token = default)
    {
        var query = _dbSet.WithSpecification(spec);

        var items = await query.Skip((page - 1) * perPage).Take(perPage).ToArrayAsync(token);
        var count = await query.CountAsync(token);
        return new PaginatedList<TEntity>(items, page, perPage, count);
    }
}