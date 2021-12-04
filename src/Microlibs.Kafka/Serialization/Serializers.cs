using System;

namespace Microlibs.Kafka.Serialization;

public static class Serializers
{
    public static readonly ISerializer<string> String = new StringSerializer();

    public static readonly ISerializer<Null> Null = new NullSerializer();

    public static readonly ISerializer<long> Long = new LongSerializer();

    public static readonly ISerializer<int> Int = new IntegerSerializer();

    public static readonly ISerializer<float> Float = new FloatSerializer();

    public static readonly ISerializer<double> Double = new DoubleSerializer();

    public static readonly ISerializer<Guid> Guid = new GuidSerializer();

    public static readonly ISerializer<short> Short = new ShortSerializer();

    public static readonly ISerializer<byte[]> ByteArray = new ByteArraySerializer();
}