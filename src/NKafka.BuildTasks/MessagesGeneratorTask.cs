using System.Text;

using Microsoft.Build.Framework;

using Newtonsoft.Json;

using NKafka.MessageGenerator;
using NKafka.MessageGenerator.Specifications;

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
        Log.LogMessage(MessageImportance.High, $"Solution {SolutionDirectory} output {OutputDirectory}");
        Log.LogMessage(MessageImportance.High, "Start messages generator");

        var files = GetFileResources();

        try
        {
            var specifications = new List<MessageSpecification>();

            foreach (var fileName in files)
            {
                if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var apiDescriptor = GetApiDescriptor(fileName);
                    specifications.Add(apiDescriptor);
                }
                catch (Exception exc)
                {
                    Log.LogError($"Не удалось обработать файл {fileName}. {exc.StackTrace}");
                }
            }

            //generate messages
            foreach (var messageSpecification in specifications)
            {
                switch (messageSpecification.Type)
                {
                    case MessageType.Request or MessageType.Response or MessageType.Header:
                        {
                            IMessageGenerator messageGenerator = new MessageGenerator.MessageGenerator("NKafka.Messages");
                            var result = messageGenerator.Generate(messageSpecification);
                            var classFileName = $"{messageGenerator.ClassName(messageSpecification)}.cs";
                            WriteMessageToFile(classFileName, result);

                            break;
                        }
                }
            }

            var supportVersionsInformation = specifications.Where(s => s.Type == MessageType.Request)
                .Select(
                    s => (Name: s.Struct.Name.Replace("Request", ""), s.ApiKey, s.FlexibleVersions, s.Struct.Versions));

            var supportVersionsGenerator = new SupportVersionsGenerator("NKafka.Protocol");
            var content = supportVersionsGenerator.Generate(supportVersionsInformation);
            const string supportVersionFileName = "SupportVersionsExtensions.cs";
            WriteSupportVersionsToFile(supportVersionFileName, content);


        }
        catch (Exception exc)
        {
            Log.LogErrorFromException(exc, true, true, null);
        }
        finally
        {
            Log.LogMessage(MessageImportance.High, "Finish messages generator");
        }

        return true;
    }

    private void WriteMessageToFile(string fileName, StringBuilder result)
    {
        var path = Path.Combine(OutputDirectory, fileName);
        Directory.CreateDirectory(OutputDirectory);
        File.WriteAllText(path, result.ToString(), Encoding.UTF8);
    }

    private void WriteSupportVersionsToFile(string fileName, StringBuilder result)
    {
        var path = Path.Combine("Protocol", fileName);
        File.WriteAllText(path, result.ToString(), Encoding.UTF8);
    }

    private IEnumerable<string> GetFileResources()
    {
        var pathToResources = Path.Combine(Directory.GetCurrentDirectory(), SolutionDirectory, "resources", "message");

        return Directory.GetFiles(pathToResources, "*.json");
    }

    private static MessageSpecification GetApiDescriptor(string fileName)
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
            return JsonConvert.DeserializeObject<MessageSpecification>(str)!;
        }
        catch (Exception exc)
        {
            throw new FormatException($"Can't parse file {fileName}", exc);
        }
    }
}