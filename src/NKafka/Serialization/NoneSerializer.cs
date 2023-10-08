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
/// Used as a default value instead of null values.
/// </summary>
public sealed class NoneSerializer<T>: IAsyncSerializer<T>
{
    /// <summary>
    /// The default global instance.
    /// </summary>
    public static readonly IAsyncSerializer<T> Instance = new NoneSerializer<T>();

    /// <inheritdoc />
    public bool PreferAsync => throw new NotImplementedException("This property is not allowed.");

    /// <summary>
    /// Default ctor
    /// </summary>
    private NoneSerializer()
    {
    }

    /// <inheritdoc />
    public Task<byte[]> SerializeAsync(T data)
    {
        throw new NotImplementedException("This method is not allowed.");
    }

    /// <inheritdoc />
    public byte[] Serialize(T data)
    {
        throw new NotImplementedException("This method is not allowed.");
    }
}