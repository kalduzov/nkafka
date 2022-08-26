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
// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class BrokerHeartbeatRequestMessage: RequestMessage
{
    /// <summary>
    /// The broker ID.
    /// </summary>
    public int BrokerIdMessage { get; set; } = 0;

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long BrokerEpochMessage { get; set; } = -1;

    /// <summary>
    /// The highest metadata offset which the broker has reached.
    /// </summary>
    public long CurrentMetadataOffsetMessage { get; set; } = 0;

    /// <summary>
    /// True if the broker wants to be fenced, false otherwise.
    /// </summary>
    public bool WantFenceMessage { get; set; } = false;

    /// <summary>
    /// True if the broker wants to be shut down, false otherwise.
    /// </summary>
    public bool WantShutDownMessage { get; set; } = false;

    public BrokerHeartbeatRequestMessage()
    {
        ApiKey = ApiKeys.BrokerHeartbeat;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public BrokerHeartbeatRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.BrokerHeartbeat;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

}
