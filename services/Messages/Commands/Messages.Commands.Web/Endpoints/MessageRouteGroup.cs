using Ardalis.Result.AspNetCore;
using MediatR;
using Messages.Commands.Application.Commands;
using Messages.Commands.Web.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Messages.Commands.Web.Endpoints;

internal static class MessageRouteGroup
{
    private const string RouteGroupName = "messages";
    internal static void MapMessages(this IEndpointRouteBuilder app, string prefix)
    {
        var group = app.MapGroup(prefix).WithTags(RouteGroupName).RequireAuthorization();
        group.MapPost("/", AddMessage);
        group.MapPut("/{id:guid}", EditMessage);
        group.MapPatch("/{id:guid}/read", ReadMessage);
        group.MapDelete("/{id:guid}", DeleteMessage);
    }

    private static async Task<IResult> AddMessage(
        [FromBody] AddMessageRequest request,
        [FromServices] IMediator mediator)
    {
        var (content, dateTime, receiverId) = request;
        var command = new AddMessageCommand(content, dateTime, receiverId);
        var result = await mediator.Send(command).ConfigureAwait(false);
        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> EditMessage(
        [FromRoute] Guid id,
        [FromBody] EditMessageRequest request,
        [FromServices] IMediator mediator)
    {
        var command = new EditMessageCommand(id, request.Content);
        var result = await mediator.Send(command).ConfigureAwait(false);
        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> DeleteMessage(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var command = new DeleteMessageCommand(id);
        var result = await mediator.Send(command).ConfigureAwait(false);
        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ReadMessage(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var command = new ReadMessageCommand(id);
        var result = await mediator.Send(command).ConfigureAwait(false);
        return result.ToMinimalApiResult();
    }
}