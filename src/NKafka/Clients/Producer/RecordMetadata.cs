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

namespace NKafka.Clients.Producer;

/// <summary>
/// Represents metadata information for a record.
/// </summary>
public record RecordMetadata
{
    /// <summary>
    /// Gets or sets the topic and partition information.
    /// </summary>
    public TopicPartition? TopicPartition { get; set; }

    /// Gets or sets the offset value.
    /// /
    public Offset Offset { get; set; }

    /// <summary>
    /// Gets or sets the size of the serialized key.
    /// </summary>
    /// <value>
    /// The size of the serialized key.
    /// </value>
    public int SerializedKeySize { get; set; }

    /// <summary>
    /// Gets or sets the size of the serialized value.
    /// </summary>
    /// <remarks>
    /// This property represents the size, in bytes, of the serialized value of an object.
    /// It is used to determine the amount of space required to store the serialized value.
    /// </remarks>
    public int SerializedValueSize { get; set; }
}