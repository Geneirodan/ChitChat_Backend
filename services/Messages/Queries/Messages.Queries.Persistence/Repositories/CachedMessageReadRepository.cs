using Messages.Queries.Persistence.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Shared.Abstractions;
using Shared.Redis;

namespace Messages.Queries.Persistence.Repositories;

public class CachedMessageReadRepository(IReadRepository<Message> repository, IDistributedCache cache, ILogger<CachedMessageReadRepository> logger) 
    : CachedReadRepository<Message>(repository, cache, logger), IMessageReadRepository;