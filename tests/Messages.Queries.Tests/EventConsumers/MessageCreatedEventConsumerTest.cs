using JetBrains.Annotations;
using MassTransit;
using Messages.Contracts;
using Messages.Queries.MassTransit.EventConsumers;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Moq;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.Tests.EventConsumers;

[TestSubject(typeof(MessageCreatedEventConsumer))]
public class MessageCreatedEventConsumerTest
{
    private readonly Mock<IMessageRepository> _repository = new();
    private readonly MessageCreatedEventConsumer _consumer;

    public MessageCreatedEventConsumerTest() => _consumer = new MessageCreatedEventConsumer(_repository.Object);

    [Theory, MemberData(nameof(ConsumeData))]
    public async Task Consume(MessageCreatedEvent @event, bool exists)
    {
        var context = new Mock<ConsumeContext<MessageCreatedEvent>>();
        context.Setup(x => x.Message).Returns(@event);
        _repository.Setup(x =>
                x.FindAsync(It.Is<GetByIdSpecification<Message>>(y => y.Id == @event.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists ? new Message() : null);

        await _consumer.Consume(context.Object);
        _repository.Verify(
            x => x.FindAsync(It.IsAny<GetByIdSpecification<Message>>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
        var times = exists ? Times.Never() : Times.Once();
        _repository.Verify(x => x.AddAsync(It.IsAny<Message>(), default), times);
        _repository.Verify(x => x.SaveChangesAsync(default), times);
    }

    public static TheoryData<MessageCreatedEvent, bool> ConsumeData => new()
    {
        {
            new MessageCreatedEvent(Guid.NewGuid(), string.Empty, DateTime.Now, Guid.NewGuid(), Guid.NewGuid()),
            true
        },
        {
            new MessageCreatedEvent(Guid.NewGuid(), string.Empty, DateTime.Now, Guid.NewGuid(), Guid.NewGuid()),
            false
        },
    };
}