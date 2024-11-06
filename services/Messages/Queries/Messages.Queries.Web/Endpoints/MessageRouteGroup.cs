using Ardalis.Result.AspNetCore;
using Mapster;
using MediatR;
using Messages.Queries.Application;
using Messages.Queries.Application.Queries;
using Messages.Queries.Web.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Messages.Queries.Web.Endpoints;

internal static class MessageRouteGroup
{
    private const string RouteGroupName = "messages";

    public static void MapMessages(this IEndpointRouteBuilder app, string prefix)
    {
        var group = app.MapGroup(prefix).WithTags(RouteGroupName).RequireAuthorization();
        group.MapGet("/", GetMessages);
        group.MapGet("/{id:guid}", GetMessageById);
    }

    private static async Task<IResult> GetMessageById(
        [FromRoute] Guid id,
        [FromServices] ISender mediator
    )
    {
        var query = new GetMessageByIdQuery(id);
        var result = await mediator.Send(query).ConfigureAwait(false);
        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> GetMessages(
        [AsParameters] GetMessagesRequest request,
        [FromServices] ISender mediator
    )
    {
        var query = request.Adapt<GetMessagesQuery>();
        var result = await mediator.Send(query).ConfigureAwait(false);
        return result.ToMinimalApiResult();
    }
}