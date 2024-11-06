using MassTransit;
using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Utils;
using Messages.Contracts;
using Messages.SignalRNotifier.Hubs;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Messages.SignalRNotifier.EventConsumers;

public abstract class MessageEventConsumer<TEvent>(IPublishEndpoint endpoint)
    : IConsumer<TEvent>
    where TEvent : class, IMessageEvent
{
    private readonly IHubProtocol[] _protocols = [new JsonHubProtocol()];

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var message = context.Message;
        var values = new EndpointMessage(
            UserId: message.ReceiverId.ToString(),
            Messages: _protocols.ToProtocolDictionary(typeof(TEvent).Name, [message])
        );
        await endpoint.Publish(values).ConfigureAwait(false);
    }

    private record EndpointMessage(string UserId, IReadOnlyDictionary<string, byte[]> Messages) : User<ChatHub>;
}