using System.Text;

using Microsoft.Extensions.Logging;

using NKafka.MessageGenerator.Specifications;

namespace NKafka.MessageGenerator;

/// <summary>
/// Генератор классов для requests и responses из папки resources
/// </summary>
public class MessageGenerator: IMessageGenerator
{
    private const string _MESSAGE_SUFFIX = "Message";

    private readonly MessageSpecification _messageSpecification;
    private readonly IHeaderGenerator _headerGenerator;
    private readonly IClassGenerator _classGenerator;
    private readonly string _solutionDirectory;
    private readonly string _outputDirectory;
    private readonly string _testMessageProject;
    private readonly ILogger _logger;
    private static readonly Dictionary<int, string> _indents = new();

    public string ClassName { get; }

    public MessageGenerator(MessageSpecification messageSpecification, IHeaderGenerator headerGenerator, IClassGenerator classGenerator)
    {
        _messageSpecification = messageSpecification;
        _headerGenerator = headerGenerator;
        _classGenerator = classGenerator;

        ClassName = $"{messageSpecification.Name}{_MESSAGE_SUFFIX}";
    }

    public StringBuilder Generate()
    {
        var builder = new StringBuilder();

        _headerGenerator.AppendUsing("System.Text");
        _headerGenerator.AppendUsing("NKafka.Protocol");
        _headerGenerator.AppendUsing("NKafka.Protocol.Records");
        _headerGenerator.AppendUsing("NKafka.Protocol.Extensions");

        var header = _headerGenerator.Generate();
        builder.Append(header);
        var @class = _classGenerator.Generate();
        builder.Append(@class);

        return builder;
    }
}