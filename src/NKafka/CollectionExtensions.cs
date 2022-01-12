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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Diagnostics.Contracts;

namespace NKafka;

internal static class CollectionExtensions
{
    /// <summary>
    ///  Мешает текущий список и возвращает его. Сложность O(n)
    /// </summary>
    /// <remarks>Этот метод не производит аллокаций нового списка</remarks>
    internal static T[] InternalShuffle<T>(T[] list)
    {
        var random = new Random(Guid.NewGuid().GetHashCode());

        for (var i = list.Length - 1; i >= 1; i--)
        {
            var j = random.Next(0, i);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }

    /// <summary>
    /// Случайно мешает текущий список и возвращает новый
    /// </summary>
    /// <returns>Возвращает новый случайно перемешанный список того же типа что и переданный</returns>
    [Pure]
    public static T[] Shuffle<T>(this T[] list)
    {
        return InternalShuffle(list.ToArray());
    }

    /// <summary>
    /// Случайно мешает текущий список и возвращает новый
    /// </summary>
    /// <returns>Возвращает новый случайно перемешанный список того же типа что и переданный</returns>
    [Pure]
    public static ICollection<T> Shuffle<T>(this ICollection<T> list)
    {
        return InternalShuffle(list.ToArray());
    }

    /// <summary>
    /// Случайно мешает текущий список и возвращает новый
    /// </summary>
    /// <returns>Возвращает новый случайно перемешанный список того же типа что и переданный</returns>
    [Pure]
    public static IReadOnlyCollection<T> Shuffle<T>(this IReadOnlyCollection<T> list)
    {
        return InternalShuffle(list.ToArray());
    }

    /// <summary>
    /// Случайно мешает текущий список и возвращает новый
    /// </summary>
    /// <returns>Возвращает новый случайно перемешанный список того же типа что и переданный</returns>
    [Pure]
    public static List<T> Shuffle<T>(this List<T> list)
    {
        return InternalShuffle(list.ToArray()).ToList();
    }

    /// <summary>
    /// Случайно мешает текущий список и возвращает новый
    /// </summary>
    /// <returns>Возвращает новый случайно перемешанный список того же типа что и переданный</returns>
    [Pure]
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
    {
        return InternalShuffle(list.ToArray());
    }
}