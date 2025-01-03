//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Text;

using NKafka.Protocol.Buffers;
using NKafka.Protocol.Extensions;

namespace NKafka.Protocol.Records;

/// <summary>
/// Record type implementation
/// https://kafka.apache.org/documentation/#record
/// </summary>
internal class Record
{
    /// <summary>
    /// Full record length
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public sbyte Attributes { get; private set; }

    /// <summary>
    /// The timestamp of the first Record in the batch. The timestamp of each Record in the RecordBatch is its 'TimestampDelta' + 'FirstTimestamp'.
    /// </summary>
    public long TimestampDelta { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public long OffsetDelta { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public byte[]? Key { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public byte[]? Value { get; private set; }

    /// <summary>
    /// Introduced in 0.11.0.0 for KIP-82, Kafka now supports application level record level headers.
    /// The Producer and Consumer APIS have been accordingly updated to write and read these headers.
    /// </summary>
    public Headers Headers { get; private set; } = Headers.Empty;

    /// <summary>
    /// Валидная запись или нет
    /// </summary>
    internal bool IsValid { get; private set; } = true;

    /// <summary>
    /// Запись считана лишь частично
    /// </summary>
    internal bool IsPartial { get; private set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public Record()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    public Record(ref BufferReader reader)
    {
        Read(ref reader);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    private void Read(ref BufferReader reader)
    {
        Length = reader.ReadVarInt();
        Attributes = reader.ReadSByte();
        TimestampDelta = reader.ReadVarInt64();
        OffsetDelta = reader.ReadVarInt();

        var keyLen = reader.ReadVarInt();

        if (reader.Remaining < keyLen)
        {
            IsValid = false;

            return;
        }

        if (keyLen > 0)
        {
            Key = reader.ReadBytes(keyLen);
        }

        var valueLen = reader.ReadVarInt();

        if (reader.Remaining < valueLen)
        {
            IsValid = false;

            return;
        }
        Value = reader.ReadBytes(valueLen);

        var countHeader = reader.ReadVarInt();

        if (countHeader != 0)
        {
            var headers = new List<Header>(countHeader);

            for (var j = 0; j < countHeader; j++)
            {
                if (reader.Remaining < 4)
                {
                    IsValid = false;

                    return;
                }

                var headerKeyLen = reader.ReadVarInt();

                if (reader.Remaining < headerKeyLen + 4)
                {
                    IsValid = false;

                    return;
                }
                var headerKey = reader.ReadString(headerKeyLen);
                var headerValueLen = reader.ReadVarInt();

                if (reader.Remaining < headerValueLen)
                {
                    IsValid = false;

                    return;
                }
                var headerValue = reader.ReadBytes(headerValueLen);

                var header = new Header(headerKey, headerValue);
                headers.Add(header);
            }
            Headers = new Headers(headers);
        }
        else
        {
            Headers = Headers.Empty;
        }
    }

    public int WriteTo(ref BufferWriter bufferWriter)
    {
        const byte attributes = 0; //  bit 0~7: unused in the current version of the protocol

        var sizeInBytes = RecordExtensions.SizeOfBodyInBytes((int)OffsetDelta, TimestampDelta, Key, Value, Headers);
        bufferWriter.WriteVarInt(sizeInBytes);
        bufferWriter.WriteByte(attributes);
        bufferWriter.WriteVarLong(TimestampDelta);
        bufferWriter.WriteVarLong(OffsetDelta);

        if (Key is null)
        {
            bufferWriter.WriteNullVarInt();
        }
        else
        {
            bufferWriter.WriteBytesWithLength(Key);
        }

        if (Value is null)
        {
            bufferWriter.WriteNullVarInt();
        }
        else
        {
            bufferWriter.WriteBytesWithLength(Value);
        }

        bufferWriter.WriteVarInt(Headers.Count);

        foreach (var header in Headers)
        {
            var headerKey = Encoding.UTF8.GetBytes(header.Key);
            bufferWriter.WriteBytesWithLength(headerKey);

            if (header.Value is null)
            {
                bufferWriter.WriteNullVarInt();
            }
            else
            {
                bufferWriter.WriteBytesWithLength(header.Value);
            }
        }

        return sizeInBytes.SizeOfVarInt() + sizeInBytes;
    }
}