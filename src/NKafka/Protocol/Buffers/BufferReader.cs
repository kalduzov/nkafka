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
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Protocol.Buffers;

/// <summary>
/// Special struct for read data by kafka protocol
/// </summary>
[StructLayout(LayoutKind.Auto)]
public ref partial struct BufferReader
{
    private ReadOnlySpan<byte> _body;
    private int _offset;
    private readonly int _length;

    /// <summary>
    /// Текущее смещение читателя
    /// </summary>
    public int CurrentOffset => _offset;

    /// <summary>
    /// Общая длинна буфера для чтения 
    /// </summary>
    public int Length => _length;

    /// <summary>
    /// Сколько байт осталось в буфере
    /// </summary>
    public int Remaining => _length - _offset;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    public BufferReader(ReadOnlySpan<byte> buffer)
    {
        _body = buffer;
        _offset = 0;
        _length = buffer.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="bodyLen"></param>
    public BufferReader(ReadOnlySpan<byte> buffer, int bodyLen)
    {
        _body = buffer;
        _offset = 0;
        _length = bodyLen;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        if (count == 0)
        {
            return;

        }
        _offset += count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public byte ReadByte()
    {
        ThrowIfInsufficientData(sizeof(byte));

        var value = _body[_offset];
        Advance(sizeof(byte));

        return value;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public sbyte ReadSByte()
    {
        const int size = sizeof(sbyte);
        ThrowIfInsufficientData(size);

        var value = unchecked((sbyte)_body[_offset]);
        Advance(size);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public short ReadShort()
    {
        const int size = sizeof(short);
        ThrowIfInsufficientData(size);

        var value = ReadInt16BigEndian(_body[_offset..]);
        Advance(size);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ushort ReadUShort()
    {
        const int size = sizeof(ushort);
        ThrowIfInsufficientData(size);
        var value = ReadUInt16BigEndian(_body[_offset..]);
        Advance(size);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int ReadInt()
    {
        const int size = sizeof(int);
        ThrowIfInsufficientData(size);

        var value = ReadInt32BigEndian(_body[_offset..]);
        Advance(size);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public uint ReadUInt()
    {
        const int size = sizeof(uint);
        ThrowIfInsufficientData(size);

        var value = ReadUInt32BigEndian(_body[_offset..]);
        Advance(size);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public long ReadLong()
    {
        const int size = sizeof(long);
        ThrowIfInsufficientData(size);

        var value = ReadInt64BigEndian(_body[_offset..]);
        Advance(size);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ulong ReadULong()
    {
        const int size = sizeof(ulong);
        ThrowIfInsufficientData(size);
        var value = ReadUInt64BigEndian(_body[_offset..]);
        Advance(size);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public double ReadDouble()
    {
        const int size = sizeof(double);
        ThrowIfInsufficientData(size);
        var value = ReadInt64BigEndian(_body[_offset..]);
        Advance(size);

        return BitConverter.Int64BitsToDouble(value);
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
    /// <exception cref="OverflowException"></exception>
    public long ReadVarLong()
    {
        var countBytes = VarLong(_body[_offset..], out var value);

        switch (countBytes)
        {
            case 0:
                _offset = _body.Length;
                ThrowIfInsufficientData(int.MaxValue);

                return -1;
            case < 0:
                throw new OverflowException("");
            default:
                Advance(countBytes);

                return value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="OverflowException"></exception>
    public ulong ReadUnsignedVarLong()
    {
        var countBytes = UnsignedVarLong(_body[_offset..], out var value);

        switch (countBytes)
        {
            case 0:
                _offset = _body.Length;

                throw new InvalidDataException("Текущий буфер не содержит достаточного количества данных для считывания");
            case < 0:
                throw new OverflowException("");
            default:
                Advance(countBytes);

                return value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public byte[] ReadBytes(int length)
    {
        ThrowIfInsufficientData(length);

        var buf = _body[_offset..(_offset + length)];
        Advance(length);

        return buf.ToArray();
    }

    /// <summary>
    /// Считывает из буфера записи
    /// </summary>
    /// <param name="length">Длинна блока данных с записями</param>
    public Records.Records? ReadRecords(int length)
    {
        ThrowIfInsufficientData(length);

        return length == 0 ? null : new Records.Records(ref this, length);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public string ReadString(int length)
    {
        var byteSting = _body.Slice(_offset, length);
        var value = Encoding.UTF8.GetString(byteSting);
        Advance(length);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Guid ReadGuid()
    {
        ThrowIfInsufficientData(16);

        var data = ReadBytes(16);

        return new Guid(data);
    }

    private static int VarLong(ReadOnlySpan<byte> span, out long value)
    {
        var countBytes = UnsignedVarLong(span, out var uval);
        value = (long)(uval >> 1);

        if ((uval & 1) != 0)
        {
            value = ~value;
        }

        return countBytes;
    }

    private static int UnsignedVarLong(ReadOnlySpan<byte> span, out ulong value)
    {
        var i = 0;
        ulong b;
        var offset = 0;
        ulong tempValue = 0;

        while (true)
        {
            b = span[i];

            if ((b & 0x80) == 0)
            {
                i++;

                break;
            }

            tempValue |= (b & 0b01111111) << offset;

            offset += 7;
            i++;

            if (offset > 63)
            {
                throw new OverflowException();
            }
        }

        tempValue |= b << offset;

        value = tempValue;

        return i;
    }

    /// <summary>
    /// Если данных в буфере не хватает для считывания указанного размера, бросает исключение
    /// </summary>
    private void ThrowIfInsufficientData(int lengthRequired, [CallerMemberName] string? method = null)
    {
        var leftBytes = _body.Length - _offset;

        if (leftBytes >= lengthRequired)
        {
            return;
        }

        _offset = _body.Length;

        var message = $"Текущий буфер не содержит достаточного количества данных для считывания. Method: {method}";

        throw new InvalidDataException(message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int ReadVarUInt()
    {
        var value = ReadUnsignedVarLong();

        return (int)value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int ReadVarInt()
    {
        var value = ReadVarLong();

        return (int)value;
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
            return new List<TaggedField>(0);
        }

        var data = ReadBytes(size);
        unknowns.Add(new TaggedField(tag, data));

        return unknowns;
    }
}