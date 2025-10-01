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

using NKafka.Serialization;

namespace NKafka;

/// <summary>
///  Represents a raw Kafka message.
/// </summary>
/// <param name="key"></param>
/// <param name="value"></param>
public class Message(byte[] key, byte[] value)
{
    /// <summary>
    ///  Gets the message key value (not null).
    /// </summary>
    public byte[] Key { get; set; } = key;

    /// <summary>
    /// Gets the message value (not null).
    /// </summary>
    public byte[] Value { get; set; } = value;

    /// <summary>
    /// The collection of message headers (default Empty). 
    /// </summary>
    public Headers Headers { get; set; } = Headers.Empty;

    /// <summary>
    /// The message timestamp. The timestamp type must be set to CreateTime. 
    /// Specify Timestamp.Default to set the message timestamp to the time
    /// of this function call.
    /// </summary>
    public Timestamp Timestamp { get; set; } = Timestamp.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:NKafka.Message" /> class.
    /// </summary>
    public Message(byte[] value)
        : this(Serializers.Null.Serialize(default), value)
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Message(Key={Key}, Value={Value}, Timestamp={Timestamp})";
    }
}