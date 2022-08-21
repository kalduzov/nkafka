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

using NKafka.Protocol;

namespace NKafka.MessageGenerator;

public class ClassGenerator: Generator, IClassGenerator
{
    private readonly ApiDescriptor _descriptor;
    private readonly IWriteMethodGenerator _writeMethodGenerator;
    private readonly IReadMethodGenerator _readMethodGenerator;
    private const string _MESSAGE_SUFFIX = "Message";
    private const string _CLASS_TEMPLATE = "public partial class {0}: {1}\r\n{{\r\n{2}}}";
    private const string _EMPTY_CLASS_TEMPLATE = "public partial class {0}\r\n{{\r\n}}";

    private readonly Dictionary<IFieldType, List<FieldDescriptor>> _internalClasses;

    public ClassGenerator(ApiDescriptor descriptor, IWriteMethodGenerator writeMethodGenerator, IReadMethodGenerator readMethodGenerator)
    {
        _descriptor = descriptor;
        _writeMethodGenerator = writeMethodGenerator;
        _readMethodGenerator = readMethodGenerator;
        _internalClasses = GetAllInternalClasses(descriptor.Fields);
    }

    public StringBuilder Generate()
    {
        switch (_descriptor.Type)
        {
            case ApiMessageType.Request or ApiMessageType.Response:
            {
                var className = $"{_descriptor.Name}{_MESSAGE_SUFFIX}";

                return GenerateRequestResponseMessage(className);
            }
            default:
                return GenerateEmptyClass();
        }
    }

    private StringBuilder GenerateEmptyClass()
    {
        var builder = new StringBuilder(_descriptor.Name.Length + _EMPTY_CLASS_TEMPLATE.Length);

        builder.AppendFormat(_EMPTY_CLASS_TEMPLATE, _descriptor.Name);

        return builder;
    }

    private StringBuilder GenerateRequestResponseMessage(string className)
    {
        var builder = new StringBuilder();

        var baseClassName = _descriptor.Type + _MESSAGE_SUFFIX;
        var body = GenerateBody(className, _descriptor);
        builder.AppendFormat(_CLASS_TEMPLATE, className, baseClassName, body);

        return builder;
    }

    private StringBuilder GenerateBody(string className, ApiDescriptor descriptor)
    {
        var builder = new StringBuilder();

        var fields = GenerateProperties(descriptor.Fields, descriptor.Type);
        builder.Append(fields);
        builder.AppendLine();

        var ctor = GenerateCtor(className, descriptor);
        builder.Append(ctor);
        builder.AppendLine();

        var readMethod = GenerateReadMethod(descriptor.Fields);
        builder.Append(readMethod);
        builder.AppendLine();

        var writeMethod = GenerateWriteMethod(descriptor.Fields);
        builder.Append(writeMethod);

        if (!_internalClasses.Any())
        {
            return builder;
        }

        builder.AppendLine();
        var subclasses = GenerateSubclasses();
        builder.Append(subclasses);

        return builder;
    }

    private StringBuilder GenerateSubclasses()
    {
        var builder = new StringBuilder();

        IncrementIndent();

        foreach (var @class in _internalClasses)
        {
            var className = $"{@class.Key.ClrName}";

            builder
                .AppendLine($"{Indent}public class {className}: Message")
                .AppendLine($"{Indent}{{");

            var fields = GenerateProperties(@class.Value, ApiMessageType.None);
            builder.Append(fields);
            builder.AppendLine();

            var constructor = GenerateCtor(className, _descriptor, true);
            builder.Append(constructor);
            builder.AppendLine();

            var decodeMethod = GenerateReadMethod(@class.Value);
            builder.Append(decodeMethod);
            builder.AppendLine();

            var writeMethod = GenerateWriteMethod(@class.Value);
            builder.Append(writeMethod);
            builder.AppendLine($"{Indent}}}");
        }

        return builder;
    }

    private StringBuilder GenerateWriteMethod(List<FieldDescriptor> fields)
    {
        IncrementIndent();
        var builder = new StringBuilder();

        builder.AppendLine($"{Indent}public override void Write(BufferWriter writer, ApiVersions version)");
        builder.AppendLine($"{Indent}{{");

        IncrementIndent();
        var writeBody = _writeMethodGenerator.Generate(fields, IndentValue);
        builder.Append(writeBody);
        DecrementIndent();

        // if (fields is not null)
        // {
        //     foreach (var field in fields)
        //     {
        //         var result = GenerateSerializeField(indent + 4, field, flexibleVersions);
        //         builder.Append(result);
        //     }
        //
        //     builder.AppendLine();
        // }

        builder.AppendLine($"{Indent}}}");
        DecrementIndent();

        // builder.AppendFormat(
        //     "  internal {0}Message Build{0}(ReadOnlySpan<byte> span, ApiVersions apiVersion, int responseLength)",
        //     apiDescriptor.Name);
        //
        // builder.AppendLine();
        // builder.AppendLine("  {");
        // builder.AppendLine("    var reader = new BufferReader(span);");
        //
        // builder.AppendLine();
        //
        // var validVersions = GetSetVersions(apiDescriptor.ValidVersions);
        // var flexibleVersions = GetSetVersions(apiDescriptor.FlexibleVersions, validVersions.Max());
        //
        // foreach (var fieldDescriptor in apiDescriptor.Fields)
        // {
        //     var fieldDecode = GenerateDecodeField(fieldDescriptor, flexibleVersions, validVersions);
        //     builder.Append(fieldDecode);
        //     builder.AppendLine();
        // }
        //

        return builder;
    }

    private StringBuilder GenerateReadMethod(List<FieldDescriptor> fields)
    {
        var builder = new StringBuilder();

        IncrementIndent();
        builder.AppendLine($"{Indent}public override void Read(BufferReader reader, ApiVersions version)");
        builder.AppendLine($"{Indent}{{");

        IncrementIndent();
        var readBody = _readMethodGenerator.Generate(fields, IndentValue);
        builder.Append(readBody);
        DecrementIndent();

        builder.AppendLine($"{Indent}}}");
        DecrementIndent();

        return builder;
    }

    private StringBuilder GenerateProperties(List<FieldDescriptor> fields, ApiMessageType messageType)
    {
        IncrementIndent();
        var builder = new StringBuilder();

        if (fields.Count == 0)
        {
            return builder;
        }

        var lastField = fields.Last();

        foreach (var fieldDescriptor in fields)
        {
            if (messageType == ApiMessageType.Response && fieldDescriptor.Name == "ThrottleTimeMs")
            {
                continue; //Данное поле есть в базовом классе
            }

            var field = GenerateField(fieldDescriptor);
            builder.Append(field);
            builder.AppendLine();

            if (fieldDescriptor != lastField)
            {
                builder.AppendLine();
            }
        }

        DecrementIndent();

        return builder;
    }

    private StringBuilder GenerateCtor(string className, ApiDescriptor apiDescriptor, bool isSubclass = false)
    {
        IncrementIndent();
        var builder = new StringBuilder();

        var apiKey = (ApiKeys)apiDescriptor.ApiKey;

        switch (apiDescriptor.Type)
        {
            case ApiMessageType.Request:
            {
                builder.Append($"{Indent}public {className}()");
                builder.AppendLine();
                builder.AppendLine($"{Indent}{{");

                if (!isSubclass)
                {
                    IncrementIndent();
                    builder.AppendLine($"{Indent}ApiKey = ApiKeys.{apiKey};");
                    DecrementIndent();
                }

                IncrementIndent();
                builder.AppendLine($"{Indent}LowestSupportedVersion = ApiVersions.Version{_descriptor.ValidVersions.Lowest};");
                builder.AppendLine($"{Indent}HighestSupportedVersion = ApiVersions.Version{_descriptor.ValidVersions.Highest};");
                DecrementIndent();

                builder.AppendLine($"{Indent}}}");

                break;
            }
            case ApiMessageType.Response:
            {
                builder.Append($"{Indent}public {className}()");
                builder.AppendLine();
                builder.AppendLine($"{Indent}{{");

                IncrementIndent();
                builder.AppendLine($"{Indent}LowestSupportedVersion = ApiVersions.Version{_descriptor.ValidVersions.Lowest};");
                builder.AppendLine($"{Indent}HighestSupportedVersion = ApiVersions.Version{_descriptor.ValidVersions.Highest};");
                DecrementIndent();

                builder.AppendLine($"{Indent}}}");
                builder.AppendLine();

                builder.AppendLine($"{Indent}public {className}(BufferReader reader, ApiVersions version)");

                IncrementIndent();
                builder.Append($"{Indent}: base(reader, version)");
                DecrementIndent();

                builder.AppendLine();
                builder.AppendLine($"{Indent}{{");

                IncrementIndent();
                builder.AppendLine($"{Indent}LowestSupportedVersion = ApiVersions.Version{_descriptor.ValidVersions.Lowest};");
                builder.AppendLine($"{Indent}HighestSupportedVersion = ApiVersions.Version{_descriptor.ValidVersions.Highest};");
                DecrementIndent();

                builder.AppendLine($"{Indent}}}");

                break;
            }
        }

        DecrementIndent();

        return builder;
    }

    private StringBuilder GenerateField(FieldDescriptor field)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"{Indent}/// <summary>");
        builder.AppendLine($"{Indent}/// {field.About}");
        builder.AppendLine($"{Indent}/// </summary>");

        var clrFullType = GetClrType(field);

        var nullableMarker = NullableMarkerIfNeeded(field);
        builder.Append($"{Indent}public {clrFullType}{nullableMarker} {field.Name} {{ get; set; }}");

        var defaultValue = GetDefaultValue(field.Default, field.Ignorable, field.Type);

        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            builder.Append($" = {defaultValue};");
        }

        return builder;
    }

    private string GetClrType(FieldDescriptor field)
    {
        if (!field.Type.IsArray)
        {
            return field.Type.ClrName;
        }

        if (field.MapKey ?? false)
        {
            return $"IReadOnlyDictionary<{field.Type.ClrName}>"; //todo тут должен быть словарь
        }

        return $"IReadOnlyCollection<{field.Type.ClrName}>";
    }

    private static string NullableMarkerIfNeeded(FieldDescriptor field)
    {
        return field.Ignorable.HasValue && field.Ignorable.Value
               || field.Default is not null && field.Default.Equals("null", StringComparison.OrdinalIgnoreCase)
            ? "?"
            : string.Empty;
    }

    private static string GetDefaultValue(string? @default, bool? ignorable, IFieldType type)
    {
        return @default;
    }

    private static Dictionary<IFieldType, List<FieldDescriptor>> GetAllInternalClasses(List<FieldDescriptor> fields)
    {
        var result = new Dictionary<IFieldType, List<FieldDescriptor>>();

        if (fields.Count == 0)
        {
            return result;
        }

        foreach (var field in fields)
        {
            if (!field.Type.IsStruct && !field.Type.IsStructArray)
            {
                continue;
            }

            result.TryAdd(field.Type, field.Fields);

            var res = GetAllInternalClasses(field.Fields);

            foreach (var re in res)
            {
                result.TryAdd(re.Key, re.Value);
            }
        }

        return result;
    }
}