using Ardalis.Result;
using Ardalis.Specification;
using MediatR;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.Application.Queries;

public sealed record GetMessageByIdQuery(Guid Id)
    : IRequest<Result<Message>>
{
    private sealed class Specification : GetByIdSpecification<Message>
    {
        public Specification(Guid id) : base(id) => Query.EnableCache("Messages", id);
    }

    public sealed class Handler(IMessageReadRepository repository, IUser user)
        : IRequestHandler<GetMessageByIdQuery, Result<Message>>
    {
        public async Task<Result<Message>> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            if (user is { Id: null })
                return Result.Unauthorized();

            var filter = new Specification(request.Id);
            var message = await repository.GetAsync(filter, cancellationToken).ConfigureAwait(false);
            return message is not null ? message : Result.NotFound();
        }
    }
}