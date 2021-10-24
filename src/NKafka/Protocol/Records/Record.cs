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

using NKafka.Protocol.Extensions;

namespace NKafka.Protocol.Records;

public class Record: IRecord
{
    private static readonly int _nullVarIntSizeBytes = -1.SizeOfVarUInt();

    public int Length { get; set; }

    /// <summary>
    /// The timestamp of the first Record in the batch. The timestamp of each Record in the RecordBatch is its 'TimestampDelta' + 'FirstTimestamp'.
    /// </summary>
    public sbyte Attributes { get; set; }

    public long TimestampDelta { get; set; }

    public long OffsetDelta { get; set; }

    public byte[] Key { get; set; }

    public byte[] Value { get; set; }

    /// <summary>
    /// Introduced in 0.11.0.0 for KIP-82, Kafka now supports application level record level headers.
    /// The Producer and Consumer APIS have been accordingly updated to write and read these headers.
    /// </summary>
    public Headers Headers { get; set; }

    public static int WriteTo(BufferWriter appendBuffer,
        int offsetDelta,
        long timestampDelta,
        byte[]? key,
        byte[]? value,
        Headers headers)
    {
        var sizeInBytes = SizeOfBodyInBytes(offsetDelta, timestampDelta, key, value, headers);
        appendBuffer.WriteVarInt(sizeInBytes);
        byte attributes = 0;
        appendBuffer.WriteByte(attributes);
        appendBuffer.WriteVarLong(timestampDelta);
        appendBuffer.WriteVarInt(offsetDelta);

        if (key is null)
        {
            appendBuffer.WriteVarInt(-1);
        }
        else
        {
            appendBuffer.WriteVarInt(key.Length);
            appendBuffer.WriteBytes(key);
        }

        if (value is null)
        {
            appendBuffer.WriteVarInt(-1);
        }
        else
        {
            appendBuffer.WriteVarInt(value.Length);
            appendBuffer.WriteBytes(value);
        }

        appendBuffer.WriteVarInt(headers.Count);

        foreach (var header in headers)
        {
            var headerKey = Encoding.UTF8.GetBytes(header.Key);
            appendBuffer.WriteVarInt(headerKey.Length);
            appendBuffer.WriteBytes(headerKey);

            if (header.Value is null)
            {
                appendBuffer.WriteVarInt(-1);
            }
            else
            {
                appendBuffer.WriteVarInt(header.Value.Length);
                appendBuffer.WriteBytes(header.Value);
            }
        }

        return sizeInBytes.SizeOfVarUInt() + sizeInBytes;
    }

    private static int SizeOfBodyInBytes(int offsetDelta,
        long timestampDelta,
        byte[]? key,
        byte[]? value,
        Headers headers)
    {
        var keySize = key?.Length ?? -1;
        var valueSize = value?.Length ?? -1;

        return SizeOfBodyInBytes(offsetDelta, timestampDelta, keySize, valueSize, headers);
    }

    private static int SizeOfBodyInBytes(int offsetDelta,
        long timestampDelta,
        int key,
        int value,
        Headers headers)
    {
        var size = 1;
        size += offsetDelta.SizeOfVarUInt();
        size += timestampDelta.SizeOfVarLong();
        size += SizeOf(key, value, headers);

        return size;
    }

    internal static int SizeOf(int keySize, int valueSize, Headers headers)
    {
        var size = 0;

        size += keySize < 0 ? _nullVarIntSizeBytes : keySize.SizeOfVarUInt() + keySize;
        size += valueSize < 0 ? _nullVarIntSizeBytes : valueSize.SizeOfVarUInt() + valueSize;

        size += headers.Count.SizeOfVarUInt();

        foreach (var header in headers)
        {
            var headerKeySize = Encoding.UTF8.GetByteCount(header.Key);
            size += headerKeySize.SizeOfVarUInt() + headerKeySize;
            size += header.Value is null ? _nullVarIntSizeBytes : header.Value.Length.SizeOfVarUInt() + header.Value.Length;
        }

        return size;
    }
}