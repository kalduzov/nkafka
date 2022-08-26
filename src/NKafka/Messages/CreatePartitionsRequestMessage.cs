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

public sealed class CreatePartitionsRequestMessage: RequestMessage
{
    /// <summary>
    /// Each topic that we want to create new partitions inside.
    /// </summary>
    public List<CreatePartitionsTopicMessage> TopicsMessage { get; set; } = new ();

    /// <summary>
    /// The time in ms to wait for the partitions to be created.
    /// </summary>
    public int TimeoutMsMessage { get; set; } = 0;

    /// <summary>
    /// If true, then validate the request, but don't actually increase the number of partitions.
    /// </summary>
    public bool ValidateOnlyMessage { get; set; } = false;

    public CreatePartitionsRequestMessage()
    {
        ApiKey = ApiKeys.CreatePartitions;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public CreatePartitionsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.CreatePartitions;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class CreatePartitionsTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public Dictionary<string,> NameMessage { get; set; } = "";

        /// <summary>
        /// The new partition count.
        /// </summary>
        public Dictionary<int,> CountMessage { get; set; } = 0;

        /// <summary>
        /// The new partition assignments.
        /// </summary>
        public List<CreatePartitionsAssignmentMessage> AssignmentsMessage { get; set; } = new ();

        public CreatePartitionsTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public CreatePartitionsTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }

    public sealed class CreatePartitionsAssignmentMessage: Message
    {
        /// <summary>
        /// The assigned broker IDs.
        /// </summary>
        public List<int> BrokerIdsMessage { get; set; } = new ();

        public CreatePartitionsAssignmentMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public CreatePartitionsAssignmentMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }
}
