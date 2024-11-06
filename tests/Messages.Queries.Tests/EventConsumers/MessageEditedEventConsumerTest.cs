﻿using JetBrains.Annotations;
using MassTransit;
using Messages.Contracts;
using Messages.Queries.MassTransit.EventConsumers;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.Tests.EventConsumers;

[TestSubject(typeof(MessageEditedEventConsumer))]
public class MessageEditedEventConsumerTest
{
    private readonly Mock<IMessageRepository> _repository = new();
    private readonly Mock<IDistributedCache> _cache = new();
    private readonly MessageEditedEventConsumer _consumer;

    public MessageEditedEventConsumerTest() =>
        _consumer = new MessageEditedEventConsumer(_repository.Object, _cache.Object);

    [Theory, MemberData(nameof(ConsumeData))]
    public async Task Consume(MessageEditedEvent @event, bool exists)
    {
        var context = new Mock<ConsumeContext<MessageEditedEvent>>();
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
        _repository.Verify(x => x.UpdateAsync(It.IsAny<Message>(), default), times);
        _repository.Verify(x => x.SaveChangesAsync(default), times);
    }

    public static TheoryData<MessageEditedEvent, bool> ConsumeData => new()
    {
        {
            new MessageEditedEvent(Guid.NewGuid(), string.Empty, Guid.NewGuid(), Guid.NewGuid()),
            true
        },
        {
            new MessageEditedEvent(Guid.NewGuid(), string.Empty, Guid.NewGuid(), Guid.NewGuid()),
            false
        },
    };
}