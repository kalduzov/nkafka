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
/// 
/// </summary>
public static class Deserializers
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<Null> Null = new NullDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<int> Int = new IntDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<long> Long = new LongDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<string> String = new StringDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<float> Float = new FloatDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<double> Double = new DoubleDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<byte[]> ByteArray = new ByteArrayDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<short> Short = new ShortDeserializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncDeserializer<Guid> Guid = new GuidDeserializer();
}