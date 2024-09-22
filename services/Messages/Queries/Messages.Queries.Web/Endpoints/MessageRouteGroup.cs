using Ardalis.Result.AspNetCore;
using Mapster;
using MediatR;
using Messages.Queries.Application;
using Messages.Queries.Web.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Messages.Queries.Web.Endpoints;

internal static class MessageRouteGroup
{
    private const string RouteGroupName = "messages";
    public static void MapMessages(this IEndpointRouteBuilder app, string prefix) =>
        app.MapGet(prefix, GetMessages).WithTags(RouteGroupName).RequireAuthorization();

    private static async Task<IResult> GetMessages(
        [AsParameters] GetMessagesRequest request,
        [FromServices] ISender mediator,
        [FromServices] IDistributedCache cache
    )
    {
        var (receiverId, search, page, perPage) = request;
        var query = request.Adapt<GetMessagesQuery>();
        var result = await mediator.Send(query).ConfigureAwait(false);
        return result.ToMinimalApiResult();
    }
}