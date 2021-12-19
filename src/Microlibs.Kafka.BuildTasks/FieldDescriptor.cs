using System.Text.Json.Serialization;

namespace Microlibs.Kafka.BuildTasks;

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

    [JsonPropertyName("about")]
    public string? About { get; set; }

    [JsonPropertyName("default")]
    public string? Default { get; set; }

    [JsonPropertyName("ignorable")]
    public bool? Ignorable { get; set; }

    [JsonPropertyName("entityType")]
    public string EntityType { get; set; }

    [JsonPropertyName("fields")]
    public List<FieldDescriptor> Fields { get; set; }
}