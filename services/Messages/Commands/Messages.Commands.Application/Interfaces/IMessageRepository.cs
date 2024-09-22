using Messages.Commands.Domain;
using Shared.Abstractions;

namespace Messages.Commands.Application.Interfaces;

public interface IMessageRepository : IRepository<Message>;