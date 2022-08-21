using System.Resources;

using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging.Abstractions;

using NKafka.BuildTasks;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class MessageGeneratorTests
{
    private readonly MessageGenerator _messageGenerator;

    public MessageGeneratorTests()
    {
        var pathToSolutions = Path.Combine("..", "..", "..", "..", "..");
        //_messageGenerator = new MessageGenerator(pathToSolutions, "message", "NKafka.Tests", NullLogger.Instance);
    }

    [Fact]
    public void GenerateTest()
    {
        //_messageGenerator.Generate();
    }
}