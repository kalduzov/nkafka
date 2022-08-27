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
        _readMethodGenerator = new ReadMethodGenerator(_codeBuffer);
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
        GenerateProperties(@struct, topLevelMessage?.Type);
        _codeBuffer.AppendLine();
        GenerateCtor(className, topLevelMessage, parentVersions);
        _codeBuffer.AppendLine();
        _readMethodGenerator.Generate(className, @struct, parentVersions);
        _codeBuffer.AppendLine();
        _writeMethodGenerator.Generate(className, @struct, parentVersions, _messageFlexibleVersions);

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

        if (topLevelMessage is not null && topLevelMessage.Type == MessageType.Request)
        {
            _codeBuffer.IncrementIndent();
            var apiKey = (ApiKeys)topLevelMessage.ApiKey;
            _codeBuffer.AppendLine($"ApiKey = ApiKeys.{apiKey};");
            _codeBuffer.DecrementIndent();
        }

        _codeBuffer.IncrementIndent();
        _codeBuffer.AppendLine($"LowestSupportedVersion = ApiVersions.Version{versions.Lowest};");
        _codeBuffer.AppendLine($"HighestSupportedVersion = ApiVersions.Version{versions.Highest};");
        _codeBuffer.DecrementIndent();

        _codeBuffer.AppendLine("}");
        _codeBuffer.AppendLine();
        _codeBuffer.AppendLine($"public {className}(BufferReader reader, ApiVersions version)");

        _codeBuffer.IncrementIndent();
        _codeBuffer.AppendLine(": base(reader, version)");
        _codeBuffer.DecrementIndent();

        _codeBuffer.AppendLine("{");

        _codeBuffer.IncrementIndent();
        _codeBuffer.AppendLine("Read(reader, version);");

        if (topLevelMessage is not null && topLevelMessage.Type == MessageType.Request)
        {
            var apiKey = (ApiKeys)topLevelMessage.ApiKey;
            _codeBuffer.AppendLine($"ApiKey = ApiKeys.{apiKey};");
        }

        _codeBuffer.AppendLine($"LowestSupportedVersion = ApiVersions.Version{versions.Lowest};");
        _codeBuffer.AppendLine($"HighestSupportedVersion = ApiVersions.Version{versions.Highest};");
        _codeBuffer.DecrementIndent();

        _codeBuffer.AppendLine("}");
    }

    private void GenerateProperties(
        StructSpecification structSpecification,
        MessageType? messageType)
    {
        var lastField = structSpecification.Fields.Last();

        foreach (var fieldDescriptor in structSpecification.Fields)
        {
            if (messageType is MessageType.Response && fieldDescriptor.Name == "ThrottleTimeMs")
            {
                continue; //Данное поле есть в базовом классе
            }

            GenerateProperty(fieldDescriptor);

            if (fieldDescriptor != lastField)
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
    }

    private void GenerateClassHeader(string className, bool isTopLevel, MessageType? messageType)
    {
        var implementedInterfaces = new HashSet<string>();

        if (isTopLevel && messageType.HasValue)
        {
            implementedInterfaces.Add(messageType + "Message");
        }
        else
        {
            implementedInterfaces.Add("Message");
        }

        _codeBuffer.AppendLine($"public sealed class {className}: {string.Join(", ", implementedInterfaces)}");
        _codeBuffer.AppendLine("{");
    }
}