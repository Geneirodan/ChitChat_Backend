using Ardalis.Specification;
using Mapster;

namespace Shared.Abstractions.Specifications;

public interface IGetByIdSpecification<T> : ISpecification<T>
{
    public Guid Id { get; }
}

public interface IGetByIdSpecification<T, TResult> : IGetByIdSpecification<T>;

public class GetByIdSpecification<T> : SingleResultSpecification<T>, IGetByIdSpecification<T>
    where T : class, IEntity<Guid>
{
    public Guid Id { get; }

    public GetByIdSpecification(Guid id)
    {
        Id = id;
        Query.Where(x => id.Equals(x.Id));
    }
}

public class GetByIdSpecification<T, TResult>
    : SingleResultSpecification<T, TResult>, IGetByIdSpecification<T, TResult>
    where T : class, IEntity<Guid>
{
    public Guid Id { get; }

    public GetByIdSpecification(Guid id)
    {
        Id = id;
        Query.Where(x => id.Equals(x.Id));
        Query.Select(x => x.Adapt<TResult>());
    }
}