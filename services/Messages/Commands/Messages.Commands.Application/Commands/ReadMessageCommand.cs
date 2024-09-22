using Ardalis.Result;
using MediatR;
using Messages.Commands.Application.Interfaces;
using Messages.Commands.Domain;
using Shared.Abstractions;
using Shared.MediatR.Attributes;

namespace Messages.Commands.Application.Commands;

[Authorize]
public sealed record ReadMessageCommand(Guid Id) : IRequest<Result>
{
    internal sealed class Handler(IMessageRepository repository, IUser user, IPublisher publisher)
        : IRequestHandler<ReadMessageCommand, Result>
    {
        public async Task<Result> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
        {
            var specification = new GetByIdSpecification<Message>(request.Id);
            var message = await repository.FindAsync(specification, cancellationToken).ConfigureAwait(false);
            
            //var message = await repository.FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (message is null)
                return Result.NotFound();

            if (message.ReceiverId != user.Id)
                return Result.Forbidden();

            var @event = message.Read();

            await repository.UpdateAsync(message, cancellationToken).ConfigureAwait(false);

            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await publisher.Publish(@event, cancellationToken).ConfigureAwait(false);

            return Result.Success();
        }
    }
}