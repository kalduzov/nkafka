namespace Microlibs.Kafka;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey">Key type</typeparam>
/// <typeparam name="TValue">Value type</typeparam>
public record Message<TKey, TValue>(TKey Key, TValue Value)
{
    public TKey Key { get; } = Key;

    public TValue Value { get; } = Value;

    public Headers Headers { get; set; } = new();

    public Timestamp Timestamp { get; set; } = Timestamp.Default;

    public Partition Partition { get; set; } = Partition.Any;
}