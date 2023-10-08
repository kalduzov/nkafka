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

namespace NKafka;

/// <summary>
///     Represents a Kafka partition.
/// </summary>
/// <remarks>
///     This structure is the same size as an int -
///     its purpose is to add some syntactical sugar
///     related to special values.
/// </remarks>
public readonly struct Partition: IEquatable<Partition>, IComparable<Partition>
{
    private const int _UNASSIGN_PARTITION_INDEX = -1;

    /// <summary>
    ///     A special value that refers to an unspecified / unknown partition.
    /// </summary>
    public static readonly Partition Any = new(_UNASSIGN_PARTITION_INDEX);

    /// <summary>
    ///     Initializes a new instance of the Partition structure.
    /// </summary>
    /// <param name="partition">
    ///     The partition value
    /// </param>
    public Partition(int partition)
    {
        Value = partition;
    }

    /// <summary>
    ///     Gets the int value corresponding to this partition.
    /// </summary>
    public int Value { get; }

    /// <summary>
    ///     Gets whether or not this is one of the special
    ///     partition values.
    /// </summary>
    public bool IsSpecial => Value == _UNASSIGN_PARTITION_INDEX;

    /// <summary>
    ///     Tests whether this Partition value is equal to the specified object.
    /// </summary>
    /// <param name="obj">
    ///     The object to test.
    /// </param>
    /// <returns>
    ///     true if obj is a Partition instance and has the same value. false otherwise.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is Partition p)
        {
            return Equals(p);
        }

        return false;
    }

    /// <summary>
    ///     Tests whether this Partition value is equal to the specified Partition.
    /// </summary>
    /// <param name="other">
    ///     The partition to test.
    /// </param>
    /// <returns>
    ///     true if other has the same value. false otherwise.
    /// </returns>
    public bool Equals(Partition other)
    {
        return other.Value == Value;
    }

    /// <summary>
    ///     Tests whether Partition value a is equal to Partition value b.
    /// </summary>
    /// <param name="a">
    ///     The first Partition value to compare.
    /// </param>
    /// <param name="b">
    ///     The second Partition value to compare.
    /// </param>
    /// <returns>
    ///     true if Partition value a and b are equal. false otherwise.
    /// </returns>
    public static bool operator ==(Partition a, Partition b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Tests whether Partition value a is not equal to Partition value b.
    /// </summary>
    /// <param name="a">
    ///     The first Partition value to compare.
    /// </param>
    /// <param name="b">
    ///     The second Partition value to compare.
    /// </param>
    /// <returns>
    ///     true if Partition value a and b are not equal. false otherwise.
    /// </returns>
    public static bool operator !=(Partition a, Partition b)
    {
        return !(a == b);
    }

    /// <summary>
    ///     Tests whether Partition value a is greater than Partition value b.
    /// </summary>
    /// <param name="a">
    ///     The first Partition value to compare.
    /// </param>
    /// <param name="b">
    ///     The second Partition value to compare.
    /// </param>
    /// <returns>
    ///     true if Partition value a is greater than Partition value b. false otherwise.
    /// </returns>
    public static bool operator >(Partition a, Partition b)
    {
        return a.Value > b.Value;
    }

    /// <summary>
    ///     Tests whether Partition value a is less than Partition value b.
    /// </summary>
    /// <param name="a">
    ///     The first Partition value to compare.
    /// </param>
    /// <param name="b">
    ///     The second Partition value to compare.
    /// </param>
    /// <returns>
    ///     true if Partition value a is less than Partition value b. false otherwise.
    /// </returns>
    public static bool operator <(Partition a, Partition b)
    {
        return a.Value < b.Value;
    }

    /// <summary>
    ///     Tests whether Partition value a is greater than or equal to Partition value b.
    /// </summary>
    /// <param name="a">
    ///     The first Partition value to compare.
    /// </param>
    /// <param name="b">
    ///     The second Partition value to compare.
    /// </param>
    /// <returns>
    ///     true if Partition value a is greater than or equal to Partition value b. false otherwise.
    /// </returns>
    public static bool operator >=(Partition a, Partition b)
    {
        return a.Value >= b.Value;
    }

    /// <summary>
    ///     Tests whether Partition value a is less than or equal to Partition value b.
    /// </summary>
    /// <param name="a">
    ///     The first Partition value to compare.
    /// </param>
    /// <param name="b">
    ///     The second Partition value to compare.
    /// </param>
    /// <returns>
    ///     true if Partition value a is less than or equal to Partition value b. false otherwise.
    /// </returns>
    public static bool operator <=(Partition a, Partition b)
    {
        return a.Value <= b.Value;
    }

    /// <summary>
    ///     Returns a hash code for this Partition.
    /// </summary>
    /// <returns>
    ///     An integer that specifies a hash value for this Partition.
    /// </returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Partition other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    ///     Converts the specified int value to an Partition value.
    /// </summary>
    /// <param name="value">
    ///     The int value to convert.
    /// </param>
    public static implicit operator Partition(int value)
    {
        return new Partition(value);
    }

    /// <summary>
    ///     Converts the specified Partition value to an int value.
    /// </summary>
    /// <param name="partition">
    ///     The Partition value to convert.
    /// </param>
    public static implicit operator int(Partition partition)
    {
        return partition.Value;
    }

    /// <summary>
    ///     Returns a string representation of the Partition object.
    /// </summary>
    /// <returns>
    ///     A string that represents the Partition object.
    /// </returns>
    public override string ToString()
    {
        return Value switch
        {
            _UNASSIGN_PARTITION_INDEX => "[Any]",
            _ => $"[{Value}]"
        };
    }
}