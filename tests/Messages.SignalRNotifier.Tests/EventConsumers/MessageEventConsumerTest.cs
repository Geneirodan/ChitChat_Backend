using JetBrains.Annotations;
using MassTransit;
using MassTransit.SignalR.Contracts;
using Messages.Contracts;
using Messages.SignalRNotifier.EventConsumers;
using Messages.SignalRNotifier.Hubs;
using Moq;

namespace Messages.SignalRNotifier.Tests.EventConsumers;

[TestSubject(typeof(MessageEventConsumer<>))]
public class MessageEventConsumerTest
{
    private readonly Mock<IPublishEndpoint> _endpoint = new();

    [Fact]
    public async Task ConsumeMessageCreatedEvent()
    {
        var @event =
            new MessageCreatedEvent(Guid.NewGuid(), string.Empty, DateTime.Now, Guid.NewGuid(), Guid.NewGuid());
        var context = new Mock<ConsumeContext<MessageCreatedEvent>>();
        context.Setup(x => x.Message).Returns(@event);

        await new MessageCreatedEventConsumer(_endpoint.Object).Consume(context.Object);
        
        _endpoint.Verify(
            x => x.Publish(
                It.Is<User<ChatHub>>(y => y.UserId == @event.ReceiverId.ToString()),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public async Task ConsumeMessageDeletedEvent()
    {
        var @event = new MessageDeletedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var context = new Mock<ConsumeContext<MessageDeletedEvent>>();
        context.Setup(x => x.Message).Returns(@event);
        
        await new MessageDeletedEventConsumer(_endpoint.Object).Consume(context.Object);
        
        _endpoint.Verify(
            x => x.Publish(
                It.Is<User<ChatHub>>(y => y.UserId == @event.ReceiverId.ToString()),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public async Task ConsumeMessageEditedEvent()
    {
        var @event = new MessageEditedEvent(Guid.NewGuid(), string.Empty, Guid.NewGuid(), Guid.NewGuid());
        var context = new Mock<ConsumeContext<MessageEditedEvent>>();
        context.Setup(x => x.Message).Returns(@event);
        
        await new MessageEditedEventConsumer(_endpoint.Object).Consume(context.Object);
        
        _endpoint.Verify(
            x => x.Publish(
                It.Is<User<ChatHub>>(y => y.UserId == @event.ReceiverId.ToString()),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public async Task ConsumeMessageReadEvent()
    {
        var @event = new MessageReadEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var context = new Mock<ConsumeContext<MessageReadEvent>>();
        context.Setup(x => x.Message).Returns(@event);
        
        await new MessageReadEventConsumer(_endpoint.Object).Consume(context.Object);
        
        _endpoint.Verify(
            x => x.Publish(
                It.Is<User<ChatHub>>(y => y.UserId == @event.ReceiverId.ToString()),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }
}