using System.Text.Json.Serialization;

namespace NKafka.MessageGenerator.Specifications;

/// <summary>
/// Message specification 
/// </summary>
public record MessageSpecification
{
    /// <summary>
    /// Message api key 
    /// </summary>
    [JsonPropertyName("apiKey")]
    public int ApiKey { get; }

    /// <summary>
    /// Message type 
    /// </summary>
    [JsonPropertyName("type")]
    public MessageType Type { get; }

    [JsonPropertyName("listeners")]
    public IReadOnlyCollection<RequestListenerType> Listeners { get; }

    /// <summary>
    /// Message name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("validVersions")]
    public Versions ValidVersions { get; }

    [JsonPropertyName("flexibleVersions")]
    public Versions FlexibleVersions { get; }

    [JsonPropertyName("fields")]
    public IReadOnlyCollection<FieldSpecification> Fields { get; }

    /// <summary>
    /// Внутренние структуры данных
    /// </summary>
    [JsonPropertyName("commonStructs")]
    public IReadOnlyCollection<StructSpecification>? CommonStructs { get; }

    [JsonIgnore]
    public string ClassName
    {
        get
        {
            return Type switch
            {
                MessageType.Request => Name + "Message",
                MessageType.Response => Name + "Message",
                MessageType.Header => Name + "Message",
                _ => Name
            };
        }
    }

    [JsonIgnore]
    public StructSpecification Struct { get; }

    [JsonConstructor]
    public MessageSpecification(
        int apiKey,
        MessageType type,
        IReadOnlyCollection<RequestListenerType> listeners,
        string name,
        Versions validVersions,
        Versions flexibleVersions,
        IReadOnlyCollection<FieldSpecification> fields,
        IReadOnlyCollection<StructSpecification>? commonStructs = null)
    {
        ApiKey = apiKey;
        Type = type;
        Name = name;
        ValidVersions = validVersions;
        FlexibleVersions = flexibleVersions;
        Listeners = listeners;
        Fields = fields;
        Struct = new StructSpecification(name, validVersions, fields);
        CommonStructs = commonStructs ?? ArraySegment<StructSpecification>.Empty;
    }
}