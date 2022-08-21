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

namespace NKafka.MessageGenerator;

public class WriteMethodGenerator: Generator, IWriteMethodGenerator
{
    private readonly ApiDescriptor _descriptor;

    public WriteMethodGenerator(ApiDescriptor descriptor)
    {
        _descriptor = descriptor;
    }

    public StringBuilder Generate(int startIndent = DEFAULT_INDENT)
    {
        IndentValue = startIndent;

        var builder = new StringBuilder();

        foreach (var field in _descriptor.Fields)
        {
            var fieldWriteCode = GenerateSerializeField(field, _descriptor.FlexibleVersions);
            builder.Append(fieldWriteCode);
        }

        return builder;
    }

    private StringBuilder GenerateSerializeField(FieldDescriptor field, Versions flexibleVersions)
    {
        var builder = new StringBuilder();

        if (field.Type.StartsWith("[]")) //array type
        {
            var type = field.Type[2..];

            if (IsSimpleType(type))
            {
                var arrayOfSimpleType = GenerateArrayOfSimpleType(flexibleVersions.Lowest, field.Name, type);
                builder.Append(arrayOfSimpleType);
            }
            else
            {
                var arrayOfComplexType = GenerateArrayOfComplexType(flexibleVersions.Lowest, field);
                builder.Append(arrayOfComplexType);
            }

            builder.AppendLine();
        }
        else //single type
        {
            if (IsSimpleType(field.Type))
            {
                var simpleType = GenerateSimpleType(field, flexibleVersions);
                builder.Append(simpleType);
            }
        }

        return builder;
    }

    private StringBuilder GenerateSimpleType(FieldDescriptor field, Versions minFlexibleVersion)
    {
        var builder = new StringBuilder();

        if (field.Type is "byte" or "bool") //no flexible
        {
        }

        return builder;
    }

    private StringBuilder GenerateArrayOfSimpleType(short minFlexibleVersion, string name, string type)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"{Indent}if (Version >= ApiVersions.Version{minFlexibleVersion})");
        builder.AppendLine($"{Indent}{{");

        IncrementIndent();
        builder.AppendLine($"{Indent}if ({name} is null)");
        builder.AppendLine($"{Indent}{{");

        IncrementIndent();
        builder.AppendLine($"{Indent}writer.WriteVarUInt(0);");
        builder.AppendLine($"{Indent}Size += 4;");
        DecrementIndent();

        builder.AppendLine($"{Indent}}}");
        builder.AppendLine($"{Indent}else");
        builder.AppendLine($"{Indent}{{");

        IncrementIndent();
        builder.AppendLine($"{Indent}writer.WriteVarUInt((uint){name}.Count + 1);");
        builder.AppendLine($"{Indent}foreach (var val in {name})");
        builder.AppendLine($"{Indent}{{");

        IncrementIndent();
        builder.AppendLine($"{Indent}writer.WriteVarInt(val);");
        DecrementIndent();

        builder.AppendLine($"{Indent}}}");
        DecrementIndent();

        builder.AppendLine($"{Indent}}}");
        DecrementIndent();

        builder.AppendLine($"{Indent}}}");
        builder.AppendLine($"{Indent}else");
        builder.AppendLine($"{Indent}{{");
        builder.AppendLine($"{Indent}}}");

        return builder;
    }

    private StringBuilder GenerateArrayOfComplexType(short minFlexibleVersion, FieldDescriptor field)
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

    private static IEnumerable<short> GetSetVersions(string versionString, short maxVersion = -1)
    {
        const short currentMaxVersion = 12;

        var set = new HashSet<short>();

        var vs = versionString.Split('-');

        short startIndex = 0;
        short endIndex = 0;

        switch (vs.Length)
        {
            case 2:
                startIndex = short.Parse(vs[0]);
                endIndex = maxVersion == -1 ? short.Parse(vs[1]) : maxVersion;

                break;
            case 1:
                if (versionString.EndsWith("+"))
                {
                    startIndex = short.Parse(versionString.TrimEnd('+'));
                    endIndex = maxVersion == -1 ? currentMaxVersion : maxVersion;
                }

                break;
        }

        for (var i = startIndex; i <= endIndex; i++)
        {
            set.Add(i);
        }

        return set;
    }

    private static bool IsSimpleType(string type)
    {
        return type is "int8" or "int16" or "int32" or "int64" or "uint16" or "uint32" or "string" or "float64" or "bool" or "uuid";
    }
}