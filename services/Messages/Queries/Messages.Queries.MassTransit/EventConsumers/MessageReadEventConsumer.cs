using MassTransit;
using Messages.Contracts;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.MassTransit.EventConsumers;

public sealed class MessageReadEventConsumer(IMessageRepository repository, IDistributedCache? cache) : IConsumer<MessageReadEvent>
{

    public async Task Consume(ConsumeContext<MessageReadEvent> context)
    {
        var specification = new GetByIdSpecification<Message>(context.Message.Id);
        var message = await repository.FindAsync(specification).ConfigureAwait(false);

        if (message is null) return;

        message.Read();

        await repository.UpdateAsync(message, context.CancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
        if(cache is not null)
            await cache.RefreshAsync($"Messages-{message.Id}", context.CancellationToken).ConfigureAwait(false);
    }
}