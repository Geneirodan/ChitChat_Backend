namespace Messages.Contracts;


[Serializable]
public sealed record MessageReadEvent(Guid Id, Guid SenderId, Guid ReceiverId) 
    : IMessageEvent;