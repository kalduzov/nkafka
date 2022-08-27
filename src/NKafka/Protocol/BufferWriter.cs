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

using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Protocol;

public readonly ref struct BufferWriter
{
    private readonly Stream _stream;

    public BufferWriter(Stream stream)
    {
        _stream = stream;
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

    public void WriteRecords(RecordBatch records)
    {
        _stream.Write(records.Buffer);
    }
}