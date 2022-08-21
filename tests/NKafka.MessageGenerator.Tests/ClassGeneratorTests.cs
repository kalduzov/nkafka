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

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class ClassGeneratorTests
{
    [Fact]
    public void GenerateTest()
    {
        var descriptor = new ApiDescriptor
        {
            Listeners = new List<string>
            {
                "zkBroker",
                "broker"
            },
            ApiKey = 24,
            Name = "AddPartitionsToTxnRequest",
            Type = ApiMessageType.Request,
            ValidVersions = Versions.Parse("0-3")!,
            FlexibleVersions = Versions.Parse("3+")!,
            Fields = new List<FieldDescriptor>
            {
                new()
                {
                    Name = "TransactionalId",
                    About = "The transactional id corresponding to the transaction.",
                    Type = IFieldType.Parse("string"),
                    Versions = Versions.Parse("0+"),
                    EntityType = "transactionalId"
                },
                new()
                {
                    Name = "Topics",
                    About = "The partitions to add to the transaction.",
                    Type = IFieldType.Parse("[]AddPartitionsToTxnTopic"),
                    Versions = Versions.Parse("0+"),
                    Fields = new List<FieldDescriptor>
                    {
                        new()
                        {
                            Name = "Name",
                            Type = IFieldType.Parse("string"),
                            Versions = Versions.Parse("0+"),
                            MapKey = true,
                            EntityType = "topicName",
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
            }
        };

        var writeMock = new Mock<IWriteMethodGenerator>();
        writeMock.Setup(x => x.Generate(It.IsAny<List<FieldDescriptor>>(), It.IsAny<int>())).Returns(new StringBuilder());

        var readMock = new Mock<IReadMethodGenerator>();
        readMock.Setup(x => x.Generate(It.IsAny<List<FieldDescriptor>>(), It.IsAny<int>())).Returns(new StringBuilder());

        var classGenerator = new ClassGenerator(descriptor, writeMock.Object, readMock.Object);
        var result = classGenerator.Generate();

        result.ToString().Should().NotBeEmpty();
    }
}