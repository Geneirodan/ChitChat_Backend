namespace Shared.Abstractions;

public interface IEntity<out TKey> 
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; }
}