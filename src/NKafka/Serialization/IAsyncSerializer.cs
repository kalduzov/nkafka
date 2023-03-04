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

namespace NKafka.Serialization;

/// <summary>
/// Defines a serializer for use with <see cref="NKafka.Clients.Producer.IProducer{TKey,TValue}" />.
/// </summary>
public interface IAsyncSerializer<in T>
{
    /// <summary>
    /// Prefer async method for serialization
    /// </summary>
    /// <remarks>
    /// If this property is set then the client will call the <see cref="SerializeAsync"/> method to serialize, otherwise <see cref="Serialize"/>
    /// </remarks>>
    bool PreferAsync => true;

    /// <summary>
    /// Serialize the key or value of a <see cref="Message{TKey,TValue}" /> instance.
    /// </summary>
    /// <param name="data">The value to serialize.</param>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <returns>The serialized value.</returns>
    Task<byte[]> SerializeAsync(T data);

    /// <summary>
    /// Serialize the key or value of a <see cref="Message{TKey,TValue}" /> instance.
    /// </summary>
    /// <param name="data">The value to serialize.</param>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <returns>The serialized value.</returns>
    byte[] Serialize(T data);
}