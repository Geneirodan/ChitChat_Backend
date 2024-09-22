using Messages.Queries.Persistence.Entities;
using Shared.Abstractions;

namespace Messages.Queries.Persistence.Repositories;

public interface IMessageRepository : IRepository<Message>;