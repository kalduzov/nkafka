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

using System.Buffers;
using System.Runtime.CompilerServices;

using NKafka.Protocol.Extensions;

namespace NKafka.Protocol;

/// <summary>
/// A helper class for writing the required types to the stream for sending to kafka
/// </summary>
public sealed class BufferWriter: IBufferWriter<byte>
{
    private const int _LEN_DATA = 4;
    private const int _NULL_VAR_INT_VALUE = -1;

    private readonly Stream _stream;
    private readonly int _lenReserved;
    private int _writtenCount = 0;

    /// <summary>
    /// 
    /// </summary>
    public long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public long Length => _stream.Length;

    /// <summary>
    /// How much space is left in the stream
    /// </summary>
    public long Remaining => _stream.Length - _stream.Position;

    /// <summary>
    /// 
    /// </summary>
    public int WrittenCount => _writtenCount;

    /// <summary>
    /// Создает новый класс для записи
    /// </summary>
    /// <param name="stream">Stream в который будет производиться запись</param>
    /// <param name="lenReserved">Записывать в первые 4 байта информацию о полном размере данных в потоке или нет</param>
    public BufferWriter(Stream stream, int lenReserved = _LEN_DATA)
    {
        _stream = stream;
        _lenReserved = lenReserved;

        if (lenReserved != 0)
        {
            _stream.Position = lenReserved;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="value"></param>
    public void PutUInt(int position, uint value)
    {
        var currentPosition = _stream.Position;
        _stream.Position = position;
        WriteUInt(value);
        _stream.Position = currentPosition;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteByte(byte value)
    {
        _stream.WriteByte(value);
        Advance(sizeof(byte));

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteBool(bool value)
    {
        _stream.WriteByte(value.AsByte());
        Advance(sizeof(bool));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteSByte(sbyte value)
    {
        _stream.WriteByte((byte)value);
        Advance(sizeof(sbyte));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteShort(short value)
    {
        _stream.Write(value.ToBigEndian());
        Advance(sizeof(short));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteUShort(ushort value)
    {
        _stream.Write(value.ToBigEndian());
        Advance(sizeof(ushort));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteInt(int value)
    {
        _stream.Write(value.ToBigEndian());
        Advance(sizeof(int));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteUInt(uint value)
    {
        _stream.Write(value.ToBigEndian());
        Advance(sizeof(uint));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteLong(long value)
    {
        _stream.Write(value.ToBigEndian());
        Advance(sizeof(long));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteULong(ulong value)
    {
        _stream.Write(value.ToBigEndian());
        Advance(sizeof(ulong));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteVarUInt(uint value)
    {
        var len = _stream.WriteVarUInt(value);
        Advance(len);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteVarUInt(int value)
    {
        var len = _stream.WriteVarUInt((uint)value);
        Advance(len);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteVarInt(int value)
    {
        var len = _stream.WriteVarInt(value);
        Advance(len);
    }

    /// <summary>
    /// 
    /// </summary>
    public void WriteNullVarInt()
    {
        var len = _stream.WriteVarInt(_NULL_VAR_INT_VALUE);
        Advance(len);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteVarLong(long value)
    {
        var len = _stream.WriteVarInt64(value);
        Advance(len);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteVarULong(ulong value)
    {
        var len = _stream.WriteVarUInt64(value);
        Advance(len);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteGuid(Guid value)
    {
        var bytes = value.ToByteArray();
        _stream.Write(bytes);
        Advance(bytes.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteDouble(double value)
    {
        var bytes = BitConverter.GetBytes(value);
        _stream.Write(bytes);
        Advance(bytes.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        _stream.Write(value);
        Advance(value.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void WriteBytesWithLength(ReadOnlySpan<byte> value)
    {
        var len = _stream.WriteVarInt(value.Length);
        _stream.Write(value);
        Advance(len + value.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="records"></param>
    public void WriteRecords(Records.Records records)
    {
        foreach (var batch in records.Batches)
        {
            _stream.Write(batch.Buffer.AsSpan(0, records.SizeInBytes));
            Advance(records.SizeInBytes);
        }
    }

    /// <summary>
    /// Записывает в начало длинну всего, что было записано ранее
    /// </summary>
    public void WriteSizeToStart()
    {
        if (_lenReserved != _LEN_DATA)
        {
            return;
        }
        _stream.Position = 0;
        var streamLen = (int)_stream.Length - _LEN_DATA;
        WriteInt(_stream.Length == 0 ? 0 : streamLen);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public Span<byte> AsSpan(int offset, int len)
    {
        return ((MemoryStream)_stream).GetBuffer().AsSpan(offset, len - offset);
    }

    /// <summary>
    /// 
    /// </summary>
    public Span<byte> WrittenSpan => AsSpan(0, (int)Position);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        if (count == 0)
        {
            return;
        }
        _writtenCount += count;
    }

    /// <inheritdoc />
    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return AsSpan(0, sizeHint);
    }
}