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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
public class SerializersBenchmark
{
    [Benchmark(Baseline = true)]
    public byte[] ConfluentDoubleSerializer()
    {
        var data = Double.MaxValue;

        if (BitConverter.IsLittleEndian)
        {
            unsafe
            {
                var result = new byte[8];
                var p = (byte*)(&data);
                result[7] = *p++;
                result[6] = *p++;
                result[5] = *p++;
                result[4] = *p++;
                result[3] = *p++;
                result[2] = *p++;
                result[1] = *p++;
                result[0] = *p++;

                return result;
            }
        }
        else
        {
            return BitConverter.GetBytes(data);
        }
    }

    [Benchmark]
    public byte[] NKafkaDoubleSerializer()
    {
        var result = new byte[8];
        WriteDoubleBigEndian(result, double.MaxValue);

        return result;
    }

    [Benchmark]
    public byte[] NKafkaDoubleSerializer2()
    {
        var d = double.MaxValue;

        return Unsafe.As<double, byte[]>(ref d);
    }

    // [Benchmark(Baseline = true)]
    // public byte[] ConfluentInt32Serializer()
    // {
    //     var data = int.MaxValue;
    //     var result = new byte[4]; // int is always 32 bits on .NET.
    //
    //     result[0] = (byte)(data >> 24);
    //     result[1] = (byte)(data >> 16); // & 0xff;
    //     result[2] = (byte)(data >> 8); // & 0xff;
    //     result[3] = (byte)data; // & 0xff;
    //
    //     return result;
    // }

    // [Benchmark]
    // public byte[] ModifyConfluentInt32Serializer()
    // {
    //     var data = int.MaxValue;
    //     Span<byte> result = stackalloc byte[sizeof(int)];
    //
    //     // network byte order -> big endian -> most significant byte in the smallest address.
    //     // Note: At the IL level, the conv.u1 operator is used to cast int to byte which truncates
    //     // the high order bits if overflow occurs.
    //     // https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.conv_u1.aspx
    //     result[0] = (byte)(data >> 24);
    //     result[1] = (byte)(data >> 16); // & 0xff;
    //     result[2] = (byte)(data >> 8); // & 0xff;
    //     result[3] = (byte)data; // & 0xff;
    //
    //     return result.ToArray();
    // }

    // [Benchmark]
    // public Span<byte> Modify2ConfluentInt32Serializer()
    // {
    //     var data = int.MaxValue;
    //     Span<byte> result = new byte[sizeof(int)];
    //
    //     result[0] = (byte)(data >> 24);
    //     result[1] = (byte)(data >> 16); // & 0xff;
    //     result[2] = (byte)(data >> 8); // & 0xff;
    //     result[3] = (byte)data; // & 0xff;
    //
    //     return result;
    // }
    //
    // [Benchmark]
    // public byte[] NKafkaInt32Serializer()
    // {
    //     unchecked
    //     {
    //         var data = int.MaxValue;
    //
    //         return new[]
    //         {
    //             (byte)(data >> 24),
    //             (byte)(data >> 16),
    //             (byte)(data >> 8),
    //             (byte)data
    //         };
    //     }
    // }

    //
    // [Benchmark]
    // public byte[] DotnetInt32Serializer()
    // {
    //     Span<byte> destination = stackalloc byte[sizeof(int)];
    //
    //     WriteInt32BigEndian(destination, int.MaxValue);
    //
    //     return destination.ToArray();
    // }
}