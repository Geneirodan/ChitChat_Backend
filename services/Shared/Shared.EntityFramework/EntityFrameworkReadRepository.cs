using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

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
        IPaginatedSpecification<TEntity, TResult> spec,
        CancellationToken token = default)
    {
        var items = await _dbSet.WithSpecification(spec).ToArrayAsync(token);
        var count = await SpecificationEvaluator.Default.GetQuery(_dbSet, spec, true).CountAsync(token);
        return new PaginatedList<TResult>(items, spec.Page, spec.PageSize, count);
    }

    public async Task<PaginatedList<TEntity>> GetAllPaginatedAsync(
        IPaginatedSpecification<TEntity> spec,
        CancellationToken token = default)
    {
        var items = await _dbSet.WithSpecification(spec).ToArrayAsync(token);
        var count = await SpecificationEvaluator.Default.GetQuery(_dbSet, spec, true).CountAsync(token);
        return new PaginatedList<TEntity>(items, spec.Page, spec.PageSize, count);
    }
}