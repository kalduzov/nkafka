using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FastEnumUtility;
using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Serialization;

public sealed class ListSerializer<T> : ISerializer<List<T>>
{
    private const int _NULL_ENTRY_VALUE = -1;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly HashSet<Type> _fixedLengthSerializers = new()
    {
        typeof(IntegerSerializer),
        typeof(ShortSerializer),
        typeof(DoubleSerializer),
        typeof(LongSerializer),
        typeof(FloatSerializer),
        typeof(GuidSerializer)
    };

    private readonly SerializationStrategy _serializationStrategy;

    private readonly ISerializer<T> _serializer;

    public ListSerializer(ISerializer<T> serializer)
    {
        _serializer = serializer;
        _serializationStrategy =
            _fixedLengthSerializers.Contains(serializer.GetType()) ? SerializationStrategy.ConstantSize : SerializationStrategy.VariableSize;
    }

    public byte[] Serialize(List<T> data)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            memoryStream.Write(_serializationStrategy.ToInt32().ToBigEndian());

            if (_serializationStrategy == SerializationStrategy.ConstantSize)
            {
                SerializeNullIndexList(memoryStream, data);
            }

            memoryStream.Write(data.Count.ToBigEndian());

            foreach (var entry in data)
            {
                if (entry is null)
                {
                    if (_serializationStrategy == SerializationStrategy.VariableSize)
                    {
                        memoryStream.Write(_NULL_ENTRY_VALUE.ToBigEndian());
                    }
                }
                else
                {
                    var bytes = _serializer.Serialize(entry);

                    if (_serializationStrategy == SerializationStrategy.VariableSize)
                    {
                        memoryStream.Write(bytes.Length.ToBigEndian());
                    }

                    memoryStream.Write(bytes);
                }
            }

            return memoryStream.ToArray();
        }
        catch (Exception exc)
        {
            throw new ProtocolKafkaException(StatusCodes.UnknownServerError, "Failed to serialize list", exc);
        }
    }

    private static void SerializeNullIndexList(Stream memoryStream, IList data)
    {
        var list = new List<int>();

        for (var i = 0; i < data.Count; i++)
        {
            if (data[i] is null)
            {
                list.Add(i);
            }
        }

        memoryStream.Write(list.Count.ToBigEndian());

        foreach (var entry in list)
        {
            memoryStream.Write(entry.ToBigEndian());
        }
    }

    private enum SerializationStrategy
    {
        ConstantSize,
        VariableSize
    }
}