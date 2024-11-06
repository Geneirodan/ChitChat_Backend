namespace Messages.Contracts;


[Serializable]
public sealed record MessageDeletedEvent(Guid Id, Guid SenderId, Guid ReceiverId) 
    : IMessageEvent;