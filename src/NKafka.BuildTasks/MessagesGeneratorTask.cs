﻿using System.Text;

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
        const string messageSuffix = "Message";

        Log.LogMessage(MessageImportance.High, $"Solution {SolutionDirectory} output {OutputDirectory}");
        Log.LogMessage(MessageImportance.High, "Start messages generator");

        var files = GetFileResources();

        try
        {
            foreach (var fileName in files)
            {
                if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var apiDescriptor = GetApiDescriptor(fileName);

                    switch (apiDescriptor.Type)
                    {
                        case MessageType.Request or MessageType.Response or MessageType.Header:
                        {
                            IMessageGenerator messageGenerator = new MessageGenerator.MessageGenerator("NKafka.Messages");
                            var result = messageGenerator.Generate(apiDescriptor);
                            var classFileName = $"{messageGenerator.ClassName(apiDescriptor)}.cs";
                            WriteContentToFile(classFileName, result);

                            break;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Log.LogError($"Не удалось обработать файл {fileName}. {exc.StackTrace}");
                }
            }
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

    private void WriteContentToFile(string fileName, StringBuilder result)
    {
        var path = Path.Combine(OutputDirectory, fileName);
        Directory.CreateDirectory(OutputDirectory);
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