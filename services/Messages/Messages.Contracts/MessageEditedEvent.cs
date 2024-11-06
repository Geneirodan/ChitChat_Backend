namespace Messages.Contracts;


[Serializable]
public sealed record MessageEditedEvent(Guid Id, string Content, Guid SenderId, Guid ReceiverId) 
    : IMessageEvent;