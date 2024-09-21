using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;

namespace Shared.EntityFramework;

public class EntityFrameworkRepository<TEntity>(DbContext context)
    : IRepository<TEntity> where TEntity : class, Abstractions.IEntity<Guid>
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public Task<TResult?> FindAsync<TResult>(
        ISingleResultSpecification<TEntity, TResult> specification,
        CancellationToken token = default
    ) => _dbSet.WithSpecification(specification).FirstOrDefaultAsync(token);

    public Task<TEntity?> FindAsync(
        ISingleResultSpecification<TEntity> spec,
        CancellationToken token = default
    ) => _dbSet.WithSpecification(spec).FirstOrDefaultAsync(token);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}