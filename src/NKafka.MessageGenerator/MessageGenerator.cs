using System.Text;

using NKafka.MessageGenerator.Specifications;
using NKafka.Protocol;

namespace NKafka.MessageGenerator;

/// <summary>
/// Генератор классов для requests и responses из папки resources
/// </summary>
public class MessageGenerator: IMessageGenerator
{
    private readonly IHeaderGenerator _headerGenerator;
    private readonly ICodeGenerator _codeGenerator;
    private readonly StructRegistry _structRegistry;
    private readonly IMethodGenerator _readMethodGenerator;
    private readonly IMethodGenerator _writeMethodGenerator;
    private Versions _messageFlexibleVersions = Versions.None;

    public MessageGenerator(string ns)
    {
        _headerGenerator = new HeaderGenerator(ns);
        _codeGenerator = new CodeGenerator();
        _structRegistry = new StructRegistry();
        _readMethodGenerator = new ReadMethodGenerator(_structRegistry, _codeGenerator);
        _writeMethodGenerator = new WriteMethodGenerator(_structRegistry, _codeGenerator);
    }

    public StringBuilder Generate(MessageSpecification message)
    {
        if (message.Struct.Versions.Contains(short.MaxValue))
        {
            throw new ArgumentException($"Message {message.Struct.Name} does not specify a maximum version.");
        }

        _structRegistry.Register(message);
        _messageFlexibleVersions = message.FlexibleVersions;

        GenerateClass(message, message.ClassName, message.Struct, message.Struct.Versions);

        _headerGenerator.AppendUsing("System.Text");
        _headerGenerator.AppendUsing("NKafka.Protocol");
        _headerGenerator.AppendUsing("NKafka.Protocol.Records");
        _headerGenerator.AppendUsing("NKafka.Protocol.Extensions");
        _headerGenerator.AppendUsing("NKafka.Exceptions");
        _headerGenerator.Generate();

        var result = new StringBuilder();
        result.Append(_headerGenerator);
        result.Append(_codeGenerator);

        return result;
    }

    private void GenerateClass(MessageSpecification? topLevelMessage, string className, StructSpecification @struct, Versions parentVersions)
    {
        _codeGenerator.AppendLine();
        var isTopLevel = topLevelMessage is not null;
        var isSetElement = @struct.HasKeys;

        if (isTopLevel && isSetElement)
        {
            throw new ArgumentException("Cannot set mapKey on top level fields.");
        }

        GenerateClassHeader(className, isTopLevel, topLevelMessage?.Type);
        _codeGenerator.IncrementIndent();
        GenerateProperties(@struct, isTopLevel, topLevelMessage, parentVersions);
        _codeGenerator.AppendLine();
        GenerateCtor(className);
        _codeGenerator.AppendLine();
        _readMethodGenerator.Generate(className, @struct, parentVersions, _messageFlexibleVersions);
        _codeGenerator.AppendLine();
        _writeMethodGenerator.Generate(className, @struct, parentVersions, _messageFlexibleVersions);

        if (isSetElement)
        {
            _codeGenerator.AppendLine();
            GenerateEquals(className, @struct, true);
        }

        _codeGenerator.AppendLine();
        GenerateEquals(className, @struct, false);

        _codeGenerator.AppendLine();
        GenerateHashCode(@struct, isSetElement);

        _codeGenerator.AppendLine();
        GenerateToString(className, @struct);

        if (!isTopLevel)
        {
            _codeGenerator.DecrementIndent();
            _codeGenerator.AppendRightBrace();
        }

        GenerateSubclasses(className, @struct, parentVersions, isSetElement);

        if (!isTopLevel)
        {
            return;
        }

        foreach (var commonStruct in _structRegistry.CommonStructs)
        {
            GenerateClass(null, commonStruct.Name + "Message", commonStruct, parentVersions);
        }

        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();
    }

    private void GenerateToString(string className, StructSpecification @struct)
    {
        _codeGenerator.AppendLine("public override string ToString()");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine($"return \"{className}(\"");
        _codeGenerator.IncrementIndent();
        var prefix = "";

        foreach (var field in @struct.Fields)
        {
            GenerateFieldToString(prefix, field);
            prefix = ", ";
        }

        _codeGenerator.AppendLine("+ \")\";");
        _codeGenerator.DecrementIndent();
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();
    }

    private void GenerateFieldToString(string prefix, FieldSpecification field)
    {
        switch (field.Type)
        {
            case IFieldType.BoolFieldType:
            {
                _codeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + ({field.Name} ? \"true\" : \"false\")");

                break;
            }
            case IFieldType.Int8FieldType:
            case IFieldType.Int16FieldType:
            case IFieldType.UInt16FieldType:
            case IFieldType.Int32FieldType:
            case IFieldType.UInt32FieldType:
            case IFieldType.Int64FieldType:
            case IFieldType.Float64FieldType:
            case IFieldType.UuidFieldType:
            {
                _codeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + {field.Name}");

                break;
            }
        }
        //todo дописать работу со сложными типами
    }

    private void GenerateHashCode(StructSpecification @struct, bool onlyMapKeys)
    {
        _codeGenerator.AppendLine("public override int GetHashCode()");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine("var hashCode = 0;");

        var names = new List<string>(@struct.Fields.Count);

        foreach (var field in @struct.Fields)
        {
            if (!onlyMapKeys || field.MapKey)
            {
                names.Add(field.Name);
            }
        }

        foreach (var nameChunks in names.Chunk(7)) //8 is max number of arguments in Combine method
        {
            var values = string.Join(", ", nameChunks);
            _codeGenerator.AppendLine($"hashCode = HashCode.Combine(hashCode, {values});");
        }

        _codeGenerator.AppendLine("return hashCode;");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();
    }

    private void GenerateEquals(string className, StructSpecification @struct, bool elementKeysAreEqual)
    {
        if (!elementKeysAreEqual)
        {
            _codeGenerator.AppendLine("public override bool Equals(object? obj)");
            _codeGenerator.AppendLeftBrace();
            _codeGenerator.IncrementIndent();
            _codeGenerator.AppendLine($"return ReferenceEquals(this, obj) || obj is {className} other && Equals(other);");
            _codeGenerator.DecrementIndent();
            _codeGenerator.AppendRightBrace();

            _codeGenerator.AppendLine();

            _codeGenerator.AppendLine($"public bool Equals({className}? other)");
            _codeGenerator.AppendLeftBrace();
            _codeGenerator.IncrementIndent();
            _codeGenerator.AppendLine("return true;");
            _codeGenerator.DecrementIndent();
            _codeGenerator.AppendRightBrace();
        }
    }

    private void GenerateSubclasses(string className, StructSpecification messageStruct, Versions parentVersions, bool isSetElement)
    {
        foreach (var field in messageStruct.Fields)
        {
            if (field.Type.IsStructArray && field.Type is IFieldType.ArrayType arrayType)
            {
                if (!_structRegistry.CommonStructNames.Contains(arrayType.ElementName))
                {
                    GenerateClass(
                        null,
                        $"{arrayType.ElementType.ToString()}Message",
                        _structRegistry.FindStruct(field),
                        parentVersions.Intersect(messageStruct.Versions));
                }
            }
            else
            {
                if (field.Type.IsStruct)
                {
                    if (!_structRegistry.CommonStructNames.Contains(field.Type.ToString()))
                    {
                        GenerateClass(
                            null,
                            $"{field.Type.ToString()}Message",
                            _structRegistry.FindStruct(field),
                            parentVersions.Intersect(messageStruct.Versions));
                    }
                }
            }
        }

        if (isSetElement)
        {
            GenerateCollection(className, messageStruct);
        }
    }

    private void GenerateCollection(string className, StructSpecification messageStruct)
    {
        var collectionName = FieldSpecification.CollectionType(messageStruct.Name);
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine($"public sealed class {collectionName}: HashSet<{className}>");
        _codeGenerator.AppendLeftBrace();

        _codeGenerator.IncrementIndent();

        _codeGenerator.AppendLine($"public {collectionName}()");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.AppendRightBrace();
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine($"public {collectionName}(int capacity)");
        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine(": base(capacity)");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.AppendRightBrace();

        _codeGenerator.DecrementIndent();

        _codeGenerator.AppendRightBrace();
    }

    private void GenerateCtor(string className)
    {
        _codeGenerator.AppendLine($"public {className}()");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.AppendRightBrace();
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine($"public {className}(BufferReader reader, ApiVersion version)");

        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine(": this()");
        _codeGenerator.DecrementIndent();

        _codeGenerator.AppendLeftBrace();

        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine("Read(reader, version);");
        _codeGenerator.DecrementIndent();

        _codeGenerator.AppendRightBrace();
    }

    private void GenerateProperties(
        StructSpecification structSpecification,
        bool isTopLevel,
        MessageSpecification? topLevelMessage,
        Versions versions)
    {
        var lastField = structSpecification.Fields.Last();

        
        _codeGenerator.AppendLine($"public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version{versions.Lowest};");
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine($"public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version{versions.Highest};");
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine($"public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;");
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine($"public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;");
        _codeGenerator.AppendLine();

        if (isTopLevel)
        {
            switch (topLevelMessage!.Type)
            {
                //Данные поля есть в базовом классе
                case MessageType.Request:
                {
                    var apiKey = (ApiKeys)topLevelMessage.ApiKey;
                    _codeGenerator.AppendLine($"public ApiKeys ApiKey => ApiKeys.{apiKey};");
                    _codeGenerator.AppendLine();

                    break;
                }
            }
        }

        _codeGenerator.AppendLine("public List<TaggedField>? UnknownTaggedFields { get; set; } = null;");
        _codeGenerator.AppendLine();

        var containsThrottleTimeField = structSpecification.Fields.Any(x => x.Name == "ThrottleTimeMs");

        if (isTopLevel && topLevelMessage!.Type == MessageType.Response && !containsThrottleTimeField)
        {
            _codeGenerator.AppendLine("public int ThrottleTimeMs { get; set; } = 0;");
            _codeGenerator.AppendLine();
        }

        foreach (var field in structSpecification.Fields)
        {
            GenerateProperty(field);

            if (field != lastField)
            {
                _codeGenerator.AppendLine();
            }
        }
    }

    private void GenerateProperty(FieldSpecification field)
    {
        _codeGenerator.AppendLine($"/// <summary>");
        _codeGenerator.AppendLine($"/// {field.About}");
        _codeGenerator.AppendLine($"/// </summary>");

        var type = field.FieldAbstractClrType(_structRegistry);
        var defaultValue = field.FieldDefault();
        var nullableMarker = defaultValue.Equals("null") ? "?" : string.Empty;
        _codeGenerator.AppendLine($"public {type}{nullableMarker} {field.Name} {{ get; set; }} = {defaultValue};");

        if (field.Name != "ErrorCode")
        {
            return;
        }

        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine($"/// <inheritdoc />");
        _codeGenerator.AppendLine("public ErrorCodes Code => (ErrorCodes)ErrorCode;");
    }

    private void GenerateClassHeader(string className, bool isTopLevel, MessageType? messageType)
    {
        var implementedInterfaces = new HashSet<string>();

        if (isTopLevel)
        {
            var baseType = messageType switch
            {
                MessageType.Request => "Request",
                MessageType.Response => "Response",
                MessageType.Header => "",
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
            };
            implementedInterfaces.Add($"I{baseType}Message");
        }
        else
        {
            implementedInterfaces.Add("IMessage");
        }

        implementedInterfaces.Add($"IEquatable<{className}>");

        _codeGenerator.AppendLine($"public sealed class {className}: {string.Join(", ", implementedInterfaces)}");
        _codeGenerator.AppendLeftBrace();
    }
}