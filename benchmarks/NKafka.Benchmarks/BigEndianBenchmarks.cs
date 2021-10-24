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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace NKafka.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
public class BigEndianBenchmarks
{
    [Benchmark]
    public Span<byte> CustomBigEndian()
    {
        const int value = int.MaxValue;

        unchecked
        {
            Span<byte> bytes = stackalloc byte[4];

            if (BitConverter.IsLittleEndian)
            {
                bytes[0] = value >> 24;
                bytes[1] = (byte)(value >> 16);
                bytes[2] = (byte)(value >> 8);
                bytes[3] = (byte)value;
            }
            else
            {
                bytes[3] = value >> 24;
                bytes[2] = (byte)(value >> 16);
                bytes[1] = (byte)(value >> 8);
                bytes[0] = (byte)value;
            }

            return bytes.ToArray();
        }
    }

    [Benchmark(Baseline = true)]
    public Span<byte> FromDotNetBigEndian()
    {
        Span<byte> bytes = stackalloc byte[4];
        System.Buffers.Binary.BinaryPrimitives.WriteInt32BigEndian(bytes, int.MaxValue);

        return bytes.ToArray();
    }
}