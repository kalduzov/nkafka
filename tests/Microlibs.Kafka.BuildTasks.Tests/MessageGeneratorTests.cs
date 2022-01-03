using System.IO;
using Microlibs.Kafka.BuildTasks;
using Xunit;

namespace Microlibs.Kafka.BuildTasks.Tests;

public class MessageGeneratorTests
{
    private readonly MessageGenerator _messageGenerator;

    public MessageGeneratorTests()
    {
        var pathToSolutions = Path.Combine("..", "..", "..", "..", "..");
        _messageGenerator = new MessageGenerator(pathToSolutions, "responses");
    }

    [Fact]
    public void GenerateTest()
    {
        _messageGenerator.Generate();
    }
}