namespace Messages.Commands.Persistence.Aggregates;

public sealed class Message : Domain.Message
{
    public void Apply(CreatedEvent @event) => (Id, Content, SendTime, SenderId, ReceiverId) = @event;
    public void Apply(EditedEvent @event) => Content = @event.Content;
    public void Apply(ReadEvent _) => IsRead = true;
    public void Apply(DeletedEvent _) => IsDeleted = true;
}