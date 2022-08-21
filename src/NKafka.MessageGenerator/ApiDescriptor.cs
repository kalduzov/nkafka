using System.Text.Json.Serialization;

using NKafka.MessageGenerator.Converters;

namespace NKafka.MessageGenerator;

/// <summary>
/// Message descriptor 
/// </summary>
public class ApiDescriptor
{
    /// <summary>
    /// Message api key 
    /// </summary>
    [JsonPropertyName("apiKey")]
    public int ApiKey { get; set; }

    /// <summary>
    /// Message type 
    /// </summary>
    [JsonPropertyName("type")]
    public ApiMessageType Type { get; set; }

    [JsonPropertyName("listeners")]
    public List<string> Listeners { get; set; }

    /// <summary>
    /// Message name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("validVersions")]
    public Versions ValidVersions { get; set; }

    [JsonPropertyName("flexibleVersions")]
    public Versions FlexibleVersions { get; set; }

    [JsonPropertyName("fields")]
    public List<FieldDescriptor> Fields { get; set; }
}