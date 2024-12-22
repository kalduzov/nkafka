//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

internal static class RecordExtensions
{
    private const int _MAX_RECORD_OVERHEAD = 21;

    private static readonly int _nullVarIntSizeBytes = (-1).SizeOfVarInt();

    internal static int SizeOfBodyInBytes(int offsetDelta,
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
        int keySize,
        int value,
        Headers headers)
    {
        var size = 1;
        size += offsetDelta.SizeOfVarInt();
        size += timestampDelta.SizeOfVarLong();
        size += SizeOf(keySize, value, headers);

        return size;
    }

    internal static int SizeOf(int keySize, int valueSize, Headers headers)
    {
        var size = 0;

        size += keySize < 0 ? _nullVarIntSizeBytes : keySize.SizeOfVarInt() + keySize;
        size += valueSize < 0 ? _nullVarIntSizeBytes : valueSize.SizeOfVarInt() + valueSize;

        size += headers.Count.SizeOfVarInt();

        foreach (var header in headers)
        {
            var headerKeySize = Encoding.UTF8.GetByteCount(header.Key);
            size += headerKeySize.SizeOfVarInt() + headerKeySize;
            size += header.Value is null ? _nullVarIntSizeBytes : header.Value.Length.SizeOfVarInt() + header.Value.Length;
        }

        return size;
    }

    /// <summary>
    /// An estimate of the upper bound on the record size in bytes
    /// </summary>
    internal static int EstimateSizeInBytesUpperBound(byte[]? serializedKey, byte[]? serializedValue, Headers headers)
    {
        var keySize = serializedKey?.Length ?? -1;
        var valueSize = serializedValue?.Length ?? -1;

        return _MAX_RECORD_OVERHEAD + SizeOf(keySize, valueSize, headers);
    }
}