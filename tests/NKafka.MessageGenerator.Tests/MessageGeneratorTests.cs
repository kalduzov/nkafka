using System.Text;

using FluentAssertions;

using Moq;

using NKafka.MessageGenerator.Specifications;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class MessageGeneratorTests
{
    [Fact]
    public void GenerateTest()
    {
        var descriptor = new MessageSpecification(
            25,
            MessageType.Request,
            new List<RequestListenerType>
            {
                RequestListenerType.ZkBroker,
                RequestListenerType.Broker
            },
            "AddOffsetsToTxnRequest",
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
                }
            });

        var headerGeneratorMock = new Mock<IHeaderGenerator>();
        headerGeneratorMock.Setup(x => x.Generate()).Returns(new StringBuilder());
        var classGeneratorMock = new Mock<IClassGenerator>();
        classGeneratorMock.Setup(x => x.Generate()).Returns(new StringBuilder());

        var messageGenerator = new MessageGenerator(descriptor, headerGeneratorMock.Object, classGeneratorMock.Object);
        var result = messageGenerator.Generate();
        result.ToString().Should().NotBeEmpty();
    }
}