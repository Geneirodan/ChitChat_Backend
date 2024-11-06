using Shared.Abstractions;

namespace Shared.Domain;

public abstract class Aggregate : Entity<Guid>
{

    public int Version { get; protected set; }
    
    public bool IsDeleted { get; protected set; }

    [NonSerialized]
    private readonly Queue<IDomainEvent> _uncommittedEvents = new();

    public IEnumerable<IDomainEvent> DequeueUncommittedEvents()
    {
        while (_uncommittedEvents.TryDequeue(out var result))
            yield return result;
    }

    protected void Enqueue(IDomainEvent @event) => _uncommittedEvents.Enqueue(@event);
}