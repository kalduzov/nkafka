using System.Text;
using System.Text.Json;

namespace Microlibs.Kafka.BuildTasks;

public class MessageGenerator : IMessageGenerator
{
    private readonly string _solutionDirectory;
    private readonly string _outputDirectory;

    public MessageGenerator(string solutionDirectory, string outputDirectory)
    {
        _solutionDirectory = solutionDirectory;
        _outputDirectory = outputDirectory;
    }

    public void Generate()
    {
        var files = GetFileResources();

        foreach (var fileName in files)
        {
            if (fileName.EndsWith("Request.json", StringComparison.OrdinalIgnoreCase))
            {
                GenerateRequest(fileName);

                continue;
            }

            if (fileName.EndsWith("Response.json", StringComparison.OrdinalIgnoreCase))
            {
                GenerateResponse(fileName);

                continue;
            }
        }
    }

    private void GenerateResponse(string fileName)
    {
        var str = string.Empty;

        using var fileStream = File.OpenText(fileName);
        {
            while (!fileStream.EndOfStream)
            {
                var line = fileStream.ReadLine().Trim();

                if (line.StartsWith("//"))
                {
                    continue;
                }

                str += line;
            }
        }

        try
        {
            var apiDescriptor = JsonSerializer.Deserialize<ApiDescriptor>(str);
            var content = GenerateResponseClass(apiDescriptor);

            var path = Path.Combine(_outputDirectory, apiDescriptor.Name + "Message.cs");
            Directory.CreateDirectory(_outputDirectory);
            File.WriteAllText(path, content.ToString(), Encoding.UTF8);
        }
        catch (Exception exc)
        {
            throw new FormatException($"Can't parse file {fileName}", exc);
        }
    }

    private static StringBuilder GenerateResponseClass(ApiDescriptor apiDescriptor)
    {
        if (!apiDescriptor.Type.Equals("response", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Этот метод генерирует только response, а в описании тип отличается");
        }

        var builder = new StringBuilder();

        var header = GenerateHeader(apiDescriptor);
        builder.Append(header);
        var fields = GenerateFields(apiDescriptor.Fields);
        builder.Append(fields);
        builder.AppendLine();
        var methods = GenerateDecodeMethod(apiDescriptor);
        builder.Append(methods);
        builder.AppendLine("}");

        return builder;
    }

    private static StringBuilder GenerateDecodeMethod(ApiDescriptor apiDescriptor)
    {
        var builder = new StringBuilder();

        builder.AppendFormat(
            "  internal static {0}Message Build{0}(ReadOnlySpan<byte> span, ApiVersions apiVersion, int responseLength)",
            apiDescriptor.Name);

        builder.AppendLine();
        builder.AppendLine("  {");
        builder.AppendLine("    var reader = new KafkaBufferReader(span);");

        builder.AppendLine();

        var validVersions = GetSetVersions(apiDescriptor.ValidVersions);
        var flexibleVersions = GetSetVersions(apiDescriptor.FlexibleVersions, validVersions.Max());

        foreach (var fieldDescriptor in apiDescriptor.Fields)
        {
            var fieldDecode = GenerateDecodeField(fieldDescriptor, flexibleVersions, validVersions);
            builder.Append(fieldDecode);
            builder.AppendLine();
        }

        builder.AppendLine("  }");
        builder.AppendLine();

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

    private static StringBuilder GenerateDecodeField(
        FieldDescriptor fieldDescriptor,
        HashSet<short> flexibleVersions,
        HashSet<short> validVersion)
    {
        var builder = new StringBuilder();

        var versions = GetSetVersions(fieldDescriptor.Versions, validVersion.Max());

        var field = IsSimpleType(fieldDescriptor.Type, out var typeName)
            ? GenerateDecodeSimpleField(fieldDescriptor.Name, typeName, validVersion, flexibleVersions, versions)
            : GenerateDecodeComplexField(fieldDescriptor.Name, typeName, validVersion, flexibleVersions, versions);

        builder.Append(field);

        return builder;
    }

    private static StringBuilder GenerateDecodeComplexField(
        string name,
        string type,
        HashSet<short> validVersion,
        HashSet<short> flexibleVersions,
        HashSet<short> versions)
    {
        var builder = new StringBuilder();

        if (versions.SetEquals(validVersion))
        {
        }
        else
        {
            //field = GenerateSimpleDecodeField(fieldDescriptor.Name, typeName);
        }

        var readType = GetReadType(type);

        builder.AppendFormat("    {0} = reader.Read{1}();", name, readType);

        return builder;
    }

    private static StringBuilder GenerateDecodeSimpleField(
        string name,
        string type,
        HashSet<short> hashSet,
        HashSet<short> validVersion,
        HashSet<short> flexibleVersions)
    {
        var builder = new StringBuilder();

        var readType = GetReadType(type);

        builder.AppendFormat("    {0} = reader.Read{1}();", name, readType);

        return builder;
    }

    private static string GetReadType(string type)
    {
        return type switch
        {
            "int32" => "Int",
            "int64" => "Long",
            "int16" => "Short",
            "string" => "String",
            "bool" => "Boolean"
        };
    }

    private static StringBuilder GenerateFields(List<FieldDescriptor> fields/*, out IReadOnlyList<FieldMetadata> metadata*/)
    {
        var builder = new StringBuilder();

        var lastField = fields.Last();

        foreach (var fieldDescriptor in fields)
        {
            var field = GenerateField(fieldDescriptor);
            builder.Append(field);
            builder.AppendLine();

            if (fieldDescriptor != lastField)
            {
                builder.AppendLine();
            }
        }

        return builder;
    }

    private static StringBuilder GenerateField(FieldDescriptor field)
    {
        var builder = new StringBuilder();

        builder.AppendLine("  /// <summary>");
        builder.AppendLine($"  /// {field.About}");
        builder.AppendLine("  /// </summary>");

        if (IsSimpleType(field.Type, out var fieldType))
        {
            var type = GetCLRType(fieldType);

            if (field.Ignorable.HasValue && field.Ignorable.Value
                || field.Default is not null && field.Default.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                type += "?";
            }

            builder.AppendFormat("  public {0} {1} {{ get; init; }}", type, field.Name);

            if (field.Default is not null)
            {
                builder.AppendFormat(" = {0};", field.Default);
            }
        }
        else
        {
            if (fieldType.Equals("byte", StringComparison.OrdinalIgnoreCase))
            {
                builder.AppendFormat("  public Span<{0}> {1} {{ get; init; }} = Array.Empty<{0}>();", fieldType, field.Name);
            }
            else
            {
                builder.AppendFormat("  public IReadOnlyCollection<{0}> {1} {{ get; init; }} = Array.Empty<{0}>();", fieldType, field.Name);
                GenerateComplexType(fieldType, field.Fields);
            }
        }

        return builder;
    }

    private static StringBuilder GenerateComplexType(string fieldType, List<FieldDescriptor> fieldFields)
    {
        var builder = new StringBuilder();

        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine();
        builder.AppendLine("namespace Microlibs.Kafka.Protocol.Responses.Types;");
        builder.AppendLine();
        builder.AppendFormat("public record {0}", fieldType);
        builder.AppendLine();
        builder.AppendLine("{");

        return builder;
    }

    private static string GetCLRType(string type)
    {
        return type switch
        {
            "int32" => "int",
            "int64" => "long",
            "int16" => "short",
            "string" => "string",
            "bool" => "bool"
        };
    }

    private static bool IsSimpleType(string type, out string typeName)
    {
        var isBytes = type.Equals("bytes", StringComparison.OrdinalIgnoreCase);
        var isComplex = type.StartsWith("[]") || isBytes;

        if (isComplex)
        {
            typeName = isBytes ? "byte" : type.Substring(2);
        }
        else
        {
            typeName = type;
        }

        return !isComplex;
    }

    private static StringBuilder GenerateHeader(ApiDescriptor apiDescriptor)
    {
        var builder = new StringBuilder();

        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine();
        builder.AppendLine("namespace Microlibs.Kafka.Protocol.Responses;");
        builder.AppendLine();
        builder.AppendFormat("public record {0} : KafkaResponseMessage", apiDescriptor.Name + "Message");
        builder.AppendLine();
        builder.AppendLine("{");

        return builder;
    }

    private static void GenerateRequest(string fileName)
    {
    }

    private IEnumerable<string> GetFileResources()
    {
        var pathToResources = Path.Combine(Directory.GetCurrentDirectory(), _solutionDirectory, "resources", "message");

        return Directory.GetFiles(pathToResources, "*.json");
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
