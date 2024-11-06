using JetBrains.Annotations;
using MassTransit;
using Messages.Contracts;
using Messages.Queries.MassTransit.EventConsumers;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Moq;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.Tests.EventConsumers;

[TestSubject(typeof(MessageReadEventConsumer))]
public class MessageReadEventConsumerTest
{
    private readonly Mock<IMessageRepository> _repository = new();
    private readonly MessageReadEventConsumer _consumer;

    public MessageReadEventConsumerTest() => _consumer = new MessageReadEventConsumer(_repository.Object);
    
    [Theory, MemberData(nameof(ConsumeData))]
    public async Task Consume(MessageReadEvent @event, bool exists)
    {
        var context = new Mock<ConsumeContext<MessageReadEvent>>();
        context.Setup(x => x.Message).Returns(@event);
        var message = exists ? new Message() : null;
        _repository.Setup(x =>
                x.FindAsync(It.Is<GetByIdSpecification<Message>>(y => y.Id == @event.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        
        await _consumer.Consume(context.Object);
        _repository.Verify(
            x => x.FindAsync(It.IsAny<GetByIdSpecification<Message>>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
        var times = exists ? Times.Once() : Times.Never();
        _repository.Verify(x => x.UpdateAsync(It.IsAny<Message>(), default), times);
        _repository.Verify(x => x.SaveChangesAsync(default), times);
    }
    
    public static TheoryData<MessageReadEvent, bool> ConsumeData => new()
    {
        {
            new MessageReadEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            true
        },
        {
            new MessageReadEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            false
        },
    };
}