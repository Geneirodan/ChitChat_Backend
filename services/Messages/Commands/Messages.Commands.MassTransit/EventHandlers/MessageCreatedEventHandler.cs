using MassTransit;
using Messages.Commands.Domain;
using Messages.Contracts;

namespace Messages.Commands.MassTransit.EventHandlers;

public sealed class MessageCreatedEventHandler(IPublishEndpoint endpoint)
    : IntegrationEventHandler<Message.CreatedEvent, MessageCreatedEvent>(endpoint);