using MassTransit;
using Messages.Commands.MassTransit.EventHandlers;
using Messages.Contracts;
using static Messages.Commands.Application.Tests.TestData;

namespace Messages.Commands.Application.Tests.EventHandlers;

[TestSubject(typeof(IntegrationEventHandler<,>))]
public class IntegrationEventHandlerTest
{
    private readonly Mock<IPublishEndpoint> _endpoint = new();

    [Fact]
    public async Task HandleMessageCreatedEvent()
    {
        var domainEvent = new Message.CreatedEvent(
            Id: ValidIds[0],
            Content: "SomeContent",
            SendTime: DateTime.MinValue,
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        var integrationEvent = new MessageCreatedEvent(
            Id: ValidIds[0],
            Content: "SomeContent",
            SendTime: DateTime.MinValue,
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        await new MessageCreatedEventHandler(_endpoint.Object).Handle(domainEvent);
        _endpoint.Verify(
            x => x.Publish(
                It.Is<MessageCreatedEvent>(value => value.Equals(integrationEvent)),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public async Task HandleMessageDeletedEvent()
    {
        var domainEvent = new Message.DeletedEvent(
            Id: ValidIds[0],
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        var integrationEvent = new MessageDeletedEvent(
            Id: ValidIds[0],
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        await new MessageDeletedEventHandler(_endpoint.Object).Handle(domainEvent);
        _endpoint.Verify(
            x => x.Publish(
                It.Is<MessageDeletedEvent>(value => value.Equals(integrationEvent)),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public async Task HandleMessageEditedEvent()
    {
        var domainEvent = new Message.EditedEvent(
            Id: ValidIds[0],
            Content: "SomeContent",
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        var integrationEvent = new MessageEditedEvent(
            Id: ValidIds[0],
            Content: "SomeContent",
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        await new MessageEditedEventHandler(_endpoint.Object).Handle(domainEvent);
        _endpoint.Verify(
            x => x.Publish(
                It.Is<MessageEditedEvent>(value => value.Equals(integrationEvent)),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public async Task HandleMessageReadEvent()
    {
        var domainEvent = new Message.ReadEvent(
            Id: ValidIds[0],
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        var integrationEvent = new MessageReadEvent(
            Id: ValidIds[0],
            SenderId: ValidIds[1],
            ReceiverId: ValidIds[2]
        );
        await new MessageReadEventHandler(_endpoint.Object).Handle(domainEvent);
        _endpoint.Verify(
            x => x.Publish(
                It.Is<MessageReadEvent>(value => value.Equals(integrationEvent)),
                It.IsAny<CancellationToken>()
            ),
            Times.Once()
        );
    }
}