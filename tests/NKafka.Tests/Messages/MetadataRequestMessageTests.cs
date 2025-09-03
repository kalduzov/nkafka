// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2025 Aleksey Kalduzov. All rights reserved
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

using NKafka.Messages;
using NKafka.Protocol;

namespace NKafka.Tests.Messages;

public sealed partial class MetadataRequestMessageTests
{
    [Fact]
    public void SerializeAndDeserializeMessage_ApiVersion12_DeepClasses_Success()
    {
        var message = new MetadataRequestMessage
        {
            Topics =
            {
                new MetadataRequestMessage.MetadataRequestTopicMessage
                {
                    TopicId = Guid.Parse("73657405-0074-0101-0000-000000000054"),
                    Name = "test1"
                },
                new MetadataRequestMessage.MetadataRequestTopicMessage
                {
                    TopicId = Guid.Parse("43657405-0074-0101-0000-100000000054"),
                    Name = "test2"
                }
            },
            AllowAutoTopicCreation = true,
            IncludeTopicAuthorizedOperations = true,
        };
        message.SerializeAndDeserializeMessageTest(ApiVersion.Version12);
    }
}