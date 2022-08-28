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
    private readonly CodeBuffer _codeBuffer;
    private readonly StructRegistry _structRegistry;
    private readonly IReadMethodGenerator _readMethodGenerator;
    private readonly IWriteMethodGenerator _writeMethodGenerator;
    private Versions _messageFlexibleVersions;

    public MessageGenerator(string ns)
    {
        _headerGenerator = new HeaderGenerator(ns);
        _codeBuffer = new CodeBuffer();
        _structRegistry = new StructRegistry();
        _readMethodGenerator = new ReadMethodGenerator(_headerGenerator, _structRegistry, _codeBuffer);
        _writeMethodGenerator = new WriteMethodGenerator(_headerGenerator, _structRegistry, _codeBuffer);
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
        result.Append(_codeBuffer);

        return result;
    }

    private void GenerateClass(MessageSpecification? topLevelMessage, string className, StructSpecification @struct, Versions parentVersions)
    {
        _codeBuffer.AppendLine();
        var isTopLevel = topLevelMessage is not null;
        var isSetElement = @struct.HasKeys;

        if (isTopLevel && isSetElement)
        {
            throw new ArgumentException("Cannot set mapKey on top level fields.");
        }

        GenerateClassHeader(className, isTopLevel, topLevelMessage?.Type);
        _codeBuffer.IncrementIndent();
        GenerateProperties(@struct, isTopLevel, topLevelMessage, parentVersions);
        _codeBuffer.AppendLine();
        GenerateCtor(className, topLevelMessage, parentVersions);
        _codeBuffer.AppendLine();
        _readMethodGenerator.Generate(className, @struct, parentVersions, _messageFlexibleVersions);
        _codeBuffer.AppendLine();
        _writeMethodGenerator.Generate(className, @struct, parentVersions, _messageFlexibleVersions);

        if (isSetElement)
        {
            _codeBuffer.AppendLine();
            GenerateEquals(className, @struct, true);
        }

        _codeBuffer.AppendLine();
        GenerateEquals(className, @struct, false);

        if (!isTopLevel)
        {
            _codeBuffer.DecrementIndent();
            _codeBuffer.AppendLine("}");
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

        _codeBuffer.DecrementIndent();
        _codeBuffer.AppendLine("}");
    }

    private void GenerateEquals(string className, StructSpecification @struct, bool elementKeysAreEqual)
    {
        if (!elementKeysAreEqual)
        {
            _codeBuffer.AppendLine("public override bool Equals(object? obj)");
            _codeBuffer.AppendLine("{");
            _codeBuffer.IncrementIndent();
            _codeBuffer.AppendLine($"return ReferenceEquals(this, obj) || obj is {className} other && Equals(other);");
            _codeBuffer.DecrementIndent();
            _codeBuffer.AppendLine("}");

            _codeBuffer.AppendLine();

            _codeBuffer.AppendLine($"public bool Equals({className}? other)");
            _codeBuffer.AppendLine("{");
            _codeBuffer.IncrementIndent();
            _codeBuffer.AppendLine("return true;");
            _codeBuffer.DecrementIndent();
            _codeBuffer.AppendLine("}");
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
        _codeBuffer.AppendLine();
        _codeBuffer.AppendLine($"public sealed class {collectionName}: HashSet<{className}>");
        _codeBuffer.AppendLine("{");

        _codeBuffer.IncrementIndent();

        _codeBuffer.AppendLine($"public {collectionName}()");
        _codeBuffer.AppendLine("{");
        _codeBuffer.AppendLine("}");
        _codeBuffer.AppendLine();
        _codeBuffer.AppendLine($"public {collectionName}(int capacity)");
        _codeBuffer.IncrementIndent();
        _codeBuffer.AppendLine(": base(capacity)");
        _codeBuffer.DecrementIndent();
        _codeBuffer.AppendLine("{");
        _codeBuffer.AppendLine("}");

        _codeBuffer.DecrementIndent();

        _codeBuffer.AppendLine("}");
    }

    private void GenerateCtor(
        string className,
        MessageSpecification? topLevelMessage,
        Versions versions)
    {
        _codeBuffer.AppendLine($"public {className}()");
        _codeBuffer.AppendLine("{");
        _codeBuffer.AppendLine("}");
        _codeBuffer.AppendLine();
        _codeBuffer.AppendLine($"public {className}(BufferReader reader, ApiVersions version)");

        _codeBuffer.IncrementIndent();
        _codeBuffer.AppendLine(": this()");
        _codeBuffer.DecrementIndent();

        _codeBuffer.AppendLine("{");

        _codeBuffer.IncrementIndent();
        _codeBuffer.AppendLine("Read(reader, version);");
        _codeBuffer.DecrementIndent();

        _codeBuffer.AppendLine("}");
    }

    private void GenerateProperties(
        StructSpecification structSpecification,
        bool isTopLevel,
        MessageSpecification topLevelMessage,
        Versions versions)
    {
        var lastField = structSpecification.Fields.Last();

        _codeBuffer.AppendLine($"public ApiVersions LowestSupportedVersion => ApiVersions.Version{versions.Lowest};");
        _codeBuffer.AppendLine();
        _codeBuffer.AppendLine($"public ApiVersions HighestSupportedVersion => ApiVersions.Version{versions.Highest};");
        _codeBuffer.AppendLine();

        if (isTopLevel)
        {
            switch (topLevelMessage.Type)
            {
                //Данные поля есть в базовом классе
                case MessageType.Request:
                {
                    var apiKey = (ApiKeys)topLevelMessage.ApiKey;
                    _codeBuffer.AppendLine($"public ApiKeys ApiKey => ApiKeys.{apiKey};");
                    _codeBuffer.AppendLine();

                    break;
                }
            }
        }

        _codeBuffer.AppendLine("public ApiVersions Version {get; set;}");
        _codeBuffer.AppendLine();
        _codeBuffer.AppendLine("public List<TaggedField>? UnknownTaggedFields { get; set; } = null;");
        _codeBuffer.AppendLine();

        var containsThrottleTimeField = structSpecification.Fields.Any(x => x.Name == "ThrottleTimeMs");
        
        if (isTopLevel && topLevelMessage.Type == MessageType.Response && !containsThrottleTimeField)
        {
            _codeBuffer.AppendLine("public int ThrottleTimeMs { get; set; } = 0;");
            _codeBuffer.AppendLine();
        }

        foreach (var field in structSpecification.Fields)
        {
            GenerateProperty(field);

            if (field != lastField)
            {
                _codeBuffer.AppendLine();
            }
        }
    }

    private void GenerateProperty(FieldSpecification field)
    {
        _codeBuffer.AppendLine($"/// <summary>");
        _codeBuffer.AppendLine($"/// {field.About}");
        _codeBuffer.AppendLine($"/// </summary>");

        var type = field.FieldAbstractClrType(_structRegistry);
        var defaultValue = field.FieldDefault();
        var nullableMarker = defaultValue.Equals("null") ? "?" : string.Empty;
        _codeBuffer.AppendLine($"public {type}{nullableMarker} {field.Name} {{ get; set; }} = {defaultValue};");

        if (field.Name != "ErrorCode")
        {
            return;
        }

        _codeBuffer.AppendLine();
        _codeBuffer.AppendLine($"/// <inheritdoc />");
        _codeBuffer.AppendLine("public ErrorCodes Code => (ErrorCodes)ErrorCode;");
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

        _codeBuffer.AppendLine($"public sealed class {className}: {string.Join(", ", implementedInterfaces)}");
        _codeBuffer.AppendLine("{");
    }
}