﻿using System;
using System.IO;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace Microlibs.Kafka.Protocol;

public ref struct KafkaBufferReader
{
    private readonly ReadOnlySpan<byte> _body;
    private int _offset;

    public KafkaBufferReader(ReadOnlySpan<byte> span)
    {
        _body = span;
        _offset = 0;
    }

    public byte ReadByte()
    {
        ThrowIfInsufficientData(sizeof(byte));

        return _body[_offset++];
    }

    public short ReadShort()
    {
        ThrowIfInsufficientData(sizeof(short));
        var temp = _offset + 2;
        var value = ReadInt16BigEndian(_body[_offset..temp]);
        _offset = temp;

        return value;
    }

    public ushort ReadUShort()
    {
        ThrowIfInsufficientData(sizeof(ushort));
        var temp = _offset + 2;
        var value = ReadUInt16BigEndian(_body[_offset..temp]);
        _offset = temp;

        return value;
    }

    public int ReadInt()
    {
        ThrowIfInsufficientData(sizeof(int));
        var temp = _offset + 4;
        var value = ReadInt32BigEndian(_body[_offset..temp]);
        _offset = temp;

        return value;
    }

    public uint ReadUInt()
    {
        ThrowIfInsufficientData(sizeof(uint));
        var temp = _offset + 4;
        var value = ReadUInt32BigEndian(_body[_offset..temp]);
        _offset = temp;

        return value;
    }

    public long ReadLong()
    {
        ThrowIfInsufficientData(sizeof(long));
        var temp = _offset + 8;
        var value = ReadInt64BigEndian(_body[_offset..temp]);
        _offset = temp;

        return value;
    }

    public ulong ReadULong()
    {
        ThrowIfInsufficientData(sizeof(ulong));
        var temp = _offset + 8;
        var value = ReadUInt64BigEndian(_body[_offset..temp]);
        _offset = temp;

        return value;
    }

    public double ReadDouble()
    {
        ThrowIfInsufficientData(sizeof(double));
        var temp = _offset + 8;
        var value = ReadInt64BigEndian(_body[_offset..temp]);
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
        var countBytes = VarLong(_body[_offset..], out var value);

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
                _offset += countBytes;

                return value;
        }
    }

    public int ReadArrayLength()
    {
        return ReadInt();
    }

    public int ReadCompactArrayLength()
    {
        var value = ReadUnsignedVarLong();

        return (int)value - 1;
    }

    public int ReadEmptyTaggedFieldArray()
    {
        var _ = ReadUnsignedVarLong();

        return 0;
    }

    public ReadOnlySpan<byte> ReadBytes()
    {
        var length = ReadInt();

        ThrowIfInsufficientData(length);

        var buf = _body[_offset..(_offset + length)];
        _offset += length;

        return buf;
    }

    public ReadOnlySpan<byte> ReadCompactBytes()
    {
        var length = (int)ReadVarLong();

        ThrowIfInsufficientData(length);

        var buf = _body[_offset..(_offset + length)];
        _offset += length;

        return buf;
    }

    public int ReadStringLength()
    {
        var value = ReadShort();

        ThrowIfInsufficientData(value);

        return value;
    }

    public string ReadString()
    {
        var stringLength = ReadStringLength();

        if (stringLength == -1)
        {
            throw new InvalidDataException("Формат данных для простого строкового типа неверен");
        }

        var byteSting = _body.Slice(_offset, stringLength);
        var value = Encoding.UTF8.GetString(byteSting);
        _offset += stringLength;

        return value;
    }

    public string? ReadNullableString()
    {
        var stringLength = ReadStringLength();

        if (stringLength == -1)
        {
            return null;
        }

        var byteSting = _body.Slice(_offset, stringLength);
        var value = Encoding.UTF8.GetString(byteSting);
        _offset += stringLength;

        return value;
    }

    public string ReadCompactString()
    {
        var stringLength = (int)ReadUnsignedVarLong() - 1;

        if (stringLength == -1)
        {
            throw new InvalidDataException("Формат данных для простого строкового типа неверен");
        }

        ThrowIfInsufficientData(stringLength);

        var byteSting = _body.Slice(_offset, stringLength);
        var value = Encoding.UTF8.GetString(byteSting);
        _offset += stringLength;

        return value;
    }

    public string? ReadCompactNullableString()
    {
        var stringLength = (int)ReadUnsignedVarLong() - 1;

        if (stringLength == -1)
        {
            return null;
        }

        ThrowIfInsufficientData(stringLength);

        var byteSting = _body.Slice(_offset, stringLength);
        var value = Encoding.UTF8.GetString(byteSting);
        _offset += stringLength;

        return value;
    }

    public int[] ReadIntArray()
    {
        var arrayLength = ReadArrayLength();

        return arrayLength == 0 ? Array.Empty<int>() : ReadIntArray(arrayLength);
    }

    public int[] ReadCompactIntArray()
    {
        var arrayLength = ReadCompactArrayLength();

        return arrayLength == 0 ? Array.Empty<int>() : ReadIntArray(arrayLength);
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

    private void ThrowIfInsufficientData(int lengthRequired, string? message = null)
    {
        var leftBytes = _body.Length - _offset;

        if (leftBytes >= lengthRequired)
        {
            return;
        }

        _offset = _body.Length;

        throw new InvalidDataException(message ?? "Текущий буфер не содержит достаточного количества данных для считывания");
    }

    private int[] ReadIntArray(int arrayLength)
    {
        var result = new int[arrayLength];

        for (var i = 0; i < arrayLength; i++)
        {
            result[i] = ReadInt();
        }

        return result;
    }
}