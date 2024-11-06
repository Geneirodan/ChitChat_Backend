using Mapster;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain;

namespace Messages.Commands.MassTransit.EventHandlers;

public abstract class IntegrationEventHandler<TDomainEvent, TIntegrationEvent>(IPublishEndpoint endpoint)
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : INotification
    where TIntegrationEvent : class, IIntegrationEvent
{
    public async Task Handle(TDomainEvent request, CancellationToken cancellationToken = default)
    {
        var integrationEvent = request.Adapt<TIntegrationEvent>();
        await endpoint.Publish(integrationEvent, cancellationToken).ConfigureAwait(false);
    }
}