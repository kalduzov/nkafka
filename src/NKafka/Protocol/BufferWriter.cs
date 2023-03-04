﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Protocol;

/// <summary>
/// Вспомогательный класс записи нужных типов в поток для отправки в кафку 
/// </summary>
public class BufferWriter
{
    private const int _NULL_VAR_INT_VALUE = -1;

    private readonly Stream _stream;
    private readonly bool _lenReserved;

    public long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public long Length => _stream.Length;

    public long Remaining => _stream.Length - _stream.Position;

    /// <summary>
    /// Создает новый класс для записи
    /// </summary>
    /// <param name="stream">Stream в который будет производиться запись</param>
    /// <param name="lenReserved">Записывать в первые 4 байта информацию о полном размере данных в потоке или нет</param>
    public BufferWriter(Stream stream, bool lenReserved = true)
    {
        _stream = stream;
        _lenReserved = lenReserved;

        if (lenReserved)
        {
            WriteInt(0); //резервируем буфер под длинну сообщения
        }
    }

    public void WriteByte(byte value)
    {
        _stream.WriteByte(value);
    }

    public void WriteBool(bool value)
    {
        _stream.WriteByte(value.AsByte());
    }

    public void WriteSByte(sbyte value)
    {
        _stream.WriteByte((byte)value);
    }

    public void WriteShort(short value)
    {
        _stream.Write(value.ToBigEndian());
    }

    public void WriteUShort(ushort value)
    {
        _stream.Write(value.ToBigEndian());
    }

    public void WriteInt(int value)
    {
        _stream.Write(value.ToBigEndian());
    }

    public void WriteUInt(uint value)
    {
        _stream.Write(value.ToBigEndian());
    }

    public void WriteLong(long value)
    {
        _stream.Write(value.ToBigEndian());
    }

    public void WriteULong(ulong value)
    {
        _stream.Write(value.ToBigEndian());
    }

    public void WriteVarUInt(uint value)
    {
        _stream.WriteVarUInt(value);
    }

    public void WriteVarUInt(int value)
    {
        _stream.WriteVarUInt((uint)value);
    }

    public void WriteVarInt(int value)
    {
        _stream.WriteVarInt(value);
    }

    public void WriteNullVarInt()
    {
        _stream.WriteVarInt(_NULL_VAR_INT_VALUE);
    }

    public void WriteVarLong(long value)
    {
        _stream.WriteVarInt64(value);
    }

    public void WriteVarULong(ulong value)
    {
        _stream.WriteVarUInt64(value);
    }

    public void WriteGuid(Guid value)
    {
        _stream.Write(value.ToByteArray());
    }

    public void WriteDouble(double value)
    {
        var bytes = BitConverter.GetBytes(value);
        _stream.Write(bytes);
    }

    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        _stream.Write(value);
    }

    public void WriteBytesWithLength(ReadOnlySpan<byte> value)
    {
        _stream.WriteVarInt(value.Length);
        _stream.Write(value);
    }

    public void WriteRecords(IRecords records)
    {
        _stream.Write(records.Buffer);
    }

    /// <summary>
    /// Записывает в начало длинну всего, что было записано ранее
    /// </summary>
    public void End()
    {
        if (!_lenReserved)
        {
            return;
        }
        _stream.Position = 0;
        WriteInt((int)_stream.Length - 4);
    }
}