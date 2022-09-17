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

namespace NKafka.Protocol.Records;

/// <summary>
///     Represents a kafka message header.
/// </summary>
/// <remarks>
///     Message headers are supported by v0.11 brokers and above.
/// </remarks>
public class Header: IHeader
{
    private readonly byte[] _val;

    /// <summary>
    ///     Create a new Header instance.
    /// </summary>
    /// <param name="key">
    ///     The header key.
    /// </param>
    /// <param name="value">
    ///     The header value (may be null).
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    public Header(string key, byte[] value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key), ExceptionMessages.Kafka_message_header_key_cannot_be_null);
        _val = value;
    }

    /// <summary>
    ///     The header key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Get the serialized header value data.
    /// </summary>
    public byte[] GetValueBytes()
    {
        return _val;
    }
}