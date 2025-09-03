// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2024 Aleksey Kalduzov. All rights reserved
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NKafka.Protocol.Buffers;

// This class fork from https://github.com/Cysharp/MemoryPack
internal ref partial struct BufferReader
{
    private ReadOnlySequence<byte> _bufferSource;
    private readonly long _totalLength;
    private ref byte _bufferReference;
    private byte[]? _rentBuffer;
    private int _advancedCount;

    /// <summary>
    /// 
    /// </summary>
    public readonly long Remaining => _totalLength - CurrentOffset;

    /// <summary>
    /// 
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public int CurrentOffset { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    public BufferReader(ReadOnlySpan<byte> buffer)
    {
        _bufferSource = ReadOnlySequence<byte>.Empty;
        _bufferReference = ref MemoryMarshal.GetReference(buffer);
        Length = buffer.Length;
        _advancedCount = 0;
        CurrentOffset = 0;
        _rentBuffer = null;
        _totalLength = buffer.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    public BufferReader(in ReadOnlySequence<byte> sequence)
    {
        _bufferSource = sequence.IsSingleSegment ? ReadOnlySequence<byte>.Empty : sequence;
        var span = sequence.FirstSpan;
        _bufferReference = ref MemoryMarshal.GetReference(span);

        Length = span.Length;
        _advancedCount = 0;
        CurrentOffset = 0;
        _rentBuffer = null;
        _totalLength = sequence.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sizeHint"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref byte GetSpanReference(int sizeHint)
    {
        if (sizeHint <= Length)
        {
            return ref _bufferReference;
        }

        return ref GetNextSpan(sizeHint);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private ref byte GetNextSpan(int sizeHint)
    {
        const string message = "Текущий буфер не содержит достаточного количества данных для считывания";

        if (_rentBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(_rentBuffer);
            _rentBuffer = null;
        }

        if (Remaining == 0)
        {
            throw new InvalidDataException(message);
        }

        try
        {
            _bufferSource = _bufferSource.Slice(_advancedCount);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new InvalidDataException(message);
        }

        _advancedCount = 0;

        if (sizeHint > Remaining)
        {
            throw new InvalidDataException(message);
        }

        if (sizeHint <= _bufferSource.FirstSpan.Length)
        {

            _bufferReference = ref MemoryMarshal.GetReference(_bufferSource.FirstSpan);
            Length = _bufferSource.FirstSpan.Length;

            return ref _bufferReference;
        }

        _rentBuffer = ArrayPool<byte>.Shared.Rent(sizeHint);
        _bufferSource.Slice(0, sizeHint).CopyTo(_rentBuffer);
        var span = _rentBuffer.AsSpan(0, sizeHint);
        _bufferReference = ref MemoryMarshal.GetReference(span);
        Length = span.Length;

        return ref _bufferReference;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Advance(int count)
    {
        if (count == 0)
        {
            return;
        }

        var rest = Length - count;

        if (rest < 0)
        {
            if (TryAdvanceSequence(count))
            {
                return;
            }
        }
        Length = rest;
        _bufferReference = ref Unsafe.Add(ref _bufferReference, count);
        _advancedCount += count;
        CurrentOffset += count;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private bool TryAdvanceSequence(int count)
    {
        var rest = _bufferSource.Length - count;

        if (rest < 0)
        {
            throw new InvalidDataException("");
        }

        _bufferSource = _bufferSource.Slice(_advancedCount + count);
        _bufferReference = ref MemoryMarshal.GetReference(_bufferSource.FirstSpan);
        Length = _bufferSource.FirstSpan.Length;
        _advancedCount = 0;
        CurrentOffset += count;

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T1 ReadUnmanaged<T1>()
        where T1 : unmanaged
    {
        var size = Unsafe.SizeOf<T1>();
        ref var spanRef = ref GetSpanReference(size);
        var value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
        Advance(size);

        return value1;
    }

    /// <summary>
    /// 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_rentBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(_rentBuffer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadBytes(int length)
    {
        if (length == 0)
        {
            return [];
        }

        ref var src = ref GetSpanReference(length);

        var value = GC.AllocateUninitializedArray<byte>(length);
        ref var dest = ref Unsafe.As<byte, byte>(ref MemoryMarshal.GetArrayDataReference(value));
        Unsafe.CopyBlockUnaligned(ref dest, ref src, (uint)length);

        Advance(length);

        return value;
    }

    /// <summary>
    /// Считывает из буфера записи
    /// </summary>
    /// <param name="length">Длинна блока данных с записями</param>
    public Records.Records? ReadRecords(int length)
    {
        if (length == 0)
        {
            return null;
        }

        ref var src = ref GetSpanReference(length);
        var arrayBuffer = new ArrayBuffer(false, false, length);
        var bw = new BufferWriter(ref arrayBuffer);

        return new Records.Records(arrayBuffer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)] // non default, no inline 
    public string ReadString(int length)
    {
        var src = MemoryMarshal.CreateReadOnlySpan(ref GetSpanReference(length), length);
        var value = Encoding.UTF8.GetString(src);
        Advance(length);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unknowns"></param>
    /// <param name="tag"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public List<TaggedField> ReadUnknownTaggedField(List<TaggedField>? unknowns, int tag, int size)
    {
        if (unknowns is null)
        {
            return [];
        }

        var data = ReadBytes(size);
        unknowns.Add(new TaggedField(tag, data));

        return unknowns;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Guid ReadGuid()
    {

        var data = ReadBytes(16);

        return new Guid(data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        var b = ReadByte();

        return b >= 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public double ReadDouble()
    {
        var value = ReadLong();

        return BitConverter.Int64BitsToDouble(value);
    }
}