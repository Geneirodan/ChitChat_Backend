using MassTransit;
using Messages.Contracts;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Shared.Abstractions;

namespace Messages.Queries.MassTransit.EventConsumers;

public sealed class MessageDeletedEventConsumer(IMessageRepository repository) : IConsumer<MessageDeletedEvent>
{
    public async Task Consume(ConsumeContext<MessageDeletedEvent> context)
    {
        var specification = new GetByIdSpecification<Message>(context.Message.Id);
        var response = await repository.FindAsync(specification).ConfigureAwait(false);

        if (response is null) return;

        await repository.DeleteAsync(response).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
    }
}