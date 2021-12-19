using System.Text.Json.Serialization;

namespace Microlibs.Kafka.BuildTasks;

public class ApiDescriptor
{
    [JsonPropertyName("apiKey")]
    public int ApiKey { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    //Пока это поле не нужно
    // [JsonPropertyName("listeners")]
    // public List<string> Listeners { get; set; } 

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("validVersions")]
    public string ValidVersions { get; set; }

    [JsonPropertyName("flexibleVersions")]
    public string FlexibleVersions { get; set; }

    [JsonPropertyName("fields")]
    public List<FieldDescriptor> Fields { get; set; }
}