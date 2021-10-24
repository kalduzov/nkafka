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
using System.Text;

using NKafka.Protocol.Records;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Protocol;

/// <summary>
/// Special struct for read data by kafka protocol
/// </summary>
public class BufferReader
{
    private readonly byte[] _body;
    private int _offset;

    public BufferReader(byte[] span)
    {
        _body = span;
        _offset = 0;
    }

    public byte ReadByte()
    {
        ThrowIfInsufficientData(sizeof(byte));

        return _body[_offset++];
    }

    public sbyte ReadSByte()
    {
        ThrowIfInsufficientData(sizeof(byte));

        return unchecked((sbyte)_body[_offset++]);
    }

    public short ReadShort()
    {
        ThrowIfInsufficientData(sizeof(short));
        var temp = _offset + 2;
        var value = ReadInt16BigEndian(_body.AsSpan()[_offset..temp]);
        _offset = temp;

        return value;
    }

    public ushort ReadUShort()
    {
        ThrowIfInsufficientData(sizeof(ushort));
        var temp = _offset + 2;
        var value = ReadUInt16BigEndian(_body.AsSpan()[_offset..temp]);
        _offset = temp;

        return value;
    }

    public int ReadInt()
    {
        ThrowIfInsufficientData(sizeof(int));
        var temp = _offset + 4;
        var value = ReadInt32BigEndian(_body.AsSpan()[_offset..temp]);
        _offset = temp;

        return value;
    }

    public uint ReadUInt()
    {
        ThrowIfInsufficientData(sizeof(uint));
        var temp = _offset + 4;
        var value = ReadUInt32BigEndian(_body.AsSpan()[_offset..temp]);
        _offset = temp;

        return value;
    }

    public long ReadLong()
    {
        ThrowIfInsufficientData(sizeof(long));
        var temp = _offset + 8;
        var value = ReadInt64BigEndian(_body.AsSpan()[_offset..temp]);
        _offset = temp;

        return value;
    }

    public ulong ReadULong()
    {
        ThrowIfInsufficientData(sizeof(ulong));
        var temp = _offset + 8;
        var value = ReadUInt64BigEndian(_body.AsSpan()[_offset..temp]);
        _offset = temp;

        return value;
    }

    public double ReadDouble()
    {
        ThrowIfInsufficientData(sizeof(double));
        var temp = _offset + 8;
        var value = ReadInt64BigEndian(_body.AsSpan()[_offset..temp]);
        _offset = temp;

        return BitConverter.Int64BitsToDouble(value);
    }

    public bool ReadBoolean()
    {
        var b = ReadByte();

        return b >= 1;
    }

    public long ReadVarLong()
    {
        var countBytes = VarLong(_body.AsSpan()[_offset..], out var value);

        switch (countBytes)
        {
            case 0:
                _offset = _body.Length;
                ThrowIfInsufficientData(int.MaxValue);

                return -1;
            case < 0:
                throw new OverflowException("");
            default:
                _offset += countBytes;

                return value;
        }
    }

    public ulong ReadUnsignedVarLong()
    {
        var countBytes = UnsignedVarLong(_body.AsSpan()[_offset..], out var value);

        switch (countBytes)
        {
            case 0:
                _offset = _body.Length;

                throw new InvalidDataException("Текущий буфер не содержит достаточного количества данных для считывания");
            case < 0:
                throw new OverflowException("");
            default:
                _offset += countBytes;

                return value;
        }
    }

    public byte[] ReadBytes(int length)
    {
        ThrowIfInsufficientData(length);

        var buf = _body.AsSpan()[_offset..(_offset + length)];
        _offset += length;

        return buf.ToArray();
    }

    public IRecords? ReadRecords(int length)
    {
        ThrowIfInsufficientData(length);

        if (length == 0)
        {
            return null;
        }

        _offset += length;

        return new Records.Records(this);
    }

    public string ReadString(int length)
    {
        var byteSting = _body.AsSpan().Slice(_offset, length);
        var value = Encoding.UTF8.GetString(byteSting);
        _offset += length;

        return value;
    }

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

    public int ReadVarUInt()
    {
        var value = ReadUnsignedVarLong();

        return (int)value;
    }

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