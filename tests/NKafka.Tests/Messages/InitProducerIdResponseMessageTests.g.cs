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
//
// <auto-generated> THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT. </auto-generated>

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

#nullable enable

using NKafka.Messages;
using NKafka.Protocol;
using Xunit;

namespace NKafka.Tests.Messages;

public sealed partial class InitProducerIdResponseMessageTests: ResponseMessageTests<InitProducerIdResponseMessage>
{
    [Fact(DisplayName = "Check serialize and deserialize 'InitProducerIdResponseMessage' message by Version0")]
    public void SerializeAndDeserializeMessage_ApiVersion0_Success()
    {
        var message = new InitProducerIdResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            ProducerId = -9223372036854775808,
            ProducerEpoch = -4242,
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version0);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'InitProducerIdResponseMessage' message by Version1")]
    public void SerializeAndDeserializeMessage_ApiVersion1_Success()
    {
        var message = new InitProducerIdResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            ProducerId = -9223372036854775808,
            ProducerEpoch = -4242,
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version1);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'InitProducerIdResponseMessage' message by Version2")]
    public void SerializeAndDeserializeMessage_ApiVersion2_Success()
    {
        var message = new InitProducerIdResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            ProducerId = -9223372036854775808,
            ProducerEpoch = -4242,
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version2);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'InitProducerIdResponseMessage' message by Version3")]
    public void SerializeAndDeserializeMessage_ApiVersion3_Success()
    {
        var message = new InitProducerIdResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            ProducerId = -9223372036854775808,
            ProducerEpoch = -4242,
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version3);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'InitProducerIdResponseMessage' message by Version4")]
    public void SerializeAndDeserializeMessage_ApiVersion4_Success()
    {
        var message = new InitProducerIdResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            ProducerId = -9223372036854775808,
            ProducerEpoch = -4242,
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version4);
    }
}
