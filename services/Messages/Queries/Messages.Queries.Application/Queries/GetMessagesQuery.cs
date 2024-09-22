using Ardalis.Result;
using Ardalis.Specification;
using MediatR;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Abstractions;

namespace Messages.Queries.Application;

public sealed record GetMessagesQuery(int Page, int PerPage, Guid ReceiverId, string Search)
    : IRequest<Result<PaginatedList<Message>>>
{
    public sealed class Specification : Specification<Message>
    {
        public Specification(string searchTerm, Guid senderId, Guid receiverId)
        {
            Query.Where(x => x.SenderId == senderId);
            Query.Where(x => x.ReceiverId == receiverId);
            if (!string.IsNullOrWhiteSpace(searchTerm))
                Query.Search(x => x.Content, searchTerm);
        }
    }

    public sealed class Handler(IMessageReadRepository repository, IUser user)
        : IRequestHandler<GetMessagesQuery, Result<PaginatedList<Message>>>
    {
        public async Task<Result<PaginatedList<Message>>> Handle(
            GetMessagesQuery request,
            CancellationToken cancellationToken
        )
        {
            if (user is { Id: null })
                return Result.Unauthorized();

            var (page, perPage, receiverId, search) = request;

            var filter = new Specification(search, user.Id.Value, receiverId);

            return await repository.GetAllPaginatedAsync(filter, page, perPage, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}