using Shared.Domain;

namespace Messages.Contracts;

public record MessageEvent(Guid Id, Guid SenderId, Guid ReceiverId) : IntegrationEvent(Id);