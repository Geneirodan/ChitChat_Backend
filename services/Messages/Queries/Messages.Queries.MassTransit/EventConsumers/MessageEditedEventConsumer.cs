using MassTransit;
using Messages.Contracts;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Shared.Abstractions;

namespace Messages.Queries.MassTransit.EventConsumers;

public sealed class MessageEditedEventConsumer(IMessageRepository repository) : IConsumer<MessageEditedEvent>
{

    public async Task Consume(ConsumeContext<MessageEditedEvent> context)
    {
        var (id, content, _, _) = context.Message;
        
        var specification = new GetByIdSpecification<Message>(id);
        var message = await repository.FindAsync(specification).ConfigureAwait(false);

        if (message is null) return;

        message.Content = content;
        
        await repository.UpdateAsync(message).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
    }
}