﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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
public static class Serializers
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<string> String = new StringSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<Null> Null = new NullSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<long> Long = new LongSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<int> Int = new IntSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<float> Float = new FloatSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<double> Double = new DoubleSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<Guid> Guid = new GuidSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<short> Short = new ShortSerializer();

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAsyncSerializer<byte[]> ByteArray = new ByteArraySerializer();
}