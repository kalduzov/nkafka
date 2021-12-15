using System.IO;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

public class ApiVersionsRequestMessage : KafkaContent
{
    public string ClientSoftwareName { get; }

    public string ClientSoftwareVersion { get; }

    public ApiVersionsRequestMessage(string clientSoftwareName, string clientSoftwareVersion)
    {
        ClientSoftwareName = clientSoftwareName;
        ClientSoftwareVersion = clientSoftwareVersion;
        Version = ApiVersions.Version3;
        ApiKey = ApiKeys.ApiVersions;
        Length = CalculateLen();
    }

    private int CalculateLen()
    {
        var len = 0;
        
        //ClientSoftwareName.AsNullableString()
        return len;
    }

    public override void SerializeToStream(Stream stream)
    {

    }
}