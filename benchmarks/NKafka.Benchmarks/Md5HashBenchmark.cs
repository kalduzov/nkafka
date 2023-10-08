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

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace NKafka.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
public class Md5HashBenchmark
{
    [Params("test")]
    public string Text;

    private MD5 _md5;

    [GlobalSetup]
    public void Setup()
    {
        _md5 = MD5.Create();
    }

    [Benchmark(Baseline = true)]
    public int CalculateSimple()
    {
        return CalculateSimpleInternal(Text);
    }

    [Benchmark]
    public int CalculateOptimization()
    {
        return CalculateOptimizationInternal(ref Text);
    }

    [Benchmark]
    public int CalculateOptimizationCrc32()
    {
        var buf = Unsafe.As<string, byte[]>(ref Text);

        return (int)Crc32(buf);
    }

    private int CalculateOptimizationInternal(ref string text)
    {
        Span<byte> dest = stackalloc byte[16];
        var buf = Unsafe.As<string, byte[]>(ref text);
        var hash = _md5.TryComputeHash(buf, dest, out _);

        return GetHashCode(dest);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateSimpleInternal(string text)
    {
        var buf = Encoding.UTF8.GetBytes(text);
        var hash = _md5.ComputeHash(buf);

        return GetHashCode(hash);
    }

    private static int GetHashCode(Span<byte> array)
    {
        unchecked
        {
            if (array == null)
            {
                return 0;
            }
            var hash = 17;

            foreach (var element in array)
            {
                hash = hash * 31 + element.GetHashCode();
            }

            return hash;
        }
    }

    private static uint Crc32(Span<byte> buffer)
    {
        var crc = 0xFFFFFFFF;

        foreach (var b in buffer)
        {
            crc = Sse42.Crc32(crc, b);
        }

        return crc ^ 0xFFFFFFFF;
    }
}