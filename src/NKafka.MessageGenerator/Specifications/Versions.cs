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

namespace NKafka.MessageGenerator.Specifications;

public sealed class Versions: IEquatable<Versions>
{
    public static readonly Versions All = new(0, short.MaxValue);

    public static readonly Versions None = new();

    public const string NONE_STRING = "none";

    public short Lowest { get; }

    public short Highest { get; }

    public Versions(short lowest, short highest)
    {
        if (lowest < 0 || highest < 0)
        {
            throw new ArgumentException("Invalid version range " + lowest + " to " + highest);
        }

        Lowest = lowest;
        Highest = highest;
    }

    public Versions()
    {
        Lowest = 0;
        Highest = -1;
    }

    public static Versions Parse(string input, Versions defaultVersions)
    {
        if (string.IsNullOrEmpty(input))
        {
            return defaultVersions;
        }

        var trimmedInput = input.Trim();

        if (trimmedInput.Length == 0)
        {
            return defaultVersions;
        }

        if (trimmedInput.Equals(NONE_STRING, StringComparison.OrdinalIgnoreCase))
        {
            return None;
        }

        if (trimmedInput.EndsWith('+'))
        {
            var lowest = short.Parse(trimmedInput[..1]);

            return new Versions(lowest, short.MaxValue);
        }
        else
        {
            var dashIndex = trimmedInput.IndexOf('-');

            if (dashIndex < 0)
            {
                var version = short.Parse(trimmedInput);

                return new Versions(version, version);
            }

            var lowest = short.Parse(trimmedInput[..dashIndex]);
            var highest = short.Parse(trimmedInput[(dashIndex + 1)..]);

            return new Versions(lowest, highest);
        }
    }

    public Versions Intersect(Versions other)
    {
        var newLowest = Lowest > other.Lowest ? Lowest : other.Lowest;
        var newHighest = Highest < other.Highest ? Highest : other.Highest;

        if (newLowest > newHighest)
        {
            return None;
        }

        return new Versions(newLowest, newHighest);
    }

    public static Versions operator -(Versions one, Versions two)
    {
        if (two.Lowest <= one.Lowest)
        {
            if (two.Highest >= one.Highest)
            {
                return None;
            }

            return two.Highest < one.Lowest ? one : new Versions((short)(two.Highest + 1), one.Highest);
        }

        if (two.Highest < one.Highest)
        {
            return null!;
        }

        var newHighest = two.Lowest - 1;

        if (newHighest < 0)
        {
            return one;
        }

        return newHighest < one.Highest ? new Versions(one.Lowest, (short)newHighest) : one;
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool Equals(Versions? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Lowest == other.Lowest
               && Highest == other.Highest;
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Versions other && Equals(other);
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Lowest, Highest);
    }

    public bool Contains(short version)
    {
        return version >= Lowest && version <= Highest;
    }

    public bool Contains(Versions version)
    {
        if (version.IsEmpty)
        {
            return true;
        }

        return !(Lowest > version.Lowest || Highest < version.Highest);
    }

    public bool IsEmpty => Lowest > Highest;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        if (IsEmpty)
        {
            return NONE_STRING;
        }

        if (Lowest == Highest)
        {
            return Lowest.ToString();
        }

        return Highest == short.MaxValue ? $"{Lowest}+" : $"{Lowest}-{Highest}";
    }
}