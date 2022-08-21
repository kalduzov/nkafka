using System.Text.Json.Serialization;

namespace NKafka.MessageGenerator;

public class FieldDescriptor
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public IFieldType Type { get; set; }

    [JsonPropertyName("versions")]
    public Versions? Versions { get; set; }
    
    [JsonPropertyName("nullableVersions")]
    public Versions? NullableVersions { get; set; }
    
    [JsonPropertyName("taggedVersions")]
    public Versions? TaggedVersions { get; set; }
    
    [JsonPropertyName("flexibleVersions")]
    public Versions? FlexibleVersions { get; set; }
    
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

