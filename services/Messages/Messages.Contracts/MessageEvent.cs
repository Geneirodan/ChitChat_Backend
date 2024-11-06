using Shared.Domain;

namespace Messages.Contracts;

public interface IMessageEvent : IIntegrationEvent
{
    public Guid SenderId { get;  }
    public Guid ReceiverId { get; }
}