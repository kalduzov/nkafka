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

public sealed partial class JoinGroupResponseMessageTests: ResponseMessageTests<JoinGroupResponseMessage>
{
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version0")]
    public void SerializeAndDeserializeMessage_ApiVersion0_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version0);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version1")]
    public void SerializeAndDeserializeMessage_ApiVersion1_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version1);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version2")]
    public void SerializeAndDeserializeMessage_ApiVersion2_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version2);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version3")]
    public void SerializeAndDeserializeMessage_ApiVersion3_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version3);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version4")]
    public void SerializeAndDeserializeMessage_ApiVersion4_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version4);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version5")]
    public void SerializeAndDeserializeMessage_ApiVersion5_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version5);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version6")]
    public void SerializeAndDeserializeMessage_ApiVersion6_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version6);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version7")]
    public void SerializeAndDeserializeMessage_ApiVersion7_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolType = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version7);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version8")]
    public void SerializeAndDeserializeMessage_ApiVersion8_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolType = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version8);
    }
    [Fact(DisplayName = "Check serialize and deserialize 'JoinGroupResponseMessage' message by Version9")]
    public void SerializeAndDeserializeMessage_ApiVersion9_Success()
    {
        var message = new JoinGroupResponseMessage
        {
            ThrottleTimeMs = -420004200,
            ErrorCode = -4242,
            GenerationId = -420004200,
            ProtocolType = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            ProtocolName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Leader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            SkipAssignment = true,
            MemberId = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            Members = new (),
        };
        SerializeAndDeserializeMessage(message, ApiVersion.Version9);
    }
}
