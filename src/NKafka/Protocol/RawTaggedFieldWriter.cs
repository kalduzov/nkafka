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

namespace NKafka.Protocol;

public class RawTaggedFieldWriter
{
    private readonly IReadOnlyCollection<TaggedField> _fields;

    private static readonly RawTaggedFieldWriter _emptyWriter = new(Array.Empty<TaggedField>());
    private int _prevTag;

    public int FieldsCount => _fields.Count;

    private RawTaggedFieldWriter(IReadOnlyCollection<TaggedField> fields)
    {
        _fields = fields;
        _prevTag = -1;
    }

    public static RawTaggedFieldWriter ForFields(IReadOnlyCollection<TaggedField>? fields)
    {
        return fields == null ? _emptyWriter : new RawTaggedFieldWriter(fields);
    }

    internal void WriteRawTags(BufferWriter writer, int nextDefinedTag)
    {
        foreach (var field in _fields)
        {
            if (field.Tag >= nextDefinedTag)
            {
                if (field.Tag == nextDefinedTag)
                {
                    throw new Exception($"Attempted to use tag {field.Tag} as an undefined tag.");
                }

                return;
            }

            if (field.Tag <= _prevTag)
            {
                throw new Exception($"Invalid raw tag field list: tag {field.Tag} comes after tag {_prevTag}, but is not higher than it.");
            }

            writer.WriteVarUInt(field.Tag);
            writer.WriteVarInt(field.Data.Length);
            writer.WriteBytes(field.Data);
            _prevTag = field.Tag;
        }
    }
}