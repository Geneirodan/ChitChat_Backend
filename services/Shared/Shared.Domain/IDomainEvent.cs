using MediatR;

namespace Shared.Domain;

public interface IDomainEvent : INotification
{
    public Guid Id { get; }
}