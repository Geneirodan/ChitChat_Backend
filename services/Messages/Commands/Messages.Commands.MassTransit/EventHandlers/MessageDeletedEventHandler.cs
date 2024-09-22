using MassTransit;
using Messages.Commands.Domain;
using Messages.Contracts;

namespace Messages.Commands.MassTransit.EventHandlers;

public sealed class MessageDeletedEventHandler(IPublishEndpoint endpoint)
    : IntegrationEventHandler<Message.DeletedEvent, MessageDeletedEvent>(endpoint);