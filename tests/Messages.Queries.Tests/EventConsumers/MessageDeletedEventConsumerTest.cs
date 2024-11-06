using JetBrains.Annotations;
using MassTransit;
using Messages.Contracts;
using Messages.Queries.MassTransit.EventConsumers;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.Tests.EventConsumers;

[TestSubject(typeof(MessageDeletedEventConsumer))]
public class MessageDeletedEventConsumerTest
{
    private readonly Mock<IMessageRepository> _repository = new();
    private readonly Mock<IDistributedCache> _cache = new();
    private readonly MessageDeletedEventConsumer _consumer;

    public MessageDeletedEventConsumerTest() =>
        _consumer = new MessageDeletedEventConsumer(_repository.Object, _cache.Object);

    [Theory, MemberData(nameof(ConsumeData))]
    public async Task Consume(MessageDeletedEvent @event, bool exists)
    {
        var context = new Mock<ConsumeContext<MessageDeletedEvent>>();
        context.Setup(x => x.Message).Returns(@event);
        var message = exists ? new Message() : null;
        _repository.Setup(x =>
                x.FindAsync(It.Is<GetByIdSpecification<Message>>(y => y.Id == @event.Id),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);

        await _consumer.Consume(context.Object);
        _repository.Verify(
            x => x.FindAsync(It.IsAny<GetByIdSpecification<Message>>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
        var times = exists ? Times.Once() : Times.Never();
        _repository.Verify(x => x.DeleteAsync(It.IsAny<Message>(), default), times);
        _repository.Verify(x => x.SaveChangesAsync(default), times);
    }

    public static TheoryData<MessageDeletedEvent, bool> ConsumeData => new()
    {
        {
            new MessageDeletedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            true
        },
        {
            new MessageDeletedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            false
        },
    };
}