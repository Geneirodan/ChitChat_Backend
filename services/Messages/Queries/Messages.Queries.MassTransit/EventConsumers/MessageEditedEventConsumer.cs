using MassTransit;
using Messages.Contracts;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.MassTransit.EventConsumers;

public sealed class MessageEditedEventConsumer(IMessageRepository repository, IDistributedCache? cache) : IConsumer<MessageEditedEvent>
{

    public async Task Consume(ConsumeContext<MessageEditedEvent> context)
    {
        var (id, content, _, _) = context.Message;
        
        var specification = new GetByIdSpecification<Message>(id);
        var message = await repository.FindAsync(specification).ConfigureAwait(false);

        if (message is null) return;

        message.Content = content;
        
        await repository.UpdateAsync(message, context.CancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);
        if(cache is not null)
            await cache.RefreshAsync($"Messages-{message.Id}", context.CancellationToken).ConfigureAwait(false);
    }
}