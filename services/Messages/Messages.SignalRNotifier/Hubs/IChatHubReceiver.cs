using Messages.Contracts;
using TypedSignalR.Client;

namespace Messages.SignalRNotifier.Hubs;

[Receiver]
public interface IChatHubReceiver
{
    Task MessageCreatedEvent(MessageCreatedEvent @event);
    Task MessageDeletedEvent(MessageDeletedEvent @event);
    Task MessageEditedEvent(MessageEditedEvent @event);
    Task MessageReadEvent(MessageReadEvent @event);
};
