using Messages.Queries.Persistence.Entities;
using Shared.EntityFramework;

namespace Messages.Queries.Persistence.Repositories;

public class MessageReadRepository(ApplicationContext context)
    : EntityFrameworkReadRepository<Message>(context), IMessageReadRepository;