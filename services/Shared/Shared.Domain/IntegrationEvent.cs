using MediatR;

namespace Shared.Domain;

public abstract record IntegrationEvent(Guid Id): INotification;