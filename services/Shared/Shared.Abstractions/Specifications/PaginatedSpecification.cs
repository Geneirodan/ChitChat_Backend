using Ardalis.Specification;
using Mapster;

namespace Shared.Abstractions.Specifications;

public interface IPaginatedSpecification<T> : ISpecification<T>
{
    public int Page { get; }
    public int PageSize { get; }
}

public interface IPaginatedSpecification<T, TResult> : IPaginatedSpecification<T>, ISpecification<T, TResult>;

public class PaginatedSpecification<T> : Specification<T>, IPaginatedSpecification<T>
    where T : IEntity<Guid>
{

    public PaginatedSpecification(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
        Query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public int Page { get; }
    public int PageSize { get; }
}

public class PaginatedSpecification<T, TResult>
    : Specification<T, TResult>, IPaginatedSpecification<T, TResult>
    where T : IEntity<Guid>
{

    public PaginatedSpecification(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
        Query.Skip((page - 1) * pageSize).Take(pageSize);
        Query.Select(x => x.Adapt<TResult>());
    }

    public int Page { get; }
    public int PageSize { get; }
}