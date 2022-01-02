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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace Microlibs.Kafka.Serialization;

public static class Serializers
{
    public static readonly ISerializer<string> String = new StringSerializer();

    public static readonly ISerializer<Null> Null = new NullSerializer();

    public static readonly ISerializer<long> Long = new LongSerializer();

    public static readonly ISerializer<int> Int = new IntegerSerializer();

    public static readonly ISerializer<float> Float = new FloatSerializer();

    public static readonly ISerializer<double> Double = new DoubleSerializer();

    public static readonly ISerializer<Guid> Guid = new GuidSerializer();

    public static readonly ISerializer<short> Short = new ShortSerializer();

    public static readonly ISerializer<byte[]> ByteArray = new ByteArraySerializer();
}