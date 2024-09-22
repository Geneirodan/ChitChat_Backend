using Messages.Commands.Application.Interfaces;
using Shared.Marten;

namespace Messages.Commands.Persistence.Repositories;

internal sealed class MessageRepository(IDocumentSession session) 
    : MartenRepository<Domain.Message,Aggregates.Message>(session), IMessageRepository;