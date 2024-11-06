namespace Messages.Contracts;


[Serializable]
public sealed record MessageCreatedEvent(Guid Id, string Content, DateTime SendTime, Guid SenderId, Guid ReceiverId)
    : IMessageEvent;