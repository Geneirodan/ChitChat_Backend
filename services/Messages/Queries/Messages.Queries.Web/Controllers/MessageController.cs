using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using MediatR;
using Messages.Queries.Application;
using Messages.Queries.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Shared.Abstractions;

namespace Messages.Queries.Web.Controllers;

internal sealed class MessageController(ISender sender) : GraphController
{
    [Authorize]
    [QueryRoot("messages")]
    public async Task<PaginatedList<Message>> GetMessages(
        Guid receiverId,
        string search = "",
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetMessagesQuery(page, perPage, receiverId, search);
        return await sender.Send(query, cancellationToken).ConfigureAwait(false);
    }
}