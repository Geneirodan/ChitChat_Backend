using MassTransit;
using Messages.Commands.Domain;
using Messages.Contracts;

namespace Messages.Commands.MassTransit.EventHandlers;

public sealed class MessageEditedEventHandler(IPublishEndpoint endpoint)
    : IntegrationEventHandler<Message.EditedEvent, MessageEditedEvent>(endpoint);