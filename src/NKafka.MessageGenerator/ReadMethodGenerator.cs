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

using NKafka.MessageGenerator.Specifications;

namespace NKafka.MessageGenerator;

internal class ReadMethodGenerator: MethodGenerator, IReadMethodGenerator
{
    private readonly IHeaderGenerator _headerGenerator;
    private readonly StructRegistry _structRegistry;
    private readonly CodeBuffer _codeBuffer;

    public ReadMethodGenerator(IHeaderGenerator headerGenerator, StructRegistry structRegistry, CodeBuffer codeBuffer)
    {
        _headerGenerator = headerGenerator;
        _structRegistry = structRegistry;
        _codeBuffer = codeBuffer;
    }

    public void Generate(string className, StructSpecification structSpecification, Versions parentVersions, Versions messageFlexibleVersions)
    {
        MessageFlexibleVersions = messageFlexibleVersions;
        _codeBuffer.AppendLine("public void Read(BufferReader reader, ApiVersions version)");
        _codeBuffer.AppendLine("{");
        _codeBuffer.IncrementIndent();

        VersionConditional
            .ForVersions(parentVersions, structSpecification.Versions)
            .AllowMembershipCheckAlwaysFalse(false)
            .IfNotMember(
                _ => { _codeBuffer.AppendLine($"throw new UnsupportedVersionException($\"Can't read version {{version}} of {className}\");"); })
            .Generate(_codeBuffer);

        var curVersions = parentVersions.Intersect(structSpecification.Versions);

        foreach (var field in structSpecification.Fields)
        {
            var fieldFlexibleVersions = FieldFlexibleVersions(field);

            if (!field.TaggedVersions.Intersect(fieldFlexibleVersions).Equals(field.TaggedVersions))
            {
                throw new Exception(
                    $"Field {field.Name} specifies tagged versions {field.TaggedVersions} "
                    + $"that are not a subset of the flexible versions {fieldFlexibleVersions}");
            }

            var mandatoryVersions = field.Versions - field.TaggedVersions;

            VersionConditional
                .ForVersions(mandatoryVersions, curVersions)
                .AlwaysEmitBlockScope(field.Type.IsVariableLength)
                .IfNotMember(_ => { _codeBuffer.AppendLine($"{field.Name} = {field.FieldDefault()};"); })
                .IfMember(
                    presentAndUntaggedVersions =>
                    {
                        if (field.Type.IsVariableLength && !field.Type.IsStruct)
                        {
                            void CallGenerateVariableLengthReader(Versions versions)
                            {
                                GenerateVariableLengthReader(
                                    FieldFlexibleVersions(field),
                                    field.Name,
                                    field.Type,
                                    versions,
                                    field.NullableVersions,
                                    $"{field.Name} = ",
                                    ";",
                                    _structRegistry.IsStructArrayWithKeys(field),
                                    field.ZeroCopy);
                            }

                            if (field.Type.IsArray && ((IFieldType.ArrayType)field.Type).ElementType.SerializationIsDifferentInFlexibleVersions)
                            {
                                VersionConditional
                                    .ForVersions(FieldFlexibleVersions(field), presentAndUntaggedVersions)
                                    .IfMember(CallGenerateVariableLengthReader)
                                    .IfNotMember(CallGenerateVariableLengthReader)
                                    .Generate(_codeBuffer);
                            }
                            else
                            {
                                CallGenerateVariableLengthReader(presentAndUntaggedVersions);
                            }
                        }
                        else
                        {
                            _codeBuffer.AppendLine($"{field.Name} = {PrimitiveReadExpression(field.Type)};");
                        }
                    })
                .Generate(_codeBuffer);
        }

        _codeBuffer.AppendLine("UnknownTaggedFields = null;");
        VersionConditional
            .ForVersions(messageFlexibleVersions, curVersions)
            .IfMember(
                curFlexibleVersions =>
                {
                    _codeBuffer.AppendLine("var numTaggedFields = reader.ReadVarUInt();");
                    _codeBuffer.AppendLine("for (var t = 0; t < numTaggedFields; t++)");
                    _codeBuffer.AppendLine("{");
                    _codeBuffer.IncrementIndent();
                    _codeBuffer.AppendLine("var tag = reader.ReadVarUInt();");
                    _codeBuffer.AppendLine("var size = reader.ReadVarUInt();");
                    _codeBuffer.AppendLine("switch (tag)");
                    _codeBuffer.AppendLine("{");
                    _codeBuffer.IncrementIndent();

                    foreach (var field in structSpecification.Fields)
                    {
                        var validTaggedVersions = field.Versions.Intersect(field.TaggedVersions);

                        if (!validTaggedVersions.IsEmpty)
                        {
                            if (!field.Tag.HasValue)
                            {
                                throw new Exception($"Field {field.Name} has tagged versions, but no tag.");
                            }

                            _codeBuffer.AppendLine($"case {field.Tag}:");
                            _codeBuffer.AppendLine("{");
                            _codeBuffer.IncrementIndent();
                            VersionConditional
                                .ForVersions(validTaggedVersions, curFlexibleVersions)
                                .IfMember(
                                    presentAndTaggedVersions =>
                                    {
                                        if (field.Type.IsVariableLength && !field.Type.IsStruct)
                                        {
                                            GenerateVariableLengthReader(
                                                FieldFlexibleVersions(field),
                                                field.Name,
                                                field.Type,
                                                presentAndTaggedVersions,
                                                field.NullableVersions,
                                                $"{field.Name} = ",
                                                $";",
                                                _structRegistry.IsStructArrayWithKeys(field),
                                                field.ZeroCopy);
                                        }
                                        else
                                        {
                                            _codeBuffer.AppendLine($"{field.Name} = {PrimitiveReadExpression(field.Type)};");
                                        }

                                        _codeBuffer.AppendLine("break;");
                                    })
                                .IfNotMember(
                                    _ =>
                                    {
                                        _codeBuffer.AppendLine(
                                            $"throw new RuntimeException($\"Tag {field.Tag} is not valid for version {{version}});");
                                    })
                                .Generate(_codeBuffer);
                            _codeBuffer.DecrementIndent();
                            _codeBuffer.AppendLine("}");
                        }
                    }
                    _codeBuffer.AppendLine("default:");
                    _codeBuffer.IncrementIndent();
                    _codeBuffer.AppendLine("UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);");
                    _codeBuffer.AppendLine("break;");
                    _codeBuffer.DecrementIndent();
                    _codeBuffer.DecrementIndent();
                    _codeBuffer.AppendLine("}");
                    _codeBuffer.DecrementIndent();
                    _codeBuffer.AppendLine("}");
                })
            .Generate(_codeBuffer);
        _codeBuffer.DecrementIndent();
        _codeBuffer.AppendLine("}");
    }

    private static string PrimitiveReadExpression(IFieldType type)
    {
        switch (type)
        {
            case IFieldType.BoolFieldType:
                return "reader.ReadByte() != 0";
            case IFieldType.Int8FieldType:
                return "reader.ReadSByte()";
            case IFieldType.Int16FieldType:
                return "reader.ReadShort()";
            case IFieldType.UInt16FieldType:
                return "reader.ReadUShort()";
            case IFieldType.Int32FieldType:
                return "reader.ReadInt()";
            case IFieldType.UInt32FieldType:
                return "reader.ReadUInt()";
            case IFieldType.Int64FieldType:
                return "reader.ReadLong()";
            case IFieldType.UuidFieldType:
                return "reader.ReadGuid()";
            case IFieldType.Float64FieldType:
                return "reader.ReadDouble()";
            case IFieldType.StructType:
                return $"new {type.ClrName}Message(reader, version)";
            default:
                throw new Exception($"Unsupported field type {type}");
        }
    }

    private void GenerateVariableLengthReader(
        Versions fieldFlexibleVersions,
        string name,
        IFieldType type,
        Versions possibleVersions,
        Versions nullableVersions,
        string assignmentPrefix,
        string assignmentSuffix,
        bool isStructArrayWithKeys,
        bool zeroCopy)
    {
        var lengthVar = type.IsArray ? "arrayLength" : "length";

        _codeBuffer.AppendLine($"int {lengthVar};");
        VersionConditional
            .ForVersions(fieldFlexibleVersions, possibleVersions)
            .IfMember(_ => { _codeBuffer.AppendLine($"{lengthVar} = reader.ReadVarUInt() - 1;"); })
            .IfNotMember(
                _ =>
                {
                    if (type.IsString)
                    {
                        _codeBuffer.AppendLine($"{lengthVar} = reader.ReadShort();");
                    }
                    else if (type.IsBytes || type.IsArray || type.IsRecords)
                    {
                        _codeBuffer.AppendLine($"{lengthVar} = reader.ReadInt();");
                    }
                    else
                    {
                        throw new Exception($"Can't handle variable length type {type}");
                    }
                })
            .Generate(_codeBuffer);
        _codeBuffer.AppendLine($"if ({lengthVar} < 0)");
        _codeBuffer.AppendLine("{");
        _codeBuffer.IncrementIndent();
        VersionConditional
            .ForVersions(nullableVersions, possibleVersions)
            .IfNotMember(_ => { _codeBuffer.AppendLine($"throw new Exception(\"non-nullable field {name} was serialized as null\");"); })
            .IfMember(_ => { _codeBuffer.AppendLine($"{assignmentPrefix}null{assignmentSuffix}"); })
            .Generate(_codeBuffer);
        _codeBuffer.DecrementIndent();

        if (type.IsString)
        {
            _codeBuffer.AppendLine("}");
            _codeBuffer.AppendLine($"else if ({lengthVar} > 0x7fff)");
            _codeBuffer.AppendLine("{");
            _codeBuffer.IncrementIndent();
            _codeBuffer.AppendLine($"throw new Exception($\"string field {name} had invalid length {{{lengthVar}}}\");");
            _codeBuffer.DecrementIndent();
        }

        _codeBuffer.AppendLine("}");
        _codeBuffer.AppendLine("else");
        _codeBuffer.AppendLine("{");
        _codeBuffer.IncrementIndent();

        if (type.IsString)
        {
            _codeBuffer.AppendLine($"{assignmentPrefix}reader.ReadString({lengthVar}){assignmentSuffix}");
        }
        else if (type.IsBytes)
        {
            _codeBuffer.AppendLine($"{assignmentPrefix}reader.ReadBytes({lengthVar}){assignmentSuffix}");
        }
        else if (type.IsRecords)
        {
            _codeBuffer.AppendLine($"{assignmentPrefix}reader.ReadRecords({lengthVar}){assignmentSuffix}");
        }
        else if (type is IFieldType.ArrayType arrayType)
        {
            if (isStructArrayWithKeys)
            {
                _codeBuffer.AppendLine($"{FieldSpecification.CollectionType(arrayType.ElementType.ToString())} newCollection = new({lengthVar});");
            }
            else
            {
                var typeSuffix = arrayType.IsStructArray ? "Message" : "";
                _codeBuffer.AppendLine($"var newCollection = new List<{arrayType.ElementType.ClrName}{typeSuffix}>({lengthVar});");
            }

            _codeBuffer.AppendLine($"for (var i = 0; i< {lengthVar}; i++)");
            _codeBuffer.AppendLine("{");
            _codeBuffer.IncrementIndent();

            if (arrayType.ElementType.IsArray)
            {
                throw new Exception("Nested arrays are not supported. Use an array of structures containing another array.");
            }

            if (arrayType.ElementType.IsBytes || arrayType.ElementType.IsString)
            {
                GenerateVariableLengthReader(
                    fieldFlexibleVersions,
                    name + " element",
                    arrayType.ElementType,
                    possibleVersions,
                    Versions.None,
                    "newCollection.Add(",
                    ");",
                    false,
                    false);
            }
            else
            {
                _codeBuffer.AppendLine($"newCollection.Add({PrimitiveReadExpression(arrayType.ElementType)});");
            }

            _codeBuffer.DecrementIndent();
            _codeBuffer.AppendLine("}");
            _codeBuffer.AppendLine($"{assignmentPrefix}newCollection{assignmentSuffix}");
        }
        else
        {
            throw new Exception($"Can't handle variable length type {type}");
        }

        _codeBuffer.DecrementIndent();
        _codeBuffer.AppendLine("}");
    }
}