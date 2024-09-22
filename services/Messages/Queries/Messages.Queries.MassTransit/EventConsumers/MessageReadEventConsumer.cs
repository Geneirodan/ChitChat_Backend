using MassTransit;
using Messages.Contracts;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Shared.Abstractions;

namespace Messages.Queries.MassTransit.EventConsumers;

public sealed class MessageReadEventConsumer(IMessageRepository repository) : IConsumer<MessageReadEvent>
{

    public async Task Consume(ConsumeContext<MessageReadEvent> context)
    {
        var specification = new GetByIdSpecification<Message>(context.Message.Id);
        var message = await repository.FindAsync(specification).ConfigureAwait(false);

        if (message is null) return;

        message.Read();

        await repository.UpdateAsync(message).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
    }
}