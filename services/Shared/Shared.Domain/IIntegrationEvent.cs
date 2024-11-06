using MediatR;

namespace Shared.Domain;

public interface IIntegrationEvent: INotification
{
    public Guid Id { get; } 
}