using Ardalis.Specification;
using Mapster;

namespace Shared.Abstractions;

public interface IGetByIdSpecification<T> : ISpecification<T>
{
    public Guid Id { get; }
}

public interface IGetByIdSpecification<T, TResult> : IGetByIdSpecification<T>;

public sealed class GetByIdSpecification<T> : SingleResultSpecification<T>, IGetByIdSpecification<T>
    where T : IEntity<Guid>
{
    public Guid Id { get; }

    public GetByIdSpecification(Guid id)
    {
        Id = id;
        Query.Where(x => id.Equals(x.Id));
    }
}

public sealed class GetByIdSpecification<T, TResult>
    : SingleResultSpecification<T, TResult>, IGetByIdSpecification<T, TResult>
    where T : IEntity<Guid>
{
    public Guid Id { get; }

    public GetByIdSpecification(Guid id)
    {
        Id = id;
        Query.Where(x => id.Equals(x.Id));
        Query.Select(x => x.Adapt<TResult>());
    }
}