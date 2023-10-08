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

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;

using NKafka.Exceptions;

namespace NKafka.Serialization;

/// <summary>
/// 
/// </summary>
public sealed class StringSerializer: IAsyncSerializer<string>
{
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Serialize(string data)
    {
        var source = data.AsSpan();

        var maxByteCount = Encoding.UTF8.GetByteCount(source);
        var dest = GC.AllocateArray<byte>(maxByteCount);

#if NET7_0_OR_GREATER
        var status = Utf8.FromUtf16(source, dest, out _, out _, replaceInvalidSequences: false);

        if (status != OperationStatus.Done)
        {
            throw new KafkaException("Cannot serialize data.");
        }
        return dest;
#else
        return Encoding.UTF8.GetBytes(data);
#endif
    }
}