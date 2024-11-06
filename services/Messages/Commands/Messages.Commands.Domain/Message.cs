using Shared.Domain;

namespace Messages.Commands.Domain;

public class Message : Aggregate
{
    public string Content { get; protected set; } = null!;

    public bool IsRead { get; protected set; }

    public DateTime SendTime { get; protected set; }

    public Guid SenderId { get; protected set; }

    public Guid ReceiverId { get; protected set; }

    public static (Message Message, CreatedEvent Event) CreateInstance(Guid id, string content,
        DateTime sendTime, Guid senderId, Guid receiverId)
    {
        var message = new Message
        {
            Id = id,
            Content = content,
            SendTime = sendTime,
            SenderId = senderId,
            ReceiverId = receiverId
        };
        var @event = new CreatedEvent(
            message.Id,
            message.Content,
            message.SendTime,
            message.SenderId,
            message.ReceiverId
        );
        message.Enqueue(@event);
        return (message, @event);
    }


    public EditedEvent Edit(string content)
    {
        Content = content;
        var @event = new EditedEvent(Id, Content, SenderId, ReceiverId);
        Enqueue(@event);
        return @event;
    }


    public ReadEvent Read()
    {
        IsRead = true;
        var @event = new ReadEvent(Id, SenderId, ReceiverId);
        Enqueue(@event);
        return @event;
    }


    public DeletedEvent Delete()
    {
        IsDeleted = true;
        var @event = new DeletedEvent(Id, SenderId, ReceiverId);
        Enqueue(@event);
        return @event;
    }

    [Serializable]
    public sealed record CreatedEvent(
        Guid Id,
        string Content,
        DateTime SendTime,
        Guid SenderId,
        Guid ReceiverId
    ) : IMessageEvent;

    [Serializable]
    public sealed record EditedEvent(Guid Id, string Content, Guid SenderId, Guid ReceiverId)
        : IMessageEvent;

    [Serializable]
    public sealed record ReadEvent(Guid Id, Guid SenderId, Guid ReceiverId) 
        : IMessageEvent;

    [Serializable]
    public sealed record DeletedEvent(Guid Id, Guid SenderId, Guid ReceiverId) 
        : IMessageEvent;

    private interface IMessageEvent : IDomainEvent
    {
        public Guid SenderId { get;}
        public Guid ReceiverId { get;  }

    }
}