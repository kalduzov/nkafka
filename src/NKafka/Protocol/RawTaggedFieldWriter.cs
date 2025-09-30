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

using NKafka.Protocol.Buffers;
using NKafka.Protocol.Extensions;

namespace NKafka.Protocol;

internal class RawTaggedFieldWriter
{
    private static readonly RawTaggedFieldWriter _emptyWriter = new([]);
    private readonly List<TaggedField> _fields;
    private int _prevTag;
    private int _position;

    public int FieldsCount => _fields.Count;

    private RawTaggedFieldWriter(List<TaggedField> fields)
    {
        _fields = fields;
        _prevTag = -1;
        _position = 0;
    }

    public static RawTaggedFieldWriter ForFields(List<TaggedField>? fields)
    {
        return fields == null ? _emptyWriter : new RawTaggedFieldWriter(fields);
    }

    internal void WriteRawTags(ref BufferWriter writer, int nextDefinedTag)
    {
        while (_position < _fields.Count)
        {
            var (tag, data) = _fields[_position];

            if (tag >= nextDefinedTag)
            {
                if (tag == nextDefinedTag)
                {
                    throw new Exception($"Attempted to use tag {tag} as an undefined tag.");
                }

                return;
            }

            if (tag <= _prevTag)
            {
                throw new Exception($"Invalid raw tag field list: tag {tag} comes after tag {_prevTag}, but is not higher than it.");
            }

            writer.WriteVarUInt32(tag);
            writer.WriteVarUInt32(data.Length);
            writer.WriteBytes(data);

            _prevTag = tag;
            _position++;
        }
    }
}