using Messages.Queries.Persistence.Entities;
using Shared.EntityFramework;

namespace Messages.Queries.Persistence.Repositories;

public class MessageRepository(ApplicationContext context)
    : EntityFrameworkRepository<Message>(context), IMessageRepository;