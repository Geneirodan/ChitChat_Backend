using Ardalis.Result;
using Ardalis.Specification;
using MediatR;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.Application.Queries;

public sealed record GetMessagesQuery(int Page, int PerPage, Guid ReceiverId, string Search)
    : IRequest<Result<PaginatedList<Message>>>
{
    private sealed class Specification : PaginatedSpecification<Message>
    {
        public Specification(string searchTerm, Guid senderId, Guid receiverId, int page, int perPage) : base(page, perPage)
        {
            Query.Where(x => x.SenderId == senderId);
            Query.Where(x => x.ReceiverId == receiverId);
            if (!string.IsNullOrWhiteSpace(searchTerm))
                Query.Search(x => x.Content, searchTerm);
            Query.EnableCache("Messages", senderId, receiverId, searchTerm, page, perPage);
            Query.OrderByDescending(x => x.SendTime);
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

            var filter = new Specification(search, user.Id.Value, receiverId, page, perPage);
            return await repository.GetAllPaginatedAsync(filter, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}