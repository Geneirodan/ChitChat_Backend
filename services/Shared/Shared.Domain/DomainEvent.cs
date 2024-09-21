using MediatR;

namespace Shared.Domain;

public abstract record DomainEvent(Guid Id) : INotification;