using MassTransit;
using Messages.Contracts;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.MassTransit.EventConsumers;

public sealed class MessageCreatedEventConsumer(IMessageRepository repository, IDistributedCache? cache)
    : IConsumer<MessageCreatedEvent>
{
    public async Task Consume(ConsumeContext<MessageCreatedEvent> context)
    {
        var (id, content, sendTime, senderId, receiverId) = context.Message;

        var specification = new GetByIdSpecification<Message>(id);
        var response = await repository.FindAsync(specification, context.CancellationToken).ConfigureAwait(false);

        if (response is not null) return;

        var message = new Message
        {
            Id = id,
            Content = content,
            SendTime = sendTime,
            SenderId = senderId,
            ReceiverId = receiverId
        };

        await repository.AddAsync(message, context.CancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);
        if(cache is not null)
            await cache.RefreshAsync($"Messages-{message.Id}", context.CancellationToken).ConfigureAwait(false);
    }
}