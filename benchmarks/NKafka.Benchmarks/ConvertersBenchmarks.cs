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

using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
public class ConvertersBenchmarks
{
    private static readonly byte[] _data;

    static ConvertersBenchmarks()
    {
        _data = new byte[]
        {
            0x00,
            0x00,
            0x00,
            0x01
        };
    }

    [Benchmark(Baseline = true)]
    public int BitConverterByteArray()
    {
        return BitConverter.ToInt32(_data);
    }

    [Benchmark()]
    public int BufferPrimitives()
    {
        return ReadInt32BigEndian(_data);
    }

    [Benchmark]
    public int StructConverterByteArray()
    {
        var struct1 = new IntStruct(_data[3], _data[2], _data[1], _data[0]);

        return struct1.Value;
    }

    [Benchmark]
    public int OperationConverterByteArray()
    {
        return _data[3] | (_data[2] << 8) | (_data[1] << 16) | (_data[0] << 24);
    }

    [Benchmark]
    public int SpanConverterByteArray()
    {
        var res = 0;

        for (var i = 0; i < _data.Length; i += 4)
        {
            res += MemoryMarshal.Cast<byte, int>(_data.AsSpan(i, 4))[0];
        }

        return res;
    }

    [StructLayout(LayoutKind.Explicit)]
    private readonly struct IntStruct
    {
        public IntStruct(byte data0, byte data1, byte data2, byte data3)
        {
            Value = 0;
            Data0 = data0;
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
        }

        [FieldOffset(0)]
        private readonly byte Data0;

        [FieldOffset(1)]
        private readonly byte Data1;

        [FieldOffset(2)]
        private readonly byte Data2;

        [FieldOffset(3)]
        private readonly byte Data3;

        [FieldOffset(0)]
        public readonly int Value;
    }
}