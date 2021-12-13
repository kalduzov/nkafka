namespace Microlibs.Kafka.Protocol;

public readonly struct ApiVersion
{
    private readonly string _message;

    public readonly short ApiKey;

    public readonly short MinVersion;

    public readonly short MaxVersion;

    public ApiVersion(short apiKey, short minVersion, short maxVersion)
    {
        ApiKey = apiKey;
        MinVersion = minVersion;
        MaxVersion = maxVersion;

        _message = $"ApiKey: {apiKey} min value is {minVersion} and max version {maxVersion}";
    }

    public override int GetHashCode()
    {
        return ApiKey;
    }

    public override bool Equals(object obj)
    {
        return GetHashCode() == obj.GetHashCode();
    }

    public override string ToString()
    {
        return _message;
    }
}