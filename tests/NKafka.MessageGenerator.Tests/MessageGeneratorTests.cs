using System.Text;

using FluentAssertions;

using Moq;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class MessageGeneratorTests
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
            ApiKey = 25,
            Name = "AddOffsetsToTxnRequest",
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
                }
            }
        };

        var headerGeneratorMock = new Mock<IHeaderGenerator>();
        headerGeneratorMock.Setup(x => x.Generate()).Returns(new StringBuilder());
        var classGeneratorMock = new Mock<IClassGenerator>();
        classGeneratorMock.Setup(x => x.Generate()).Returns(new StringBuilder());

        var messageGenerator = new MessageGenerator(descriptor, headerGeneratorMock.Object, classGeneratorMock.Object);
        var result = messageGenerator.Generate();
        result.ToString().Should().NotBeEmpty();
    }
}