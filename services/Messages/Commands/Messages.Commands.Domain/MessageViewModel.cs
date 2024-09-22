namespace Messages.Commands.Domain;

public sealed record MessageViewModel(Guid Id, string Content, bool IsRead, DateTime SendTime);