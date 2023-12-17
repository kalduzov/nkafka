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

using NKafka.Resources;

namespace NKafka;

/// <summary>
///     Represents a kafka message header.
/// </summary>
/// <remarks>
///     Message headers are supported by v0.11 brokers and above.
/// </remarks>
/// <remarks>
///     Create a new Header instance.
/// </remarks>
/// <param name="key">
///     The header key.
/// </param>
/// <param name="value">
///     The header value (may be null).
/// </param>
/// <exception cref="ArgumentNullException"></exception>
public sealed class Header(string key, byte[]? value): IEquatable<Header>
{
    /// <summary>
    /// The header key.
    /// </summary>
    public string Key { get; } = key ?? throw new ArgumentNullException(nameof(key), ExceptionMessages.Kafka_message_header_key_cannot_be_null);

    /// <summary>
    /// Get the serialized header value data.
    /// </summary>
    public byte[]? Value { get; } = value;

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
    /// </returns>
    public bool Equals(Header? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (Value is null && other.Value is null)
        {
            return Key == other.Key;
        }

        if (Value is not null && other.Value is not null)
        {
            return Value.SequenceEqual(other.Value) && Key == other.Key;
        }

        return false;
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((Header)obj);
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return Key.GetHashCode() * 397 + (Value?.GetHashCode() ?? 0);
        }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"RecordHeader(key = {Key})";
    }
}