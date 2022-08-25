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

using System.Text.Json;
using System.Text.Json.Serialization;

using FluentAssertions;

using NKafka.MessageGenerator.Specifications;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class JsonParseTests
{
    [Fact]
    public async Task ParseRequestType_Successful()
    {
        var request = await GetMessageSpecification("data/MetadataRequest.json");
        request.Name.Should().Be("MetadataRequest");
        request.Type.Should().Be(MessageType.Request);
        request.Listeners.Should()
            .BeEquivalentTo(
                new[]
                {
                    RequestListenerType.Broker,
                    RequestListenerType.ZkBroker
                });
        request.ApiKey.Should().Be(3);
        request.FlexibleVersions.Highest.Should().Be(short.MaxValue);
        request.FlexibleVersions.Lowest.Should().Be(9);
        request.ValidVersions.Highest.Should().Be(12);
        request.ValidVersions.Lowest.Should().Be(0);
        request.Fields.Count.Should().Be(4);
    }

    [Fact]
    public void ParseResponseType_Successful()
    {
    }

    private static async Task<MessageSpecification> GetMessageSpecification(string fileName)
    {
        var str = string.Empty;

        using var fileStream = File.OpenText(fileName);
        {
            while (!fileStream.EndOfStream)
            {
                var line = (await fileStream.ReadLineAsync())!.Trim();

                if (line.StartsWith("//"))
                {
                    continue;
                }

                str += line;
            }
        }

        try
        {
            return JsonSerializer.Deserialize<MessageSpecification>(
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