namespace Microlibs.Kafka;

/// <summary>
///     A type for use in conjunction with NullSerializer and NullDeserializer
///     that enables null key or values to be enforced when producing or 
///     consuming messages.
/// </summary>
public sealed record Null();