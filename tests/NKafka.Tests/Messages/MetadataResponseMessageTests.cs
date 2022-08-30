﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
using NKafka.Messages;
using NKafka.Protocol;

using Xunit;

namespace NKafka.Tests.Messages;

public class MetadataResponseMessageTests: ResponseMessageTests<MetadataResponseMessage>
{
    [Theory(DisplayName = "Check serialize and deserialize 'MetadataResponseMessage' message")]
    [InlineData(ApiVersion.Version0)]
    [InlineData(ApiVersion.Version1)]
    [InlineData(ApiVersion.Version2)]
    [InlineData(ApiVersion.Version3)]
    [InlineData(ApiVersion.Version4)]
    [InlineData(ApiVersion.Version5)]
    [InlineData(ApiVersion.Version6)]
    [InlineData(ApiVersion.Version7)]
    [InlineData(ApiVersion.Version8)]
    [InlineData(ApiVersion.Version9)]
    [InlineData(ApiVersion.Version10)]
    [InlineData(ApiVersion.Version11)]
    [InlineData(ApiVersion.Version12)]
    public void SerializeAndDeserializeMessage_Success(ApiVersion version)
    {
        var message = new MetadataResponseMessage
        {
        };
        SerializeAndDeserializeMessage(message, version);
    }
}
