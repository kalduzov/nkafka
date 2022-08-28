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

using System.Text;

using NKafka.MessageGenerator.Specifications;

namespace NKafka.MessageGenerator;

internal class WriteMethodGenerator: IWriteMethodGenerator
{
    private readonly IHeaderGenerator _headerGenerator;
    private readonly StructRegistry _structRegistry;
    private readonly CodeBuffer _codeBuffer;
    private Versions _messageFlexibleVersions = Versions.None;

    public WriteMethodGenerator(IHeaderGenerator headerGenerator, StructRegistry structRegistry, CodeBuffer codeBuffer)
    {
        _headerGenerator = headerGenerator;
        _structRegistry = structRegistry;
        _codeBuffer = codeBuffer;
    }

    public void Generate(string className, StructSpecification structSpecification, Versions parentVersions, Versions messageFlexibleVersions)
    {
        _messageFlexibleVersions = messageFlexibleVersions;

        _codeBuffer.AppendLine("internal override void Write(BufferWriter writer, ApiVersions version)");
        _codeBuffer.AppendLine("{");
        _codeBuffer.IncrementIndent();

        VersionConditional.ForVersions(structSpecification.Versions, parentVersions)
            .AllowMembershipCheckAlwaysFalse(false)
            .IfNotMember(
                _ => { _codeBuffer.AppendLine($"throw new UnsupportedVersionException($\"Can't write version {{version}} of {className}\");"); })
            .Generate(_codeBuffer);

        _codeBuffer.AppendLine("var numTaggedFields = 0;");
        var curVersions = parentVersions.Intersect(structSpecification.Versions);
        var taggedFields = new Dictionary<int, FieldSpecification>();

        foreach (var field in structSpecification.Fields)
        {
            var cond = VersionConditional.ForVersions(field.Versions, curVersions)
                .IfMember(
                    presentVersions =>
                    {
                        VersionConditional.ForVersions(field.TaggedVersions, presentVersions)
                            .IfNotMember(
                                presentAndUntaggedVersions =>
                                {
                                    if (field.Type.IsVariableLength && !field.Type.IsStruct)
                                    {
                                        void CallGenerateVariableLengthWriter(Versions versions)
                                        {
                                            GenerateVariableLengthWriter(
                                                FieldFlexibleVersions(field),
                                                field.Name,
                                                field.Type,
                                                versions,
                                                field.NullableVersions,
                                                field.ZeroCopy);
                                        }

                                        if (field.Type.IsArray
                                            && ((IFieldType.ArrayType)field.Type).ElementType.SerializationIsDifferentInFlexibleVersions)
                                        {
                                            VersionConditional.ForVersions(FieldFlexibleVersions(field), presentAndUntaggedVersions)
                                                .IfMember(CallGenerateVariableLengthWriter)
                                                .IfNotMember(CallGenerateVariableLengthWriter)
                                                .Generate(_codeBuffer);
                                        }
                                        else
                                        {
                                            CallGenerateVariableLengthWriter(presentAndUntaggedVersions);
                                        }
                                    }
                                    else
                                    {
                                        _codeBuffer.AppendLine($"{PrimitiveWriteExpression(field.Type, field.Name)};");
                                    }
                                })
                            .IfMember(
                                _ =>
                                {
                                    field.GenerateNonDefaultValueCheck(_structRegistry, _codeBuffer, field.NullableVersions);
                                    _codeBuffer.IncrementIndent();
                                    _codeBuffer.AppendLine("numTaggedFields++;");
                                    _codeBuffer.DecrementIndent();
                                    _codeBuffer.AppendLine("}");

                                    if (!taggedFields.TryAdd(field.Tag.Value, field))
                                    {
                                        throw new Exception($"Field {field.Name} has tag {field.Tag}, but another field already used that tag.");
                                    }
                                })
                            .Generate(_codeBuffer);
                    });

            if (!field.Ignorable)
            {
                cond.IfNotMember(_ => { field.GenerateNonIgnorableFieldCheck(_structRegistry, _codeBuffer); });
            }

            cond.Generate(_codeBuffer);
        }

        _codeBuffer.AppendLine("var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);");
        _codeBuffer.AppendLine("numTaggedFields += rawWriter.FieldsCount;");

        VersionConditional.ForVersions(messageFlexibleVersions, curVersions)
            .IfNotMember(_ => { GenerateCheckForUnsupportedNumTaggedFields("numTaggedFields > 0"); })
            .IfMember(
                _ =>
                {
                    _codeBuffer.AppendLine("writer.WriteVarUInt(numTaggedFields);");
                    var prevTag = -1;

                    foreach (var field in taggedFields.Values)
                    {
                        if (prevTag + 1 != field.Tag)
                        {
                            _codeBuffer.AppendLine($"rawWriter.WriteRawTags(writer,{field.Tag});");
                        }

                        VersionConditional
                            .ForVersions(field.Versions, field.TaggedVersions.Intersect(field.Versions))
                            .AllowMembershipCheckAlwaysFalse(false)
                            .IfMember(
                                presentAndTaggedVersions =>
                                {
                                    var cond = IsNullConditional.ForName(field.Name)
                                        .NullableVersions(field.NullableVersions)
                                        .PossibleVersions(presentAndTaggedVersions)
                                        .AlwaysEmitBlockScope(true)
                                        .IfShouldNotBeNull(
                                            () =>
                                            {
                                                if (!field.Default.Equals("null"))
                                                {
                                                    field.GenerateNonDefaultValueCheck(_structRegistry, _codeBuffer, Versions.None);
                                                    _codeBuffer.IncrementIndent();
                                                }

                                                _codeBuffer.AppendLine($"writer.WriteVarUInt({field.Tag});");

                                                if (field.Type.IsString)
                                                {
                                                    _codeBuffer.AppendLine($"var stringBytes = Encoding.UTF8.GetBytes({field.Name});");
                                                    _codeBuffer.AppendLine(
                                                        "writer.WriteVarUInt(stringBytes.Length + (stringBytes.Length + 1).SizeOfVarUInt());");
                                                    _codeBuffer.AppendLine("writer.WriteVarUInt(stringBytes.Length + 1);");
                                                    _codeBuffer.AppendLine("writer.WriteBytes(stringBytes);");
                                                }
                                                else if (field.Type.IsBytes)
                                                {
                                                    _codeBuffer.AppendLine(
                                                        $"writer.WriteVarUInt({field.Name}.Length + ({field.Name}.Length + 1).SizeOfVarUInt());");
                                                    _codeBuffer.AppendLine($"writer.WriteVarUInt({field.Name}.Length + 1);");
                                                    _codeBuffer.AppendLine($"writer.WriteBytes({field.Name});");
                                                }
                                                else if (field.Type.IsArray)
                                                {
                                                    //todo тут проблема с рассчетом размера - надо подумать как сделать    
                                                    GenerateVariableLengthWriter(
                                                        FieldFlexibleVersions(field),
                                                        field.Name,
                                                        field.Type,
                                                        presentAndTaggedVersions,
                                                        Versions.None,
                                                        field.ZeroCopy);
                                                }
                                                else if (field.Type.IsStruct)
                                                {
                                                    //todo тут проблема с рассчетом размера - надо подумать как сделать
                                                    _codeBuffer.AppendLine($"{PrimitiveWriteExpression(field.Type, field.Name)};");
                                                }
                                                else if (field.Type.IsRecords)
                                                {
                                                    throw new Exception(
                                                        $"Unsupported attempt to declare field `{field.Name}` with `records` type as a tagged field.");
                                                }
                                                else
                                                {
                                                    _codeBuffer.AppendLine($"writer.WriteVarUInt({field.Type.Size});");
                                                    _codeBuffer.AppendLine($"{PrimitiveWriteExpression(field.Type, field.Name)};");
                                                }

                                                if (!field.Default.Equals("null"))
                                                {
                                                    _codeBuffer.DecrementIndent();
                                                    _codeBuffer.AppendLine("}");
                                                }
                                            });

                                    if (!field.Default.Equals("null"))
                                    {
                                        cond.IfNull(
                                            () =>
                                            {
                                                _codeBuffer.AppendLine($"writer.WriteVarUInt({field.Tag});");
                                                _codeBuffer.AppendLine("writer.WriteVarUInt(1);");
                                                _codeBuffer.AppendLine("writer.WriteVarUInt(0);");
                                            });
                                    }

                                    cond.Generate(_codeBuffer);
                                })
                            .Generate(_codeBuffer);
                        prevTag = field.Tag.Value;
                    }

                    if (prevTag < int.MaxValue)
                    {
                        _codeBuffer.AppendLine("rawWriter.WriteRawTags(writer, int.MaxValue);");
                    }
                })
            .Generate(_codeBuffer);
        _codeBuffer.DecrementIndent();
        _codeBuffer.AppendLine("}");
    }

    private void GenerateCheckForUnsupportedNumTaggedFields(string conditional)
    {
        _codeBuffer.AppendLine($"if ({conditional})");
        _codeBuffer.AppendLine("{");
        _codeBuffer.IncrementIndent();
        _codeBuffer.AppendLine(
            "throw new UnsupportedVersionException($\"Tagged fields were set, "
            + "but version {version} of this message does not support them.\");");
        _codeBuffer.DecrementIndent();
        _codeBuffer.AppendLine("}");
    }

    private static string PrimitiveWriteExpression(IFieldType type, string name)
    {
        return type switch
        {
            IFieldType.BoolFieldType => $"writer.WriteBool({name})",
            IFieldType.Int8FieldType => $"writer.WriteSByte({name})",
            IFieldType.Int16FieldType => $"writer.WriteShort({name})",
            IFieldType.UInt16FieldType => $"writer.WriteUShort({name})",
            IFieldType.Int32FieldType => $"writer.WriteInt({name})",
            IFieldType.UInt32FieldType => $"writer.WriteUInt({name})",
            IFieldType.Int64FieldType => $"writer.WriteLong({name})",
            IFieldType.UuidFieldType => $"writer.WriteGuid({name})",
            IFieldType.Float64FieldType => $"writer.WriteDouble({name})",
            IFieldType.StructType => $"{name}.Write(writer, version)",
            _ => throw new Exception($"Unsupported field type {type}")
        };
    }

    private Versions FieldFlexibleVersions(FieldSpecification field)
    {
        if (field.FlexibleVersions is null)
        {
            return _messageFlexibleVersions;
        }

        if (!_messageFlexibleVersions.Intersect(field.FlexibleVersions).Equals(field.FlexibleVersions))
        {
            throw new Exception(
                $"The flexible versions for field {field.Name} are {field.FlexibleVersions}, "
                + $"which are not a subset of the flexible versions for the message as a whole, which are {_messageFlexibleVersions}");
        }

        return field.FlexibleVersions;
    }

    private void GenerateVariableLengthWriter(
        Versions fieldFlexibleVersions,
        string name,
        IFieldType type,
        Versions possibleVersions,
        Versions nullableVersions,
        bool zeroCopy)
    {
        IsNullConditional.ForName(name)
            .PossibleVersions(possibleVersions)
            .NullableVersions(nullableVersions)
            .AlwaysEmitBlockScope(type.IsString)
            .IfNull(
                () =>
                {
                    VersionConditional.ForVersions(nullableVersions, possibleVersions)
                        .IfMember(
                            presentVersions =>
                            {
                                VersionConditional.ForVersions(fieldFlexibleVersions, presentVersions)
                                    .IfMember(_ => { _codeBuffer.AppendLine($"writer.WriteVarUInt(0);"); })
                                    .IfNotMember(
                                        _ =>
                                        {
                                            if (type.IsString)
                                            {
                                                _codeBuffer.AppendLine($"writer.WriteShort(-1);");
                                            }
                                            else
                                            {
                                                _codeBuffer.AppendLine($"writer.WriteInt(-1);");
                                            }
                                        })
                                    .Generate(_codeBuffer);
                            })
                        .IfNotMember(_ => { _codeBuffer.Append("throw new NullReferenceException();"); })
                        .Generate(_codeBuffer);
                })
            .IfShouldNotBeNull(
                () =>
                {
                    string lengthExpression;

                    if (type.IsString)
                    {
                        _codeBuffer.AppendLine($"var stringBytes = Encoding.UTF8.GetBytes({name});");
                        lengthExpression = "stringBytes.Length";
                    }
                    else if (type.IsBytes)
                    {
                        lengthExpression = $"{name}.Length";
                    }
                    else if (type.IsRecords)
                    {
                        lengthExpression = $"{name}.Length";
                    }
                    else if (type.IsArray)
                    {
                        lengthExpression = $"{name}.Count";
                    }
                    else
                    {
                        throw new Exception($"Unhandled type {type}");
                    }

                    VersionConditional.ForVersions(fieldFlexibleVersions, possibleVersions)
                        .IfMember(_ => { _codeBuffer.AppendLine($"writer.WriteVarUInt({lengthExpression} + 1);"); })
                        .IfNotMember(
                            _ =>
                            {
                                if (type.IsString)
                                {
                                    _codeBuffer.AppendLine($"writer.WriteShort((short){lengthExpression});");
                                }
                                else
                                {
                                    _codeBuffer.AppendLine($"writer.WriteInt({lengthExpression});");
                                }
                            })
                        .Generate(_codeBuffer);

                    if (type.IsString)
                    {
                        _codeBuffer.AppendLine("writer.WriteBytes(stringBytes);");
                    }
                    else if (type.IsBytes)
                    {
                        _codeBuffer.AppendLine($"writer.WriteBytes({name});");
                    }
                    else if (type.IsRecords)
                    {
                        _codeBuffer.AppendLine($"writer.WriteRecords({name});");
                    }
                    else if (type is IFieldType.ArrayType arrayType)
                    {
                        var elementType = arrayType.ElementType;
                        _codeBuffer.AppendLine($"foreach (var element in {name})");
                        _codeBuffer.AppendLine("{");
                        _codeBuffer.IncrementIndent();

                        if (elementType.IsArray)
                        {
                            throw new Exception("Nested arrays are not supported. Use an array of structures containing another array.");
                        }
                        else if (elementType.IsBytes || elementType.IsString)
                        {
                            GenerateVariableLengthWriter(fieldFlexibleVersions, "element", elementType, possibleVersions, Versions.None, false);
                        }
                        else
                        {
                            _codeBuffer.AppendLine($"{PrimitiveWriteExpression(elementType, "element")};");
                        }

                        _codeBuffer.DecrementIndent();
                        _codeBuffer.AppendLine("}");
                    }
                })
            .Generate(_codeBuffer);
    }
}