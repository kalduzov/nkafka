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
using System.Runtime.InteropServices;

using NKafka.Exceptions;

namespace NKafka.Protocol.Buffers;

// This class fork from https://github.com/Cysharp/MemoryPack
[StructLayout(LayoutKind.Auto)]
internal ref partial struct BufferWriter
{
    private readonly ref ArrayBuffer _buffer;
    private ref byte _bufferReference;
    private int _advancedCount;

    /// <summary>
    /// 
    /// </summary>
    public int WrittenCount { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public int BufferLength { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    public BufferWriter(ref ArrayBuffer writer)
    {
        _buffer = ref writer;
        _bufferReference = ref Unsafe.NullRef<byte>();
        BufferLength = 0;
        WrittenCount = 0;
        _advancedCount = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sizeHint"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref byte GetSpanReference(int sizeHint)
    {
        if (BufferLength < sizeHint)
        {
            RequestNewBuffer(sizeHint);
        }

        return ref _bufferReference;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void RequestNewBuffer(int sizeHint)
    {
        if (_advancedCount != 0)
        {
            _buffer.Advance(_advancedCount);
            _advancedCount = 0;
        }
        var span = _buffer.GetSpan(sizeHint);
        _bufferReference = ref MemoryMarshal.GetReference(span);
        BufferLength = span.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <exception cref="KafkaException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        if (count == 0)
            return;

        var rest = BufferLength - count;

        if (rest < 0)
        {
            throw new SerializeDataException("Cannot advance past the end of the buffer.");
        }

        BufferLength = rest;
        _bufferReference = ref Unsafe.Add(ref _bufferReference, count);
        _advancedCount += count;
        WrittenCount += count;
        _buffer.Advance(count);
    }

    /// <summary>
    /// 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Flush()
    {
        if (_advancedCount != 0)
        {
            _buffer.Advance(_advancedCount);
            _advancedCount = 0;
        }
        _bufferReference = ref Unsafe.NullRef<byte>();
        BufferLength = 0;
        WrittenCount = 0;
    }

    public void WriteBytes(byte[] bytes)
    {
        ref var dest = ref GetSpanReference(bytes.Length);
        ref var src = ref Unsafe.As<byte, byte>(ref MemoryMarshal.GetReference(bytes.AsSpan()));
        Unsafe.CopyBlockUnaligned(ref dest, ref src, (uint)bytes.Length);

        Advance(bytes.Length);

    }

    public void WriteGuid(Guid value)
    {
        var bytes = value.ToByteArray();
        WriteBytes(bytes);
    }

    public void WriteRecords(Records.Records? records)
    {
        if (records is null)
        {
            return;
        }
        WriteBytes(records.Buffer.ToArrayAndReset());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUnmanaged<T1>(scoped in T1 value1)
        where T1 : unmanaged
    {
        var size = Unsafe.SizeOf<T1>();
        ref var spanRef = ref GetSpanReference(size);
        Unsafe.WriteUnaligned(ref spanRef, value1);
        Advance(size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUnmanaged<T1, T2>(scoped in T1 value1, scoped in T2 value2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>();
        ref var spanRef = ref GetSpanReference(size);
        Unsafe.WriteUnaligned(ref spanRef, value1);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()), value2);
        Advance(size);
    }
}