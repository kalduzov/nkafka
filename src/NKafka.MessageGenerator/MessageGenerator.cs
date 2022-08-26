using System.Text;

using NKafka.MessageGenerator.Specifications;

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

    public MessageGenerator(string ns)
    {
        _headerGenerator = new HeaderGenerator(ns);
        _codeBuffer = new CodeBuffer();
        _structRegistry = new StructRegistry();
        _readMethodGenerator = new ReadMethodGenerator(_codeBuffer);
        _writeMethodGenerator = new WriteMethodGenerator(_codeBuffer);
    }

    public StringBuilder Generate(MessageSpecification message)
    {
        if (message.Struct.Versions.Contains(short.MaxValue))
        {
            throw new ArgumentException($"Message {message.Struct.Name} does not specify a maximum version.");
        }

        _structRegistry.Register(message);

        GenerateClass(message, message.ClassName, message.Struct, message.Struct.Versions);

        _headerGenerator.AppendUsing("System.Text");
        _headerGenerator.AppendUsing("NKafka.Protocol");
        _headerGenerator.AppendUsing("NKafka.Protocol.Records");
        _headerGenerator.AppendUsing("NKafka.Protocol.Extensions");
        _headerGenerator.Generate();

        var result = new StringBuilder();
        result.Append(_headerGenerator);
        result.Append(_codeBuffer);

        return result;
    }

    private void GenerateClass(MessageSpecification? message, string className, StructSpecification messageStruct, Versions parentVersions)
    {
        _codeBuffer.AppendLine();
        var isTopLevel = message is not null;
        var isDictionaryElement = messageStruct.HasKeys;

        if (isTopLevel && isDictionaryElement)
        {
            throw new ArgumentException("Cannot set mapKey on top level fields.");
        }

        GenerateClassHeader(className, message?.Type);
        _codeBuffer.IncrementIndent();
        GenerateProperties(messageStruct, message?.Type, isDictionaryElement);
        _codeBuffer.AppendLine();
        GenerateCtor(className, messageStruct, isTopLevel, isDictionaryElement);
        _codeBuffer.AppendLine();
        _readMethodGenerator.Generate(className, messageStruct, parentVersions);
        _codeBuffer.AppendLine();
        _writeMethodGenerator.Generate(className, messageStruct, parentVersions);
        _codeBuffer.AppendLine();

        if (!isTopLevel)
        {
            _codeBuffer.DecrementIndent();
            _codeBuffer.AppendLine("}");
        }

        GenerateSubclasses(className, messageStruct, parentVersions, isDictionaryElement);

        if (!isTopLevel)
        {
            return;
        }

        foreach (var commonStruct in _structRegistry.CommonStructs)
        {
            GenerateClass(null, commonStruct.Name, commonStruct, commonStruct.Versions);
        }

        _codeBuffer.DecrementIndent();
        _codeBuffer.AppendLine("}");
    }

    private void GenerateSubclasses(string className, StructSpecification messageStruct, Versions parentVersions, bool isDictionaryElement)
    {
        foreach (var field in messageStruct.Fields)
        {
            if (field.Type.IsStructArray && field.Type is IFieldType.ArrayType arrayType)
            {
                if (!_structRegistry.CommonStructNames.Contains(arrayType.ElementName))
                {
                    GenerateClass(
                        null,
                        arrayType.ElementType.ToString(),
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
                            field.Type.ToString(),
                            _structRegistry.FindStruct(field),
                            parentVersions.Intersect(messageStruct.Versions));
                    }
                }
            }
        }

        if (isDictionaryElement)
        {
            GenerateDictionary(className, messageStruct);
        }
    }

    private void GenerateDictionary(string className, StructSpecification messageStruct)
    {
    }

    private void GenerateCtor(string className, StructSpecification messageStruct, bool isTopLevel, bool isDictionaryElement)
    {
    }

    private void GenerateProperties(StructSpecification structSpecification, MessageType? messageType, bool isDictionaryElement)
    {
        var lastField = structSpecification.Fields.Last();

        foreach (var fieldDescriptor in structSpecification.Fields)
        {
            if (messageType is MessageType.Response && fieldDescriptor.Name == "ThrottleTimeMs")
            {
                continue; //Данное поле есть в базовом классе
            }

            GenerateProperty(fieldDescriptor, isDictionaryElement);

            if (fieldDescriptor != lastField)
            {
                _codeBuffer.AppendLine();
            }
        }
    }

    private void GenerateProperty(FieldSpecification field, bool isDictionaryElement)
    {
        _codeBuffer.AppendLine($"/// <summary>");
        _codeBuffer.AppendLine($"/// {field.About}");
        _codeBuffer.AppendLine($"/// </summary>");

        var type = GetClrType(field, isDictionaryElement);
        var nullableMarker = NullableMarkerIfNeeded(field);
        _codeBuffer.Append($"public {type}{nullableMarker} {field.Name} {{ get; set; }} = {field.FieldDefault()};");
    }

    private static string GetClrType(FieldSpecification field, bool isDictionaryElement)
    {
        if (!field.Type.IsArray)
        {
            return field.Type.ClrName;
        }

        if (isDictionaryElement)
        {
            return $"Dictionary<{field.Type.ClrName}>"; //todo тут должен быть словарь
        }

        return $"List<{field.Type.ClrName}>";
    }

    private static string NullableMarkerIfNeeded(FieldSpecification field)
    {
        return field.Ignorable && field.Default.Equals("null", StringComparison.OrdinalIgnoreCase)
            ? "?"
            : string.Empty;
    }

    private void GenerateClassHeader(string className, MessageType? messageType)
    {
        var baseClassName = messageType is null ? "Message" : messageType + "Message";
        _codeBuffer.AppendLine($"public sealed class {className}: {baseClassName}");
        _codeBuffer.AppendLine("{");
    }

    // public StringBuilder Generate()
    // {
    //     var builder = new StringBuilder();
    //
    //     _headerGenerator.AppendUsing("System.Text");
    //     _headerGenerator.AppendUsing("NKafka.Protocol");
    //     _headerGenerator.AppendUsing("NKafka.Protocol.Records");
    //     _headerGenerator.AppendUsing("NKafka.Protocol.Extensions");
    //
    //     var header = _headerGenerator.Generate();
    //     builder.Append(header);
    //     var @class = _classGenerator.Generate();
    //     builder.Append(@class);
    //
    //     return builder;
    // }
}