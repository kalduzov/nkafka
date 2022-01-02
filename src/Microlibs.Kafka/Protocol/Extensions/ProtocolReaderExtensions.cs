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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace Microlibs.Kafka.Protocol.Extensions;

internal static class ProtocolReaderExtensions
{
    internal static ReadOnlySpan<byte> ReadNullableString(this ReadOnlySpan<byte> span, out string value)
    {
        var nextSpan = span.ReadInt16(out var stringLength);

        if (stringLength == -1)
        {
            value = null!;

            return nextSpan;
        }

        var byteSting = nextSpan[..stringLength];
        value = Encoding.UTF8.GetString(byteSting);

        return nextSpan[stringLength..];
    }

    internal static ReadOnlySpan<byte> ReadString(this ReadOnlySpan<byte> span, out string value)
    {
        var nextSpan = span.ReadInt16(out var stringLength);
        var byteSting = nextSpan[..stringLength];
        value = Encoding.UTF8.GetString(byteSting);

        return nextSpan[stringLength..];
    }

    internal static ReadOnlySpan<byte> ReadInt16(this ReadOnlySpan<byte> span, out short value)
    {
        value = ReadInt16BigEndian(span[..2]);

        return span[2..];
    }

    internal static ReadOnlySpan<byte> ReadInt32(this ReadOnlySpan<byte> span, out int value)
    {
        value = ReadInt32BigEndian(span[..4]);

        return span[4..];
    }

    internal static ReadOnlySpan<byte> ReadBoolean(this ReadOnlySpan<byte> span, out bool value)
    {
        value = span[..1][0] == 1;

        return span[1..];
    }
}