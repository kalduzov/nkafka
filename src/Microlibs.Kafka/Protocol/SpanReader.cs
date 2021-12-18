using System;
using System.IO;
using static System.Buffers.Binary.BinaryPrimitives;

namespace Microlibs.Kafka.Protocol;

public ref struct SpanReader
{
    private readonly ReadOnlySpan<byte> _body;
    private int _offset;

    public SpanReader(ReadOnlySpan<byte> span)
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

    public long ReadVariant()
    {
        return 0;
    }

    public ulong ReadUVariant()
    {
        return 0;
    }

    private void ThrowIfInsufficientData(int lengthRequired)
    {
        var leftBytes = _body.Length - _offset;

        if (leftBytes >= lengthRequired)
        {
            return;
        }

        _offset = _body.Length;

        throw new InvalidDataException("Текущий буфер не содержит достаточного количества данных для считывания");
    }
}