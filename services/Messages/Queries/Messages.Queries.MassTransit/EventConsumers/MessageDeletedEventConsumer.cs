using MassTransit;
using Messages.Contracts;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.MassTransit.EventConsumers;

public sealed class MessageDeletedEventConsumer(IMessageRepository repository, IDistributedCache? cache) : IConsumer<MessageDeletedEvent>
{
    public async Task Consume(ConsumeContext<MessageDeletedEvent> context)
    {
        var id = context.Message.Id;
        var specification = new GetByIdSpecification<Message>(id);
        var response = await repository.FindAsync(specification).ConfigureAwait(false);

        if (response is null) return;

        await repository.DeleteAsync(response, context.CancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);
        if(cache is not null)
            await cache.RefreshAsync($"Messages-{id}", context.CancellationToken).ConfigureAwait(false);
    }
}