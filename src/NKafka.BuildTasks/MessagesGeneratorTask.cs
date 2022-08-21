using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Build.Framework;

using NKafka.MessageGenerator;

using ILogger = Microsoft.Extensions.Logging.ILogger;
using Task = Microsoft.Build.Utilities.Task;

namespace NKafka.BuildTasks;

public class MessagesGeneratorTask: Task
{
    [Required]
    public string SolutionDirectory { get; set; }

    [Required]
    public string OutputDirectory { get; set; }

    [Required]
    public string TestsMessageProject { get; set; }

    public override bool Execute()
    {
        const string messageSuffix = "Message";

        // Log.LogMessage(MessageImportance.High, $"Solution {SolutionDirectory} output {OutputDirectory}");
        // Log.LogMessage(MessageImportance.High, "Start messages generator");

        var files = GetFileResources();

        try
        {
            foreach (var fileName in files)
            {
                if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var apiDescriptor = GetApiDescriptor(fileName);

                switch (apiDescriptor.Type)
                {
                    case ApiMessageType.Request or ApiMessageType.Response:
                    {
                        var messageGenerator = BuildMessageGenerator(apiDescriptor);
                        var result = messageGenerator.Generate();
                        var classFileName = $"{messageGenerator.ClassName}.cs";
                        WriteContentToFile(classFileName, result);

                        break;
                    }
                }
            }
        }
        catch (Exception exc)
        {
            //Log.LogError(exc.Message);
        }
        finally
        {
            //Log.LogMessage(MessageImportance.High, "Finish messages generator");
        }

        return true;
    }

    private void WriteContentToFile(string fileName, StringBuilder result)
    {
        var path = Path.Combine(OutputDirectory, fileName);
        Directory.CreateDirectory(OutputDirectory);
        File.WriteAllText(path, result.ToString(), Encoding.UTF8);
    }

    private static IMessageGenerator BuildMessageGenerator(ApiDescriptor apiDescriptor)
    {
        var headerGenerator = new HeaderGenerator("NKafka.Messages");
        var readMethodGenerator = new ReadMethodGenerator(apiDescriptor);
        var writeMethodGenerator = new WriteMethodGenerator(apiDescriptor);
        var classGenerator = new ClassGenerator(apiDescriptor, writeMethodGenerator, readMethodGenerator);

        return new MessageGenerator.MessageGenerator(apiDescriptor, headerGenerator, classGenerator);
    }

    private IEnumerable<string> GetFileResources()
    {
        var pathToResources = Path.Combine(Directory.GetCurrentDirectory(), SolutionDirectory, "resources", "message");

        return Directory.GetFiles(pathToResources, "*.json");
    }

    private static ApiDescriptor GetApiDescriptor(string fileName)
    {
        var str = string.Empty;

        using var fileStream = File.OpenText(fileName);
        {
            while (!fileStream.EndOfStream)
            {
                var line = fileStream.ReadLine()?.Trim();

                if (line.StartsWith("//"))
                {
                    continue;
                }

                str += line;
            }
        }

        try
        {
            return JsonSerializer.Deserialize<ApiDescriptor>(
                str,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                })!;
        }
        catch (Exception exc)
        {
            throw new FormatException($"Can't parse file {fileName}", exc);
        }
    }
}