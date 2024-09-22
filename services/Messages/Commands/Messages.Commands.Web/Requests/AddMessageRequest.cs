namespace Messages.Commands.Web.Requests;

public sealed record AddMessageRequest(string Content, DateTime Timestamp, Guid ReceiverId);
