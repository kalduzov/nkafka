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

using FluentAssertions;

using Moq;

using NKafka.MessageGenerator.Specifications;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class ClassGeneratorTests
{
    [Fact]
    public void GenerateRequestTest()
    {
        var descriptor = new MessageSpecification(
            24,
            MessageType.Request,
            new List<RequestListenerType>
            {
                RequestListenerType.ZkBroker,
                RequestListenerType.Broker
            },
            "AddPartitionsToTxnRequest",
            Versions.Parse("0-3")!,
            Versions.Parse("3+")!,
            new List<FieldSpecification>
            {
                new()
                {
                    Name = "TransactionalId",
                    About = "The transactional id corresponding to the transaction.",
                    Type = IFieldType.Parse("string"),
                    Versions = Versions.Parse("0+"),
                    EntityType = EntityType.TransactionalId
                },
                new()
                {
                    Name = "Topics",
                    About = "The partitions to add to the transaction.",
                    Type = IFieldType.Parse("[]AddPartitionsToTxnTopic"),
                    Versions = Versions.Parse("0+"),
                    Fields = new List<FieldSpecification>
                    {
                        new()
                        {
                            Name = "Name",
                            Type = IFieldType.Parse("string"),
                            Versions = Versions.Parse("0+"),
                            MapKey = true,
                            EntityType = EntityType.TopicName,
                            About = "The name of the topic."
                        },
                        new()
                        {
                            Name = "Partitions",
                            Type = IFieldType.Parse("[]int32"),
                            Versions = Versions.Parse("0+"),
                            About = "The partition indexes to add to the transaction"
                        }
                    }
                }
            });

        var writeMock = new Mock<IWriteMethodGenerator>();
        writeMock.Setup(x => x.Generate(It.IsAny<List<FieldSpecification>>(), It.IsAny<int>())).Returns(new StringBuilder());

        var readMock = new Mock<IReadMethodGenerator>();
        readMock.Setup(x => x.Generate(It.IsAny<List<FieldSpecification>>(), It.IsAny<int>())).Returns(new StringBuilder());

        var classGenerator = new ClassGenerator(descriptor, writeMock.Object, readMock.Object);
        var result = classGenerator.Generate();

        result.ToString().Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateResponseTest()
    {
        var descriptor = new MessageSpecification(
            17,
            MessageType.Response,
            new List<RequestListenerType>
            {
                Capacity = 0
            },
            "SaslHandshakeResponse",
            Versions.Parse("0-1")!,
            Versions.Parse("none")!,
            new List<FieldSpecification>
            {
                new()
                {
                    Name = "ErrorCode",
                    About = "The error code, or 0 if there was no error.",
                    Type = IFieldType.Parse("int16"),
                    Versions = Versions.Parse("0+")
                },
                new()
                {
                    Name = "Mechanisms",
                    Type = IFieldType.Parse("[]string"),
                    Versions = Versions.Parse("0+"),
                    About = "The mechanisms enabled in the server."
                }
            });

        var writeMock = new Mock<IWriteMethodGenerator>();
        writeMock.Setup(x => x.Generate(It.IsAny<List<FieldSpecification>>(), It.IsAny<int>())).Returns(new StringBuilder());

        var readMock = new Mock<IReadMethodGenerator>();
        readMock.Setup(x => x.Generate(It.IsAny<List<FieldSpecification>>(), It.IsAny<int>())).Returns(new StringBuilder());

        var classGenerator = new ClassGenerator(descriptor, writeMock.Object, readMock.Object);
        var result = classGenerator.Generate().ToString();

        result.Should().NotBeEmpty();
    }
}