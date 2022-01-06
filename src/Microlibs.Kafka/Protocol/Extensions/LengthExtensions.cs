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
using System.Collections.Generic;

namespace Microlibs.Kafka.Protocol.Extensions;

internal static class LengthExtensions
{
    /// <summary>
    /// Возвращает полную длинну данных в массиве
    /// </summary>
    internal static int GetArrayLength<T>(this T[] array, bool isFlexibility = false)
    {
        var len = isFlexibility ? GetVarIntLen(array.Length + 1) : 4; //4 байта стандартная длинна для массивов в протоколе

        return len;
    }
    
    /// <summary>
    /// Возвращает полную длинну данных в массиве
    /// </summary>
    internal static int GetArrayLength<T>(this IReadOnlyCollection<T> array, bool isFlexibility = false)
    {
        var len = isFlexibility ? GetVarIntLen(array.Count + 1) : 4; //4 байта стандартная длинна для массивов в протоколе

        return len;
    }

    internal static byte GetVarIntLen(this int number)
    {
        return GetVarLongLen(number);
    }

    internal static byte GetVarLongLen(this long number)
    {
        byte len = number switch
        {
            > 0 and <= (1L << 7 * 1) - 1 => 1,
            > (1L << 7 * 1) - 1 and <= (1L << 7 * 2) - 1 => 2,
            > (1L << 7 * 2) - 1 and <= (1L << 7 * 3) - 1 => 3,
            > (1L << 7 * 3) - 1 and <= (1L << 7 * 4) - 1 => 4,
            > (1L << 7 * 4) - 1 and <= (1L << 7 * 5) - 1 => 5,
            > (1L << 7 * 5) - 1 and <= (1L << 7 * 6) - 1 => 6,
            > (1L << 7 * 6) - 1 and <= (1L << 7 * 7) - 1 => 7,
            > (1L << 7 * 7) - 1 and <= (1L << 7 * 8) - 1 => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(number), number, null)
        };

        return len;
    }
}