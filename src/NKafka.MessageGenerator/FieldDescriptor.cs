using System.Text.Json.Serialization;

namespace NKafka.MessageGenerator;

public class FieldDescriptor
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("versions")]
    public string Versions { get; set; }
    
    [JsonPropertyName("nullableVersions")]
    public string? NullableVersions { get; set; }
    
    [JsonPropertyName("taggedVersions")]
    public string? TaggedVersions { get; set; }
    
    [JsonPropertyName("flexibleVersions")]
    public string? FlexibleVersions { get; set; }
    
    [JsonPropertyName("about")]
    public string? About { get; set; }

    [JsonPropertyName("default")]
    public string? Default { get; set; }

    [JsonPropertyName("ignorable")]
    public bool? Ignorable { get; set; }

    [JsonPropertyName("entityType")]
    public string EntityType { get; set; }

    [JsonPropertyName("mapKey")]
    public bool? MapKey { get; set; }

    [JsonPropertyName("tag")]
    public int? Tag { get; set; }

    [JsonPropertyName("zeroCopy")]
    public bool? ZeroCopy { get; set; }

    [JsonPropertyName("fields")]
    public List<FieldDescriptor> Fields { get; set; }
}