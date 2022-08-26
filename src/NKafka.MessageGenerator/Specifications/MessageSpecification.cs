using System.Collections.Immutable;

using Newtonsoft.Json;

namespace NKafka.MessageGenerator.Specifications;

/// <summary>
/// Message specification 
/// </summary>
public record MessageSpecification
{
    /// <summary>
    /// Message api key 
    /// </summary>
    public int ApiKey { get; }

    /// <summary>
    /// Message type 
    /// </summary>
    public MessageType Type { get; }

    public IReadOnlyCollection<RequestListenerType>? Listeners { get; }

    public Versions ValidVersions => Struct.Versions;

    public Versions FlexibleVersions { get; }

    public IReadOnlyCollection<FieldSpecification> Fields => Struct.Fields;

    /// <summary>
    /// Внутренние структуры данных
    /// </summary>
    public IReadOnlyCollection<StructSpecification> CommonStructs { get; }

    [JsonIgnore]
    public string ClassName
    {
        get
        {
            return Type switch
            {
                MessageType.Request => Struct.Name + "Message",
                MessageType.Response => Struct.Name + "Message",
                MessageType.Header => Struct.Name + "Message",
                _ => Struct.Name
            };
        }
    }

    [JsonIgnore]
    public StructSpecification Struct { get; }

    [JsonConstructor]
    public MessageSpecification(
        [JsonProperty("apiKey")] int? apiKey,
        [JsonProperty("type")] MessageType type,
        [JsonProperty("listeners")] IReadOnlyCollection<RequestListenerType>? listeners,
        [JsonProperty("name")] string name,
        [JsonProperty("validVersions")] string validVersions,
        [JsonProperty("flexibleVersions")] string flexibleVersions,
        [JsonProperty("fields")] IReadOnlyCollection<FieldSpecification> fields,
        [JsonProperty("commonStructs")] IReadOnlyCollection<StructSpecification>? commonStructs)
    {
        Struct = new StructSpecification(name, validVersions, fields);
        ApiKey = apiKey ?? -1;
        Type = type;
        CommonStructs = (commonStructs ?? Array.Empty<StructSpecification>()).ToImmutableArray();

        if (string.IsNullOrWhiteSpace(flexibleVersions))
        {
            throw new ArgumentException("You must specify a value for flexibleVersions. Please use 0+ for all new messages.");
        }

        FlexibleVersions = Versions.Parse(flexibleVersions, Versions.None);

        if (FlexibleVersions.IsEmpty && FlexibleVersions.Highest < short.MaxValue)
        {
            throw new ArgumentException(
                $"Field {name} specifies flexibleVersions {FlexibleVersions}, which is not open-ended. flexibleVersions must be either none, "
                + "or an open-ended range (that ends with a plus sign).");
        }

        if (listeners is not null && listeners.Count != 0 && type != MessageType.Request)
        {
            throw new ArgumentException("The `requestScope` property is only valid for messages with type `request`");
        }
        Listeners = listeners;

    }
}