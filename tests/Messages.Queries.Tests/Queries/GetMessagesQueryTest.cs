using Ardalis.Result;
using FluentAssertions;
using JetBrains.Annotations;
using Messages.Queries.Application.Queries;
using Messages.Queries.Persistence.Entities;
using Messages.Queries.Persistence.Repositories;
using Moq;
using Shared.Abstractions;
using Shared.Abstractions.Specifications;

namespace Messages.Queries.Tests.Queries;

[TestSubject(typeof(GetMessagesQuery))]
public class GetMessagesQueryTest
{
    private readonly Mock<IMessageReadRepository> _repository = new();
    private readonly Mock<IUser> _user = new();
    private readonly GetMessagesQuery.Handler _handler;

    public GetMessagesQueryTest() => 
        _handler = new GetMessagesQuery.Handler(_repository.Object, _user.Object);

    [Theory, MemberData(nameof(HandlerData))]
    public async Task Handler(GetMessagesQuery query, Result<PaginatedList<Message>> expectedResult, Guid? userId)
    {
        _user.Setup(x => x.Id)
            .Returns(userId);
        _repository.Setup(x => x.GetAllPaginatedAsync(It.IsAny<IPaginatedSpecification<Message>>(), default))
            .ReturnsAsync(expectedResult.Value);
        
        var result = await _handler.Handle(query, default);
        result.Should().BeEquivalentTo(expectedResult);
    }

    public static TheoryData<GetMessagesQuery, Result<PaginatedList<Message>>, Guid?> HandlerData => new()
    {
        {
            new GetMessagesQuery(1, 20, Guid.NewGuid(), "smth"),
            Result.Success(new PaginatedList<Message>(Array.Empty<Message>(),1,20,0)),
            Guid.NewGuid()
        },
        {
            new GetMessagesQuery(1, 20, Guid.NewGuid(), ""),
            Result.Success(new PaginatedList<Message>(Array.Empty<Message>(),1,20,0)),
            Guid.NewGuid()
        },
        {
            new GetMessagesQuery(1, 20, Guid.NewGuid(), ""),
            Result.Unauthorized(),
            null
        }
    };
}