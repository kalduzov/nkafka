using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Logging;

using NKafka.Protocol;

namespace NKafka.MessageGenerator;

/// <summary>
/// Генератор классов для requests и responses из папки resources
/// </summary>
public class MessageGenerator: IMessageGenerator
{
    private const string _MESSAGE_SUFFIX = "Message";

    private readonly ApiDescriptor _apiDescriptor;
    private readonly IHeaderGenerator _headerGenerator;
    private readonly IClassGenerator _classGenerator;
    private readonly string _solutionDirectory;
    private readonly string _outputDirectory;
    private readonly string _testMessageProject;
    private readonly ILogger _logger;
    private static readonly Dictionary<int, string> _indents = new();

    public string ClassName { get; }

    public MessageGenerator(string solutionDirectory, string outputDirectory, string testMessageProject, ILogger logger)
    {
        _solutionDirectory = solutionDirectory;
        _outputDirectory = outputDirectory;
        _testMessageProject = testMessageProject;
        _logger = logger;
    }

    public MessageGenerator(ApiDescriptor apiDescriptor, IHeaderGenerator headerGenerator, IClassGenerator classGenerator)
    {
        _apiDescriptor = apiDescriptor;
        _headerGenerator = headerGenerator;
        _classGenerator = classGenerator;

        ClassName = $"{apiDescriptor.Name}{_MESSAGE_SUFFIX}";
    }

    public StringBuilder Generate()
    {
        var builder = new StringBuilder();

        _headerGenerator.AppendUsing("System.Text");
        _headerGenerator.AppendUsing("NKafka.Protocol");
        _headerGenerator.AppendUsing("NKafka.Protocol.Records");
        _headerGenerator.AppendUsing("NKafka.Protocol.Extensions");

        var header = _headerGenerator.Generate();
        builder.Append(header);
        var @class = _classGenerator.Generate();
        builder.Append(@class);

        return builder;
    }

    // private void GenerateClass(ApiDescriptor apiDescriptor)
    // {
    //     const string messageSuffix = "Message";
    //
    //     switch (apiDescriptor.Type)
    //     {
    //         case ApiMessageType.Request or ApiMessageType.Response:
    //         {
    //             var className = $"{apiDescriptor.Name}{messageSuffix}";
    //             var content = GenerateApiMessageClass(className, apiDescriptor);
    //             var fileName = $"{className}.cs";
    //             WriteContentToFile(fileName, content);
    //
    //             var testClassName = $"{className}Tests";
    //             var testContent = GenerateTestClass(className, apiDescriptor);
    //             var testFileName = $"{testClassName}.cs";
    //             WriteTestToFile(testFileName, testContent);
    //
    //             break;
    //         }
    //         case ApiMessageType.Data:
    //         {
    //             _logger?.LogWarning("Тип data пока не реализован");
    //
    //             break;
    //         }
    //         default:
    //             _logger?.LogWarning("Неизвестный тип {ApiDescriptorType}", apiDescriptor.Type);
    //
    //             break;
    //     }
    // }

    private static StringBuilder GenerateTestClass(string className, ApiDescriptor apiDescriptor)
    {
        const int defaultIndent = 4;

        var builder = new StringBuilder();
        var fileHeader = GenerateTestFileHeader(className, apiDescriptor);
        builder.Append(fileHeader);

        var testMethod = GenerateTestMethod(defaultIndent, className, apiDescriptor);
        builder.Append(testMethod);

        builder.AppendLine("}");

        return builder;
    }

    private static StringBuilder GenerateTestMethod(int indent, string className, ApiDescriptor apiDescriptor)
    {
        var builder = new StringBuilder();
        var validVersions = GetSetVersions(apiDescriptor.ValidVersions);

        builder.AppendLine($"{GenerateIndent(indent)}[Theory(DisplayName = \"Check serialize and deserialize '{className}' message\")]");

        foreach (var version in validVersions)
        {
            builder.AppendLine($"{GenerateIndent(indent)}[InlineData(ApiVersions.Version{version})]");
        }

        builder.AppendLine($"{GenerateIndent(indent)}public void SerializeAndDeserializeMessage_Success(ApiVersions version)");
        builder.AppendLine($"{GenerateIndent(indent)}{{");
        builder.AppendLine($"{GenerateIndent(indent + 4)}var message = new {className}");
        builder.AppendLine($"{GenerateIndent(indent + 4)}{{");
        builder.AppendLine($"{GenerateIndent(indent + 4)}}};");
        builder.AppendLine($"{GenerateIndent(indent + 4)}SerializeAndDeserializeMessage(message, version);");
        builder.AppendLine($"{GenerateIndent(indent)}}}");

        return builder;
    }

    private static StringBuilder GenerateTestFileHeader(string className, ApiDescriptor apiDescriptor)
    {
        var builder = new StringBuilder();

        var licenseInfo = GenerateLicenseInfo();
        builder.Append(licenseInfo);

        builder.AppendLine();
        builder.AppendLine("using NKafka.Messages;");
        builder.AppendLine("using NKafka.Protocol;");
        builder.AppendLine();
        builder.AppendLine("using Xunit;");
        builder.AppendLine();
        builder.AppendLine("namespace NKafka.Tests.Messages;");
        builder.AppendLine();

        switch (apiDescriptor.Type)
        {
            case ApiMessageType.Response:
                builder.Append($"public class {className}Tests: ResponseMessageTests<{className}>");

                break;
            case ApiMessageType.Request:
                builder.Append($"public class {className}Tests: RequestMessageTests<{className}>");

                break;
            case ApiMessageType.Header:
                break;
            case ApiMessageType.Metadata:
                break;
            case ApiMessageType.Data:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        builder.AppendLine();
        builder.AppendLine("{");

        return builder;
    }

    private void WriteTestToFile(string fileName, StringBuilder content)
    {
        var path = Path.Combine(_solutionDirectory, "tests", _testMessageProject, "Messages");
        Directory.CreateDirectory(path);

        path = Path.Combine(path, fileName);
        File.WriteAllText(path, content.ToString(), Encoding.UTF8);
    }

    private void WriteContentToFile(string fileName, StringBuilder content)
    {
        var path = Path.Combine(_outputDirectory, fileName);
        Directory.CreateDirectory(_outputDirectory);
        File.WriteAllText(path, content.ToString(), Encoding.UTF8);
    }

    private static ApiDescriptor GetApiDescriptor(string fileName)
    {
        var str = string.Empty;

        using var fileStream = File.OpenText(fileName);
        {
            while (!fileStream.EndOfStream)
            {
                var line = fileStream.ReadLine()?.Trim();

                if (line.StartsWith("//"))
                {
                    continue;
                }

                str += line;
            }
        }

        try
        {
            return JsonSerializer.Deserialize<ApiDescriptor>(
                str,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                })!;
        }
        catch (Exception exc)
        {
            throw new FormatException($"Can't parse file {fileName}", exc);
        }
    }

    private static StringBuilder GenerateApiMessageClass(string className, ApiDescriptor apiDescriptor)
    {
        const int defaultIndent = 4;

        var builder = new StringBuilder();

        var header = GenerateFileHeader(className, apiDescriptor);
        builder.Append(header);

        var fields = GenerateProperties(defaultIndent, apiDescriptor.Fields, apiDescriptor.Type);
        builder.Append(fields);

        var ctor = GenerateCtor(defaultIndent, className, apiDescriptor);
        builder.Append(ctor);

        var decodeMethod = GenerateReadMethod(defaultIndent, apiDescriptor);
        builder.Append(decodeMethod);

        var writeMethod = GenerateWriteMethod(defaultIndent, apiDescriptor.Fields, apiDescriptor.FlexibleVersions);
        builder.Append(writeMethod);

        var internalClasses = GetAllInternalClasses(apiDescriptor.Fields);
        var subclasses = GenerateSubclasses(defaultIndent, internalClasses, apiDescriptor);
        builder.Append(subclasses);
        builder.AppendLine();

        builder.AppendLine("}");

        return builder;
    }

    private static StringBuilder GenerateWriteMethod(int indent, List<FieldDescriptor> fields, string flexibleVersions)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"{GenerateIndent(indent)}public override void Write(BufferWriter writer, ApiVersions version)");
        builder.AppendLine($"{GenerateIndent(indent)}{{");

        if (fields is not null)
        {
            foreach (var field in fields)
            {
                var result = GenerateSerializeField(indent + 4, field, flexibleVersions);
                builder.Append(result);
            }

            builder.AppendLine();
        }

        builder.AppendLine($"{GenerateIndent(indent)}}}");
        builder.AppendLine();

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

    private static StringBuilder GenerateSerializeField(int indent, FieldDescriptor field, string flexibleVersions)
    {
        var minFlexibleVersion = GetSetVersions(flexibleVersions).Min();
        var builder = new StringBuilder();

        if (field.Type.StartsWith("[]"))
        {
            var type = field.Type[2..];

            if (IsSimpleType(type))
            {
                var arrayOfSimpleType = GenerateArrayOfSimpleType(indent, minFlexibleVersion, field.Name, type);
                builder.Append(arrayOfSimpleType);
            }
            else
            {
                var arrayOfComplexType = GenerateArrayOfComplexType(indent, minFlexibleVersion, field);
                builder.Append(arrayOfComplexType);
            }

            builder.AppendLine();
        }

        return builder;
    }

    private static StringBuilder GenerateArrayOfSimpleType(int indent, short minFlexibleVersion, string name, string type)
    {
        var builder = new StringBuilder();

        //var t =sim

        builder.Append(
            @$"{GenerateIndent(indent)}if (Version >= ApiVersions.Version{minFlexibleVersion})
{GenerateIndent(indent)}{{
{GenerateIndent(indent + 4)}if ({name} is null)
{GenerateIndent(indent + 4)}{{
{GenerateIndent(indent + 8)}writer.WriteVarUInt(0);      
{GenerateIndent(indent + 4)}}}
{GenerateIndent(indent + 4)}else
{GenerateIndent(indent + 4)}{{  
{GenerateIndent(indent + 8)}writer.WriteVarUInt((uint){name}.Count + 1);
{GenerateIndent(indent + 8)}foreach (var val in {name})
{GenerateIndent(indent + 8)}{{
{GenerateIndent(indent + 12)}writer.WriteVarInt(val);
{GenerateIndent(indent + 8)}}}
{GenerateIndent(indent + 4)}}}
{GenerateIndent(indent)}}}
{GenerateIndent(indent)}else
{GenerateIndent(indent)}{{
{GenerateIndent(indent)}}}");

        return builder;
    }

    private static StringBuilder GenerateArrayOfComplexType(int indent, short minFlexibleVersion, FieldDescriptor field)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"{GenerateIndent(indent)}//flexible version");
        builder.AppendLine($"{GenerateIndent(indent)}if (Version >= ApiVersions.Version{minFlexibleVersion})");
        builder.AppendLine($"{GenerateIndent(indent)}{{");

        if (field.Ignorable ?? false)
        {
            builder.AppendLine($"{GenerateIndent(indent + 4)}if ({field.Name} is null)");
            builder.AppendLine($"{GenerateIndent(indent + 4)}{{");
            builder.AppendLine($"{GenerateIndent(indent + 8)}writer.WriteVarUInt(0);");
            builder.AppendLine($"{GenerateIndent(indent + 4)}}}");
            builder.AppendLine($"{GenerateIndent(indent + 4)}else");
            builder.AppendLine($"{GenerateIndent(indent + 4)}{{");
        }

        var indentC = field.Ignorable ?? false ? indent + 4 : indent;
        builder.AppendLine(
            @$"{GenerateIndent(indentC + 4)}writer.WriteVarUInt((uint){field.Name}.Count + 1);
{GenerateIndent(indentC + 4)}foreach (var element in {field.Name})
{GenerateIndent(indentC + 4)}{{
{GenerateIndent(indentC + 8)}element.Write(writer, version);
{GenerateIndent(indentC + 4)}}}");

        if (field.Ignorable ?? false)
        {
            builder.AppendLine($"{GenerateIndent(indent + 4)}}}");
        }

        builder.AppendLine($"{GenerateIndent(indent)}}}");
        builder.AppendLine(
            @$"{GenerateIndent(indent)}else //no flexible version
{GenerateIndent(indent)}{{
{GenerateIndent(indent)}}}");

        return builder;
    }

    private static Dictionary<string, List<FieldDescriptor>> GetAllInternalClasses(List<FieldDescriptor> fields)
    {
        var result = new Dictionary<string, List<FieldDescriptor>>();

        if (fields is null || fields.Count == 0)
        {
            return result;
        }

        foreach (var field in fields)
        {
            var type = field.Type;

            if (type is "bytes" or "records")
            {
                continue;
            }

            if (field.Type.Contains("[]"))
            {
                type = field.Type[2..];
            }

            if (!IsSimpleType(type))
            {
                result.TryAdd(type, field.Fields);
            }

            var res = GetAllInternalClasses(field.Fields);

            foreach (var re in res)
            {
                result.TryAdd(re.Key, re.Value);
            }
        }

        return result;
    }

    private static StringBuilder GenerateCtor(int indent, string className, ApiDescriptor apiDescriptor, bool isSubclass = false)
    {
        var builder = new StringBuilder();

        var validVersions = GetSetVersions(apiDescriptor.ValidVersions);
        var apiKey = (ApiKeys)apiDescriptor.ApiKey;

        switch (apiDescriptor.Type)
        {
            case ApiMessageType.Request:
            {
                builder.Append($"{GenerateIndent(indent)}public {className}()");
                builder.AppendLine();
                builder.AppendLine($"{GenerateIndent(indent)}{{");

                if (!isSubclass)
                {
                    builder.AppendLine($"{GenerateIndent(indent + 4)}ApiKey = ApiKeys.{apiKey};");
                }

                builder.AppendLine($"{GenerateIndent(indent + 4)}LowestSupportedVersion = ApiVersions.Version{validVersions.Min()};");
                builder.AppendLine($"{GenerateIndent(indent + 4)}HighestSupportedVersion = ApiVersions.Version{validVersions.Max()};");
                builder.AppendLine($"{GenerateIndent(indent)}}}");
                builder.AppendLine();

                break;
            }
            case ApiMessageType.Response:
            {
                builder.Append($"{GenerateIndent(indent)}public {className}()");
                builder.AppendLine();
                builder.AppendLine($"{GenerateIndent(indent)}{{");
                builder.AppendLine($"{GenerateIndent(indent + 4)}LowestSupportedVersion = ApiVersions.Version{validVersions.Min()};");
                builder.AppendLine($"{GenerateIndent(indent + 4)}HighestSupportedVersion = ApiVersions.Version{validVersions.Max()};");
                builder.AppendLine($"{GenerateIndent(indent)}}}");
                builder.AppendLine();

                builder.AppendLine($"{GenerateIndent(indent)}public {className}(BufferReader reader, ApiVersions version)");
                builder.Append($"{GenerateIndent(indent + 4)}: base(reader, version)");
                builder.AppendLine();
                builder.AppendLine($"{GenerateIndent(indent)}{{");
                builder.AppendLine($"{GenerateIndent(indent + 4)}LowestSupportedVersion = ApiVersions.Version{validVersions.Min()};");
                builder.AppendLine($"{GenerateIndent(indent + 4)}HighestSupportedVersion = ApiVersions.Version{validVersions.Max()};");
                builder.AppendLine($"{GenerateIndent(indent)}}}");
                builder.AppendLine();

                break;
            }
        }

        return builder;
    }

    private static StringBuilder GenerateReadMethod(int indent, ApiDescriptor apiDescriptor)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"{GenerateIndent(indent)}public override void Read(BufferReader reader, ApiVersions version)");
        builder.AppendLine($"{GenerateIndent(indent)}{{");
        builder.AppendLine($"{GenerateIndent(indent)}}}");
        builder.AppendLine();

        return builder;
    }

    private static StringBuilder GenerateSubclasses(
        int indent,
        Dictionary<string, List<FieldDescriptor>> complexClasses,
        ApiDescriptor apiDescriptor)
    {
        var builder = new StringBuilder();

        foreach (var complexClass in complexClasses)
        {
            var className = $"{complexClass.Key}Message";
            builder.Append($"{GenerateIndent(indent)}public class {className}: Message")
                .AppendLine()
                .AppendLine($"{GenerateIndent(indent)}{{");

            var fields = GenerateProperties(indent + 4, complexClass.Value, ApiMessageType.None);
            builder.Append(fields);

            var constructor = GenerateCtor(indent + 4, className, apiDescriptor, true);
            builder.Append(constructor);

            var decodeMethod = GenerateReadMethod(indent + 4, apiDescriptor);
            builder.Append(decodeMethod);
            builder.AppendLine();

            var writeMethod = GenerateWriteMethod(indent + 4, complexClass.Value, apiDescriptor.FlexibleVersions);
            builder.Append(writeMethod);
            builder.AppendLine().AppendLine($"{GenerateIndent(indent)}}}");
        }

        return builder;
    }

    private static HashSet<short> GetSetVersions(string versionString, short maxVersion = -1)
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

    private static StringBuilder GenerateProperties(
        int indent,
        List<FieldDescriptor> fields,
        ApiMessageType messageType)
    {
        var builder = new StringBuilder();

        if (fields is null || fields.Count == 0)
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

            var field = GenerateField(indent, fieldDescriptor);
            builder.Append(field);
            builder.AppendLine();

            if (fieldDescriptor != lastField)
            {
                builder.AppendLine();
            }
        }

        builder.AppendLine();

        return builder;
    }

    private static StringBuilder GenerateField(int indent, FieldDescriptor field)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"{GenerateIndent(indent)}/// <summary>");
        builder.AppendLine($"{GenerateIndent(indent)}/// {field.About}");
        builder.AppendLine($"{GenerateIndent(indent)}/// </summary>");

        var fieldType = GeneratePropertyType(field.Type);

        var nullableMarker = NullableMarkerIfNeeded(field);
        builder.Append($"{GenerateIndent(indent)}public {fieldType}{nullableMarker} {field.Name} {{ get; set; }}");

        var defaultValue = GetDefaultValue(field.Default, field.Ignorable, field.Type, fieldType);

        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            builder.Append($" = {defaultValue};");
        }

        return builder;
    }

    private static string NullableMarkerIfNeeded(FieldDescriptor field)
    {
        return field.Ignorable.HasValue && field.Ignorable.Value
               || field.Default is not null && field.Default.Equals("null", StringComparison.OrdinalIgnoreCase)
            ? "?"
            : string.Empty;
    }

    private static string GetDefaultValue(string? @default, bool? ignorable, string type, string fieldType)
    {
        return @default;
    }

    private static bool IsSimpleType(string type)
    {
        return type is "int8" or "int16" or "int32" or "int64" or "uint16" or "uint32" or "string" or "float64" or "bool" or "uuid";
    }

    private static string GeneratePropertyType(string fieldType)
    {
        if (fieldType == "bytes")
        {
            return "byte[]";
        }

        if (fieldType == "records")
        {
            return "RecordBatch";
        }

        if (fieldType.StartsWith("[]"))
        {
            var typeName = fieldType[2..];

            if (IsSimpleType(typeName))
            {
                typeName = MapTypeToClrType(typeName);
            }
            else
            {
                typeName += "Message";
            }

            return $"IReadOnlyCollection<{typeName}>";
        }

        return IsSimpleType(fieldType) ? MapTypeToClrType(fieldType) : $"{fieldType}Message";
    }

    private static string MapTypeToClrType(string type)
    {
        return type switch
        {
            "int8" => "sbyte",
            "int16" => "short",
            "int32" => "int",
            "int64" => "long",
            "uint16" => "ushort",
            "uint32" => "uint",
            "string" => "string",
            "float64" => "double",
            "bool" => "bool",
            "uuid" => "Guid",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private static string MapTypeToSerializeMethod(string type)
    {
        return type switch
        {
            "int8" => "Int",
            "int16" => "Short",
            "int32" => "int",
            "int64" => "long",
            "uint16" => "ushort",
            "uint32" => "uint",
            "string" => "string",
            "float64" => "double",
            "bool" => "bool",
            "uuid" => "Guid",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// Генерируется заголовов файла
    /// </summary>
    private static StringBuilder GenerateFileHeader(string mainClassName, ApiDescriptor apiDescriptor)
    {
        var builder = new StringBuilder();

        var licenseInfo = GenerateLicenseInfo();
        builder.Append(licenseInfo);

        builder.AppendLine("using System.Text;");
        builder.AppendLine();
        builder.AppendLine("using NKafka.Protocol;");
        builder.AppendLine("using NKafka.Protocol.Records;");
        builder.AppendLine("using NKafka.Protocol.Extensions;");
        builder.AppendLine();
        builder.AppendLine("namespace NKafka.Messages;");
        builder.AppendLine();

        switch (apiDescriptor.Type)
        {
            case ApiMessageType.Response:
                builder.Append($"public partial class {mainClassName}: ResponseMessage");

                break;
            case ApiMessageType.Request:
                builder.Append($"public partial class {mainClassName}: RequestMessage");

                break;
            case ApiMessageType.Header:
                break;
            case ApiMessageType.Metadata:
                break;
            case ApiMessageType.Data:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        builder.AppendLine();
        builder.AppendLine("{");

        return builder;
    }

    private static string GenerateLicenseInfo()
    {
        return @"//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the ""License"");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.";
    }

    private IEnumerable<string> GetFileResources()
    {
        var pathToResources = Path.Combine(Directory.GetCurrentDirectory(), _solutionDirectory, "resources", "message");

        return Directory.GetFiles(pathToResources, "*.json");
    }

    private static string GenerateIndent(int indent = 0)
    {
        if (_indents.TryGetValue(indent, out var result))
        {
            return result;
        }

        result = string.Empty;

        for (var i = 0; i < indent; i++)
        {
            result += " ";
        }

        _indents.TryAdd(indent, result);

        return result;
    }
}

// internal record FieldMetadata(
//     string Name,
//     string Type,
//     bool IsSimple,
//     short minVersion,
//     short maxVersion,
//     short minFlex,
//     short maxFlex);