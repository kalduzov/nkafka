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

using NKafka.MessageGenerator.Specifications;

namespace NKafka.MessageGenerator;

public class TestMessageGenerator: ClassGenerator, ITestsMessageGenerator
{
    public TestMessageGenerator(string ns)
        : base(ns)
    {
    }

    protected override void InternalGenerate(MessageSpecification message)
    {
        if (message.Struct.Versions.Contains(short.MaxValue))
        {
            throw new ArgumentException($"Message {message.Struct.Name} does not specify a maximum version.");
        }

        StructRegistry.Register(message);

        HeaderGenerator.AppendUsing("NKafka.Messages");
        HeaderGenerator.AppendUsing("NKafka.Protocol");
        HeaderGenerator.AppendUsing("Xunit");

        GenerateClass(message, message.ClassName, message.Struct, message.Struct.Versions);
    }

    private void GenerateClass(MessageSpecification message, string className, StructSpecification @struct, Versions parentVersions)
    {
        CodeGenerator.AppendLine();
        GenerateClassHeader(className, message.Type);
        CodeGenerator.IncrementIndent();

        for (var version = message.ValidVersions.Lowest; version <= message.ValidVersions.Highest; version++)
        {
            GenerateSerializeAndDeserializeMessageSuccessMethod(message, className, version);
        }

        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();
    }

    private void GenerateSerializeAndDeserializeMessageSuccessMethod(MessageSpecification message, string className, short apiVersion)
    {
        CodeGenerator.AppendLine($"[Fact(DisplayName = \"Check serialize and deserialize '{className}' message by Version{apiVersion}\")]");

        CodeGenerator.AppendLine($"public void SerializeAndDeserializeMessage_ApiVersion{apiVersion}_Success()");
        CodeGenerator.AppendLeftBrace();
        CodeGenerator.IncrementIndent();

        CodeGenerator.AppendLine($"var message = new {className}");
        CodeGenerator.AppendLeftBrace();

        GenerateRandomPropertyValues(message, message.Fields, apiVersion);

        CodeGenerator.AppendRightBraceWithEndSymbol(";");

        CodeGenerator.AppendLine($"SerializeAndDeserializeMessage(message, ApiVersion.Version{apiVersion});");

        CodeGenerator.DecrementIndent();
        CodeGenerator.AppendRightBrace();
    }

    private void GenerateRandomPropertyValues(MessageSpecification message, IReadOnlyCollection<FieldSpecification> fields, short apiVersion)
    {
        CodeGenerator.IncrementIndent();

        foreach (var field in fields)
        {
            if (!field.Versions.Contains(apiVersion))
            {
                continue;
            }

            var value = GetRandomValue(message, field);
            CodeGenerator.AppendLine($"{field.Name} = {value},");
        }

        CodeGenerator.DecrementIndent();
    }

    private static string GetRandomValue(MessageSpecification message, FieldSpecification field)
    {
        return field.Type switch
        {
            IFieldType.Int8FieldType => "42",
            IFieldType.Int16FieldType => "-4242",
            IFieldType.Int32FieldType => "-420004200",
            IFieldType.Int64FieldType => $"{long.MinValue}",
            IFieldType.BoolFieldType => "true",
            IFieldType.BytesFieldType => "new byte[] {0, 1, 2, 3, 4, 5, 244}",
            IFieldType.Float64FieldType => "42.42",
            IFieldType.StringFieldType =>
                "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\"",
            IFieldType.UuidFieldType => $"new Guid(\"{Guid.NewGuid()}\")",
            IFieldType.UInt16FieldType => "4242",
            IFieldType.UInt32FieldType => $"{int.MaxValue}",
            // IFieldType.ArrayType arrayType => GenerateArray(message, field, arrayType),
            // IFieldType.StructType => GenerateStruct(message, field),
            _ => "new ()"
        };
    }

    private void GenerateClassHeader(string className, MessageType messageType)
    {
        var classPrefix = messageType.ToString();

        if (messageType == MessageType.Header)
        {
            classPrefix = className.Replace("Header", "");
        }

        CodeGenerator.AppendLine($"public sealed partial class {className}Tests: {classPrefix}MessageTests<{className}>");
        CodeGenerator.AppendLeftBrace();
    }
}