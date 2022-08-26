using FluentAssertions;

using NKafka.MessageGenerator.Specifications;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class MessageGeneratorTests
{
    [Theory]
    [InlineData("data\\DescribeQuorumResponse.json")]
    [InlineData("data\\MetadataRequest.json")]
    [InlineData("data\\MetadataResponse.json")]
    [InlineData("data\\LeaderAndIsrResponse.json")]
    [InlineData("data\\AlterUserScramCredentialsRequest.json")]
    [InlineData("data\\BeginQuorumEpochRequest.json")]
    [InlineData("data\\FetchRequest.json")]
    [InlineData("data\\ConsumerProtocolAssignment.json")]
    [InlineData("data\\RequestHeader.json")]
    public async Task GenerateTest_Successful(string fileName)
    {
        var specification = await JsonParseTests.GetMessageSpecification(fileName);

        var messageGenerator = new MessageGenerator("test");
        var result = messageGenerator.Generate(specification);
        
        result.ToString().Should().NotBeEmpty();
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