using MassTransit;
using Messages.Commands.Domain;
using Messages.Contracts;

namespace Messages.Commands.MassTransit.EventHandlers;

public sealed class MessageReadEventHandler(IPublishEndpoint endpoint)
    : IntegrationEventHandler<Message.ReadEvent, MessageReadEvent>(endpoint);