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

namespace NKafka.MessageGenerator.Tests;

public class MessageGeneratorTests
{
    [Theory]
    [InlineData("data/DescribeQuorumResponse.json")]
    [InlineData("data/MetadataRequest.json")]
    [InlineData("data/MetadataResponse.json")]
    [InlineData("data/LeaderAndIsrResponse.json")]
    [InlineData("data/AlterUserScramCredentialsRequest.json")]
    [InlineData("data/BeginQuorumEpochRequest.json")]
    [InlineData("data/FetchRequest.json")]

    //[InlineData("data\\ConsumerProtocolAssignment.json")]
    [InlineData("data/RequestHeader.json")]
    public async Task GenerateTest_Successful(string fileName)
    {
        var specification = await JsonParseTests.GetMessageSpecification(fileName);

        var messageGenerator = new MessageGenerator("test");
        var result = messageGenerator.Generate(specification);

        result.ToString().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("data/RequestHeader.json", "writer.WriteInt16String(ClientId);")]
    [InlineData("data/MetadataRequest.json", "writer.WriteCompactString(Name);")]
    public async Task GenerateTest_UsesKafkaStringWriterMethods(string fileName, string expectedSnippet)
    {
        var specification = await JsonParseTests.GetMessageSpecification(fileName);

        var messageGenerator = new MessageGenerator("test");
        var result = messageGenerator.Generate(specification).ToString();

        result.Should().Contain(expectedSnippet);
        result.Should().NotContain("Encoding.UTF8.GetBytes");
    }

    [Theory]
    [InlineData("data/RequestHeader.json", "writer.WriteNullableInt16String(ClientId);")]
    [InlineData("data/MetadataResponse.json", "writer.WriteNullableCompactString(ClusterId);")]
    public async Task GenerateTest_UsesKafkaNullableStringWriterMethods(string fileName, string expectedSnippet)
    {
        var specification = await JsonParseTests.GetMessageSpecification(fileName);

        var messageGenerator = new MessageGenerator("test");
        var result = messageGenerator.Generate(specification).ToString();

        result.Should().Contain(expectedSnippet);
    }

    [Fact]
    public async Task GenerateTest_UsesCompactStringSizeHelperForTaggedStrings()
    {
        var specification = await JsonParseTests.GetMessageSpecification("data/FetchRequest.json");

        var messageGenerator = new MessageGenerator("test");
        var result = messageGenerator.Generate(specification).ToString();

        result.Should().Contain("writer.WriteVarUInt32(ClusterId.SizeOfCompactString());");
        result.Should().NotContain("var stringBytesCount = Encoding.UTF8.GetByteCount(ClusterId);");
    }

    [Fact]
    public void GenerateTest()
    {
        // var message = new MessageSpecification(
        //     25,
        //     MessageType.Request,
        //     new List<RequestListenerType>
        //     {
        //         RequestListenerType.ZkBroker,
        //         RequestListenerType.Broker
        //     },
        //     "AddOffsetsToTxnRequest",
        //     Versions.Parse("0-3", null!),
        //     Versions.Parse("3+", null!),
        //     new List<FieldSpecification>
        //     {
        //         // new()
        //         // {
        //         //     Name = "TransactionalId",
        //         //     About = "The transactional id corresponding to the transaction.",
        //         //     Type = IFieldType.Parse("string"),
        //         //     Versions = Versions.Parse("0+"),
        //         //     EntityType = EntityType.TransactionalId
        //         // }
        //     });
        //
        // var messageGenerator = new MessageGenerator("test");
        // var result = messageGenerator.Generate(message);
        // result.ToString().Should().NotBeEmpty();
    }
}
