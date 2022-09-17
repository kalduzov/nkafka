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

using System.Threading.Tasks;

namespace NKafka.Serialization;

public sealed class LongSerializer: IAsyncSerializer<long>
{
    public Task<byte[]> SerializeAsync(long data)
    {
        var result = new[]
        {
            (byte)(data >> 56),
            (byte)(data >> 48),
            (byte)(data >> 40),
            (byte)(data >> 32),
            (byte)(data >> 24),
            (byte)(data >> 16),
            (byte)(data >> 8),
            (byte)data
        };

        return Task.FromResult(result);
    }
}