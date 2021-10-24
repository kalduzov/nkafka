//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

using System.Buffers;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using Microsoft.IO;

namespace NKafka.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
// [NativeMemoryProfiler]
// [ThreadingDiagnoser]
public class WritersBenchmarks
{
    private static readonly RecyclableMemoryStreamManager _memoryStreamManager = new();
    private static readonly Guid _id = Guid.NewGuid();

    [Benchmark]
    public Span<byte> WriteToStream()
    {
        using var ms = _memoryStreamManager.GetStream(_id, "test", 124000);

        foreach (var i in Enumerable.Range(1, 200))
        {
            ms.Write(GetData(i));
        }

        return ms.GetBuffer();
    }

    [Benchmark]
    public Span<byte> WriteToArrayBufferWriter()
    {
        var buffer = new ArrayBufferWriter<byte>(20000);

        foreach (var i in Enumerable.Range(1, 200))
        {
            buffer.Write(GetData(i));
        }

        return buffer.GetSpan();
    }

    private static ReadOnlySpan<byte> GetData(int step)
    {
        return step switch
        {
            > 1 and < 100 => "element short"u8,
            >= 100 and < 200 => "element long element long element long element long"u8,
            _ => "none"u8
        };
    }
}