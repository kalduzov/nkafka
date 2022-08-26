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

public class WriteMethodGenerator: Generator, IWriteMethodGenerator
{
    private readonly CodeBuffer _codeBuffer;

    public WriteMethodGenerator(CodeBuffer codeBuffer)
    {
        _codeBuffer = codeBuffer;
    }

    public StringBuilder Generate(IReadOnlyCollection<FieldSpecification> fields, int startIndent = DEFAULT_INDENT)
    {
        IndentValue = startIndent;

        var builder = new StringBuilder();

        foreach (var field in fields)
        {
            var fieldWriteCode = GenerateSerializeField(field);
            builder.Append(fieldWriteCode);
        }

        return builder;
    }

    private StringBuilder GenerateSerializeField(FieldSpecification field)
    {
        var builder = new StringBuilder();

        // if (field.Type.IsArray) //array type
        // {
        //     if (!field.Type.IsStruct)
        //     {
        //         var arrayOfSimpleType = GenerateArrayOfSimpleType(_descriptor.FlexibleVersions.Lowest, field.Name, field.Type);
        //         builder.Append(arrayOfSimpleType);
        //     }
        //     else
        //     {
        //         var arrayOfComplexType = GenerateArrayOfComplexType(_descriptor.FlexibleVersions.Lowest, field);
        //         builder.Append(arrayOfComplexType);
        //     }
        //
        //     builder.AppendLine();
        // }
        // else //single type
        // {
        //     if (!field.Type.IsStruct)
        //     {
        //         var simpleType = GenerateSimpleType(field, _descriptor.FlexibleVersions);
        //         builder.Append(simpleType);
        //     }
        // }

        return builder;
    }

    private StringBuilder GenerateSimpleType(FieldSpecification field, Versions minFlexibleVersion)
    {
        var builder = new StringBuilder();

        // if (field.Type is "byte" or "bool") //no flexible
        // {
        // }

        return builder;
    }

    private StringBuilder GenerateArrayOfSimpleType(short minFlexibleVersion, string name, IFieldType type)
    {
        var builder = new StringBuilder();

        // builder.AppendLine($"{Indent}if (Version >= ApiVersions.Version{minFlexibleVersion})");
        // builder.AppendLine($"{Indent}{{");
        //
        // IncrementIndent();
        //
        // if (type.CanBeNullable)
        // {
        //     builder.AppendLine($"{Indent}if ({name} is null)");
        //     builder.AppendLine($"{Indent}{{");
        //
        //     IncrementIndent();
        //     builder.AppendLine($"{Indent}writer.WriteVarUInt(0);");
        //
        //     // if (_descriptor.Type == MessageType.Request)
        //     // {
        //     //     builder.AppendLine($"{Indent}Size += {type.Size};");
        //     // }
        //
        //     DecrementIndent();
        //
        //     builder.AppendLine($"{Indent}}}");
        //     builder.AppendLine($"{Indent}else");
        //     builder.AppendLine($"{Indent}{{");
        // }
        //
        // IncrementIndent();
        // builder.AppendLine($"{Indent}writer.WriteVarUInt((uint){name}.Count + 1);");
        // builder.AppendLine($"{Indent}foreach (var val in {name})");
        // builder.AppendLine($"{Indent}{{");
        //
        // IncrementIndent();
        // builder.AppendLine($"{Indent}writer.WriteVarInt(val);");
        // DecrementIndent();
        //
        // builder.AppendLine($"{Indent}}}");
        // DecrementIndent();
        //
        // builder.AppendLine($"{Indent}}}");
        // DecrementIndent();
        //
        // builder.AppendLine($"{Indent}}}");
        // builder.AppendLine($"{Indent}else");
        // builder.AppendLine($"{Indent}{{");
        // builder.AppendLine($"{Indent}}}");

        return builder;
    }

    private StringBuilder GenerateArrayOfComplexType(short minFlexibleVersion, FieldSpecification field)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"{Indent}//flexible version");
        builder.AppendLine($"{Indent}if (Version >= ApiVersions.Version{minFlexibleVersion})");
        builder.AppendLine($"{Indent}{{");

        // IncrementIndent();
        // if (field.Ignorable ?? false)
        // {
        //     builder.AppendLine($"{Indent}if ({field.Name} is null)");
        //     builder.AppendLine($"{Indent}{{");
        //     
        //     IncrementIndent();
        //     builder.AppendLine($"{Indent}writer.WriteVarUInt(0);");
        //     DecrementIndent();
        //     
        //     builder.AppendLine($"{Indent}}}");
        //     builder.AppendLine($"{Indent}else");
        //     builder.AppendLine($"{Indent}{{");
        // }
        // DecrementIndent();

//         var indentC = field.Ignorable ?? false ? indent + 4 : indent;
//         
//         builder.AppendLine(
//             @$"{GenerateIndent(indentC + 4)}writer.WriteVarUInt((uint){field.Name}.Count + 1);
// {GenerateIndent(indentC + 4)}foreach (var element in {field.Name})
// {GenerateIndent(indentC + 4)}{{
// {GenerateIndent(indentC + 8)}element.Write(writer, version);
// {GenerateIndent(indentC + 4)}}}");
//
//         if (field.Ignorable ?? false)
//         {
//             builder.AppendLine($"{Indent + 4)}}}");
//         }
//
        builder.AppendLine($"{Indent}}}");
        builder.AppendLine($"{Indent}else //no flexible version");
        builder.AppendLine($"{Indent}{{");
        builder.AppendLine($"{Indent}}}");

        return builder;
    }

    public void Generate(string className, StructSpecification structSpecification, Versions parentVersions)
    {
        _codeBuffer.AppendLine("public override void Write(BufferWriter writer, ApiVersions version)");
        _codeBuffer.AppendLine("{");
        _codeBuffer.AppendLine("}");
    }
}