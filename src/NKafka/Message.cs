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
///  Represents a (deserialized) Kafka message.
/// </summary>
/// <typeparam name="TKey">Key type</typeparam>
/// <typeparam name="TValue">Value type</typeparam>
public class Message<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    /// <summary>
    ///  Gets the message key value (not null).
    /// </summary>
    public TKey Key { get; set; }

    /// <summary>
    /// Gets the message value (not null).
    /// </summary>
    public TValue Value { get; set; }

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
    /// Initializes a new instance of the <see cref="T:System.Object" /> class.
    /// </summary>
    public Message(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Message(Key={Key}, Value={Value}, Timestamp={Timestamp})";
    }
}