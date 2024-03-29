﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

using System.Web;

using NKafka.MessageGenerator.Specifications;
using NKafka.Protocol;

namespace NKafka.MessageGenerator;

/// <summary>
/// Генератор классов для requests и responses из папки resources
/// </summary>
public class MessageGenerator: ClassGenerator, IMessageGenerator
{
    private readonly IMethodGenerator _readMethodGenerator;
    private readonly IMethodGenerator _writeMethodGenerator;
    private Versions _messageFlexibleVersions = Versions.None;

    public MessageGenerator(string ns)
        : base(ns)
    {
        _readMethodGenerator = new ReadMethodGenerator(StructRegistry, CodeGenerator);
        _writeMethodGenerator = new WriteMethodGenerator(StructRegistry, CodeGenerator);
    }

    protected override void InternalGenerate(MessageSpecification message)
    {
        if (message.Struct.Versions.Contains(short.MaxValue))
        {
            throw new ArgumentException($"Message {message.Struct.Name} does not specify a maximum version.");
        }

        StructRegistry.Register(message);
        _messageFlexibleVersions = message.FlexibleVersions;

        GenerateClass(message, message.ClassName, message.Struct, message.Struct.Versions);

        HeaderGenerator.AppendUsing("System.Text");
        HeaderGenerator.AppendUsing("NKafka.Protocol");
        HeaderGenerator.AppendUsing("NKafka.Protocol.Records");
        HeaderGenerator.AppendUsing("NKafka.Protocol.Buffers");
        HeaderGenerator.AppendUsing("NKafka.Protocol.Extensions");
        HeaderGenerator.AppendUsing("NKafka.Exceptions");
    }

    private void GenerateClass(MessageSpecification messageSpecification,
        string className,
        StructSpecification structSpecification,
        Versions parentVersions)
    {
        CodeGenerator.AppendLine();
        var isSetElement = structSpecification.HasKeys;

        if (isSetElement)
        {
            throw new ArgumentException("Cannot set mapKey on top level fields.");
        }

        GenerateClassHeader(className, true, messageSpecification.Type);
        CodeGenerator.IncrementIndent();
        GenerateProperties(structSpecification, true, messageSpecification);
        CodeGenerator.AppendLine();
        GenerateCtor(className);
        CodeGenerator.AppendLine();
        _readMethodGenerator.Generate(className, structSpecification, parentVersions, _messageFlexibleVersions);
        CodeGenerator.AppendLine();
        _writeMethodGenerator.Generate(className, structSpecification, parentVersions, _messageFlexibleVersions);

        CodeGenerator.AppendLine();
        GenerateEquals(className, structSpecification);

        CodeGenerator.AppendLine();
        GenerateHashCode(structSpecification, isSetElement);

        CodeGenerator.AppendLine();
        GenerateToString(className, structSpecification);

        GenerateSubclasses(className, structSpecification, parentVersions, isSetElement);

        foreach (var commonStruct in StructRegistry.CommonStructs)
        {
            GenerateSubClass(
                commonStruct.Name + "Message",
                commonStruct,
                parentVersions);
        }

        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();
    }

    private void GenerateSubClass(string className, StructSpecification structSpecification, Versions parentVersions)
    {
        CodeGenerator.AppendLine();
        var isSetElement = structSpecification.HasKeys;
        GenerateClassHeader(className, false, MessageType.None);
        CodeGenerator.IncrementIndent();
        GenerateProperties(structSpecification, false, MessageSpecification.Empty);
        CodeGenerator.AppendLine();
        GenerateCtor(className);
        CodeGenerator.AppendLine();
        _readMethodGenerator.Generate(className, structSpecification, parentVersions, _messageFlexibleVersions);
        CodeGenerator.AppendLine();
        _writeMethodGenerator.Generate(className, structSpecification, parentVersions, _messageFlexibleVersions);

        CodeGenerator.AppendLine();
        GenerateEquals(className, structSpecification);

        CodeGenerator.AppendLine();
        GenerateHashCode(structSpecification, isSetElement);

        CodeGenerator.AppendLine();
        GenerateToString(className, structSpecification);

        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();

        GenerateSubclasses(className, structSpecification, parentVersions, isSetElement);
    }

    private void GenerateToString(string className, StructSpecification @struct)
    {
        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine("public override string ToString()");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine($"return \"{className}(\"");
        CodeGenerator.IncrementIndent();
        var prefix = "";

        foreach (var field in @struct.Fields)
        {
            GenerateFieldToString(prefix, field);
            prefix = ", ";
        }

        CodeGenerator.AppendLine("+ \")\";");
        CodeGenerator.DecrementIndent();
        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();
    }

    private void GenerateFieldToString(string prefix, FieldSpecification field)
    {
        switch (field.Type)
        {
            case IFieldType.BoolFieldType:
                {
                    CodeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + ({field.Name} ? \"true\" : \"false\")");

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
            case IFieldType.RecordsFieldType:
                {
                    CodeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + {field.Name}");

                    break;
                }

            case IFieldType.StringFieldType:
                {
                    CodeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + (string.IsNullOrWhiteSpace({field.Name}) ? \"null\" : {field.Name})");

                    break;
                }
            case IFieldType.StructType:
                {
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (field.NullableVersions.IsEmpty)
                    {
                        CodeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + {field.Name}.ToString()");
                    }
                    else
                    {
                        CodeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + ({field.Name} is null ? \"null\" : {field.Name}.ToString())");
                    }

                    break;
                }
            case IFieldType.BytesFieldType:
            case IFieldType.ArrayType:
                {
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (field.NullableVersions.IsEmpty)
                    {
                        CodeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + {field.Name}.DeepToString()");
                    }
                    else
                    {
                        CodeGenerator.AppendLine($"+ \"{prefix}{field.Name}=\" + ({field.Name} is null ? \"null\" : {field.Name}.DeepToString())");
                    }

                    break;
                }
        }

    }

    private void GenerateHashCode(StructSpecification @struct, bool onlyMapKeys)
    {
        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine("public override int GetHashCode()");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine("var hashCode = 0;");

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
            CodeGenerator.AppendLine($"hashCode = HashCode.Combine(hashCode, {values});");
        }

        CodeGenerator.AppendLine("return hashCode;");
        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();
    }

    private void GenerateEquals(string className, StructSpecification @struct)
    {
        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine("public override bool Equals(object? obj)");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine($"return ReferenceEquals(this, obj) || obj is {className} other && Equals(other);");
        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();

        CodeGenerator.AppendLine();

        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine($"public bool Equals({className}? other)");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.IncrementIndent();

        CodeGenerator.AppendLine("if (other is null)");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine("return false;");
        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();

        foreach (var field in @struct.Fields)
        {
            GenerateFieldEquals(field);
        }

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        CodeGenerator.AppendLine("return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);");

        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();
    }

    private void GenerateFieldEquals(FieldSpecification field)
    {
        switch (field.Type)
        {
            case IFieldType.UuidFieldType:
                {
                    CodeGenerator.AppendLine($"if (!{field.Name}.Equals(other.{field.Name}))");
                    CodeGenerator.AppendLeftBrace();
                    CodeGenerator.IncrementIndent();

                    CodeGenerator.AppendLine("return false;");

                    CodeGenerator.DecrementIndent();
                    CodeGenerator.AppendRightBrace();

                    break;
                }
            case IFieldType.StringFieldType:
            case IFieldType.ArrayType:
            case IFieldType.StructType:
                {

                    CodeGenerator.AppendLine($"if ({field.Name} is null)");
                    CodeGenerator.AppendLeftBrace();
                    CodeGenerator.IncrementIndent();

                    CodeGenerator.AppendLine($"if (other.{field.Name} is not null)");
                    CodeGenerator.AppendLeftBrace();
                    CodeGenerator.IncrementIndent();
                    CodeGenerator.AppendLine("return false;");
                    CodeGenerator.DecrementIndent();
                    CodeGenerator.AppendRightBrace();
                    CodeGenerator.DecrementIndent();
                    CodeGenerator.AppendRightBrace();

                    CodeGenerator.AppendLine("else");

                    CodeGenerator.AppendLeftBrace();
                    CodeGenerator.IncrementIndent();

                    if (field.Type is IFieldType.ArrayType)
                    {
                        CodeGenerator.AppendLine($"if (!{field.Name}.SequenceEqual(other.{field.Name}))");
                    }
                    else
                    {
                        CodeGenerator.AppendLine($"if (!{field.Name}.Equals(other.{field.Name}))");
                    }
                    CodeGenerator.AppendLeftBrace();
                    CodeGenerator.IncrementIndent();

                    CodeGenerator.AppendLine("return false;");

                    CodeGenerator.DecrementIndent();
                    CodeGenerator.AppendRightBrace();

                    CodeGenerator.DecrementIndent();
                    CodeGenerator.AppendRightBrace();

                    break;
                }
            case IFieldType.BytesFieldType:
                {
                    //todo надо понять нужно ли нам это поле?
                    // if (field.ZeroCopy)
                    // {
                    //     CodeGenerator.AppendLine($"if (!ReferenceEquals({field.Name},other.{field.Name}))");
                    //     CodeGenerator.AppendLeftBrace();
                    //     CodeGenerator.IncrementIndent();
                    //
                    //     CodeGenerator.AppendLine("return false;");
                    //
                    //     CodeGenerator.DecrementIndent();
                    //     CodeGenerator.AppendRightBrace();
                    // }
                    // else
                    {
                        CodeGenerator.AppendLine($"if (!{field.Name}.SequenceEqual(other.{field.Name}))");
                        CodeGenerator.AppendLeftBrace();
                        CodeGenerator.IncrementIndent();

                        CodeGenerator.AppendLine("return false;");

                        CodeGenerator.DecrementIndent();
                        CodeGenerator.AppendRightBrace();
                    }

                    break;
                }
            case IFieldType.RecordsFieldType:
                {
                    CodeGenerator.AppendLine($"if (!ReferenceEquals({field.Name},other.{field.Name}))");
                    CodeGenerator.AppendLeftBrace();
                    CodeGenerator.IncrementIndent();

                    CodeGenerator.AppendLine("return false;");

                    CodeGenerator.DecrementIndent();
                    CodeGenerator.AppendRightBrace();

                    break;
                }
            default:
                {
                    CodeGenerator.AppendLine($"if ({field.Name} != other.{field.Name})");
                    CodeGenerator.AppendLeftBrace();
                    CodeGenerator.IncrementIndent();

                    CodeGenerator.AppendLine("return false;");

                    CodeGenerator.DecrementIndent();
                    CodeGenerator.AppendRightBrace();

                    break;
                }
        }
    }

    private void GenerateSubclasses(string className, StructSpecification @struct, Versions parentVersions, bool isSetElement)
    {
        foreach (var field in @struct.Fields)
        {
            if (field.Type.IsStructArray && field.Type is IFieldType.ArrayType arrayType)
            {
                if (!StructRegistry.CommonStructNames.Contains(arrayType.ElementName))
                {
                    GenerateSubClass(
                        $"{arrayType.ElementType.ToString()}Message",
                        StructRegistry.FindStruct(field),
                        parentVersions.Intersect(@struct.Versions));
                }
            }
            else
            {
                if (field.Type.IsStruct)
                {
                    if (!StructRegistry.CommonStructNames.Contains(field.Type.ToString()))
                    {
                        GenerateSubClass(
                            $"{field.Type.ToString()}Message",
                            StructRegistry.FindStruct(field),
                            parentVersions.Intersect(@struct.Versions));
                    }
                }
            }
        }

        if (isSetElement)
        {
            GenerateCollection(className, @struct);
        }
    }

    private void GenerateCollection(string className, StructSpecification messageStruct)
    {
        var collectionName = FieldSpecification.CollectionType(messageStruct.Name);
        CodeGenerator.AppendLine();
        CodeGenerator.AppendLine("/// <summary>");
        CodeGenerator.AppendLine($"/// Describes the contract for message {collectionName}");
        CodeGenerator.AppendLine("/// </summary>");
        CodeGenerator.AppendLine($"public sealed partial class {collectionName}: HashSet<{className}>");
        CodeGenerator.AppendLeftBrace();

        CodeGenerator.IncrementIndent();

        CodeGenerator.AppendLine("/// <summary>");
        CodeGenerator.AppendLine($"/// Basic collection constructor");
        CodeGenerator.AppendLine("/// </summary>");
        CodeGenerator.AppendLine($"public {collectionName}()");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.AppendRightBrace();

        CodeGenerator.AppendLine();
        CodeGenerator.AppendLine("/// <summary>");
        CodeGenerator.AppendLine($"/// Basic collection constructor with the ability to set capacity");
        CodeGenerator.AppendLine("/// </summary>");
        CodeGenerator.AppendLine($"public {collectionName}(int capacity)");
        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine(": base(capacity)");
        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.AppendRightBrace();

        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine("public override bool Equals(object? obj)");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine($"return SetEquals((IEnumerable<{className}>)obj);");
        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();

        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();
    }

    private void GenerateCtor(string className)
    {
        CodeGenerator.AppendLine("/// <summary>");
        CodeGenerator.AppendLine($"/// The basic constructor of the message {className}");
        CodeGenerator.AppendLine("/// </summary>");

        CodeGenerator.AppendLine($"public {className}()");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.AppendRightBrace();
        CodeGenerator.AppendLine();

        CodeGenerator.AppendLine("/// <summary>");
        CodeGenerator.AppendLine($"/// Base constructor for deserializing message {className}");
        CodeGenerator.AppendLine("/// </summary>");
        CodeGenerator.AppendLine($"public {className}(ref BufferReader reader, ApiVersion version)");

        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine(": this()");
        CodeGenerator.DecrementIndent();

        CodeGenerator.AppendLeftBrace();

        CodeGenerator.IncrementIndent();
        CodeGenerator.AppendLine("IncomingBufferLength = reader.Length;");
        CodeGenerator.AppendLine("Read(ref reader, version);");
        CodeGenerator.DecrementIndent();

        CodeGenerator.AppendRightBrace();
    }

    private void GenerateProperties(
        StructSpecification structSpecification,
        bool isTopLevel,
        MessageSpecification topLevelMessage)
    {
        var lastField = structSpecification.Fields.Last();

        if (isTopLevel)
        {
            switch (topLevelMessage.Type)
            {
                //Данные поля есть в базовом классе
                case MessageType.Request:
                    {
                        var apiKey = (ApiKeys)topLevelMessage.ApiKey;
                        CodeGenerator.AppendLine("/// <inheritdoc />");
                        CodeGenerator.AppendLine($"public ApiKeys ApiKey => ApiKeys.{apiKey};");
                        CodeGenerator.AppendLine();

                        var onlyController = IsOnlyController(topLevelMessage) ? "true" : "false";
                        CodeGenerator.AppendLine("/// <summary>");
                        CodeGenerator.AppendLine("/// Indicates whether the request is accessed by any broker or only by the controller");
                        CodeGenerator.AppendLine("/// </summary>");
                        CodeGenerator.AppendLine($"public const bool ONLY_CONTROLLER = {onlyController};");
                        CodeGenerator.AppendLine();

                        CodeGenerator.AppendLine("/// <inheritdoc />");
                        CodeGenerator.AppendLine("public bool OnlyController => ONLY_CONTROLLER;");
                        CodeGenerator.AppendLine();

                        break;
                    }
            }
        }

        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine("public List<TaggedField>? UnknownTaggedFields { get; set; } = null;");
        CodeGenerator.AppendLine();

        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine("public int IncomingBufferLength { get; private set; } = 0;");
        CodeGenerator.AppendLine();

        var containsThrottleTimeField = structSpecification.Fields.Any(x => x.Name == "ThrottleTimeMs");

        if (isTopLevel && topLevelMessage.Type == MessageType.Response && !containsThrottleTimeField)
        {
            CodeGenerator.AppendLine("/// <inheritdoc />");
            CodeGenerator.AppendLine("public int ThrottleTimeMs { get; set; } = 0;");
            CodeGenerator.AppendLine();
        }

        foreach (var field in structSpecification.Fields)
        {
            GenerateProperty(field);

            if (field != lastField)
            {
                CodeGenerator.AppendLine();
            }
        }
    }

    private static bool IsOnlyController(MessageSpecification topLevelMessage)
    {
        if (topLevelMessage.Listeners is null)
        {
            return false;
        }

        return topLevelMessage.Listeners.Contains(RequestListenerType.Controller)
               && !topLevelMessage.Listeners.Contains(RequestListenerType.Broker);

    }

    private void GenerateProperty(FieldSpecification field)
    {
        CodeGenerator.AppendLine("/// <summary>");
        CodeGenerator.AppendLine($"/// {HttpUtility.HtmlEncode(field.About)}");
        CodeGenerator.AppendLine("/// </summary>");

        var type = field.FieldAbstractClrType(StructRegistry);
        var defaultValue = field.FieldDefault();
        var nullableMarker = defaultValue.Equals("null") ? "?" : string.Empty;
        CodeGenerator.AppendLine($"public {type}{nullableMarker} {field.Name} {{ get; set; }} = {defaultValue};");

        if (field.Name != "ErrorCode")
        {
            return;
        }

        CodeGenerator.AppendLine();
        CodeGenerator.AppendLine("/// <inheritdoc />");
        CodeGenerator.AppendLine("public ErrorCodes Code => (ErrorCodes)ErrorCode;");
    }

    private void GenerateClassHeader(string className, bool isTopLevel, MessageType messageType)
    {
        var implementedInterfaces = new HashSet<string>();

        if (isTopLevel)
        {
            var baseType = messageType switch
            {
                MessageType.Request => "Request",
                MessageType.Response => "Response",
                MessageType.Header => "",
                MessageType.Data => "",
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
            };
            implementedInterfaces.Add($"I{baseType}Message");
        }
        else
        {
            implementedInterfaces.Add("IMessage");
        }

        implementedInterfaces.Add($"IEquatable<{className}>");

        CodeGenerator.AppendLine("/// <summary>");
        CodeGenerator.AppendLine($"/// Describes the contract for message {className}");
        CodeGenerator.AppendLine("/// </summary>");
        CodeGenerator.AppendLine($"public sealed partial class {className}: {string.Join(", ", implementedInterfaces)}");
        CodeGenerator.AppendLeftBrace();
    }
}