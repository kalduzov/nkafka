// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FastEnumUtility;

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;

namespace NKafka.Serialization;

public sealed class ListSerializer<T>: IAsyncSerializer<List<T>>
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

    private readonly IAsyncSerializer<T> _serializer;

    public ListSerializer(IAsyncSerializer<T> serializer)
    {
        _serializer = serializer;
        _serializationStrategy =
            _fixedLengthSerializers.Contains(serializer.GetType()) ? SerializationStrategy.ConstantSize : SerializationStrategy.VariableSize;
    }

    public async Task<byte[]> SerializeAsync(List<T> data)
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
                    var bytes = await _serializer.SerializeAsync(entry);

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
            throw new ProtocolKafkaException(ErrorCodes.UnknownServerError, "Failed to serialize list", exc);
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