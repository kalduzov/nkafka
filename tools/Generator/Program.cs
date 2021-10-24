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

using System.Text;

using Newtonsoft.Json;

using NKafka.MessageGenerator;
using NKafka.MessageGenerator.Specifications;

Console.WriteLine("Kafka classes generator");
Console.WriteLine();

var solutionDirectory = GetRootDirectory(args[0]);
var outputDirectory = GetOutPutDirectory(args[1]);

//private readonly string messagesDirectory = 

var command = args.Length == 3 ? args[2].ToLowerInvariant() : "messages";

switch (command)
{
    case "messages":
        {
            GenerateMessages();

            break;
        }
    case "tests":
        {
            GenerateTests();

            break;
        }

    default:
        {
            GenerateMessages();
            GenerateTests();

            break;
        }
}

void GenerateMessages()
{
    Console.WriteLine("Start messages generator");

    var files = GetFileResources();
    var specifications = GetMessageSpecifications(files);

    try
    {
        //generate messages
        foreach (var messageSpecification in specifications)
        {
            switch (messageSpecification.Type)
            {
                case MessageType.Request or MessageType.Response or MessageType.Header:
                    {
                        IMessageGenerator messageGenerator = new MessageGenerator("NKafka.Messages");
                        var result = messageGenerator.Generate(messageSpecification);
                        var classFileName = $"{messageGenerator.ClassName(messageSpecification)}.g.cs";
                        WriteMessageToFile(classFileName, result);

                        break;
                    }
            }
        }

        // generate SupportVersionsExtensions
        var supportVersionsInformation = specifications.Where(s => s.Type == MessageType.Request)
            .Select(s => (Name: s.Struct.Name.Replace("Request", ""), s.ApiKey, s.FlexibleVersions, s.Struct.Versions));

        var supportVersionsGenerator = new SupportVersionsGenerator("NKafka.Protocol");
        var content = supportVersionsGenerator.Generate(supportVersionsInformation);
        const string supportVersionFileName = "SupportVersionsExtensions.g.cs";
        WriteSupportVersionsToFile(supportVersionFileName, content);

        // generate ResponseBuilder

        var responseInformation = specifications.Where(s => s.Type == MessageType.Response)
            .Select(s => (ApiKeyName: s.Struct.Name.Replace("Response", ""), s.ApiKey, s.ClassName));

        var responseBuilderGenerator = new ResponseBuilderGenerator("NKafka.Protocol");
        content = responseBuilderGenerator.Generate(responseInformation);
        const string responseBuilderFileName = "ResponseBuilder.g.cs";
        WriteResponseBuilderToFile(responseBuilderFileName, content);

        // generate RequestBuilder

        var requestInformation = specifications.Where(s => s.Type == MessageType.Request)
            .Select(s => (ApiKeyName: s.Struct.Name.Replace("Request", ""), s.ApiKey, s.ClassName));

        var requestBuilderGenerator = new RequestBuilderGenerator("NKafka.Protocol");
        content = requestBuilderGenerator.Generate(requestInformation);
        const string requestBuilderFileName = "RequestBuilder.g.cs";
        WriteResponseBuilderToFile(requestBuilderFileName, content);
    }
    catch (Exception exc)
    {
        Console.WriteLine(exc);
    }
    finally
    {
        Console.WriteLine("Finish messages generator");
    }
}

void GenerateTests()
{
    Console.WriteLine("Start tests generator");

    var files = GetFileResources();
    var specifications = GetMessageSpecifications(files);

    try
    {
        foreach (var messageSpecification in specifications)
        {
            switch (messageSpecification.Type)
            {
                case MessageType.Request:
                case MessageType.Response:
                case MessageType.Header:
                    {
                        ITestsMessageGenerator generator = new TestMessageGenerator("NKafka.Tests.Messages");
                        var result = generator.Generate(messageSpecification);
                        var classFileName = $"{generator.ClassName(messageSpecification)}.g.cs";
                        WriteTestToFile(classFileName, result);

                        break;
                    }
            }
        }
    }
    finally
    {
        Console.WriteLine("Finish tests generator");
    }
}

List<MessageSpecification> GetMessageSpecifications(IEnumerable<string> enumerable)
{
    var specifications = new List<MessageSpecification>();

    foreach (var fileName in enumerable)
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
            Console.WriteLine($"Не удалось обработать файл {fileName}. {exc.StackTrace}");
        }
    }

    return specifications;
}

void WriteMessageToFile(string fileName, StringBuilder result)
{
    var path = Path.Combine(outputDirectory, "Messages", fileName);
    Directory.CreateDirectory(outputDirectory);
    File.WriteAllText(path, result.ToString(), Encoding.UTF8);
}

void WriteTestToFile(string fileName, StringBuilder result)
{
    var path = Path.Combine(GetMessagesTestsDirectory(), fileName);
    Directory.CreateDirectory(outputDirectory);
    File.WriteAllText(path, result.ToString(), Encoding.UTF8);
}

void WriteSupportVersionsToFile(string fileName, StringBuilder result)
{
    var path = Path.Combine(outputDirectory, "Protocol", fileName);
    File.WriteAllText(path, result.ToString(), Encoding.UTF8);
}

void WriteResponseBuilderToFile(string fileName, StringBuilder result)
{
    var path = Path.Combine(outputDirectory, "Protocol", fileName);
    File.WriteAllText(path, result.ToString(), Encoding.UTF8);
}

IEnumerable<string> GetFileResources()
{
    var pathToResources = Path.Combine(solutionDirectory, "resources", "message");

    return Directory.GetFiles(pathToResources, "*.json");
}

string GetMessagesTestsDirectory()
{
    return Path.Combine(solutionDirectory, "tests", "NKafka.Tests", "Messages");
}

static MessageSpecification GetApiDescriptor(string fileName)
{
    var str = string.Empty;

    using var fileStream = File.OpenText(fileName);
    {
        while (!fileStream.EndOfStream)
        {
            var line = fileStream.ReadLine()!.Trim();

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

string GetRootDirectory(string path)
{
    return path;
}

string GetOutPutDirectory(string path)
{
    return path;
}