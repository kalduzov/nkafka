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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class ListPartitionReassignmentsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top-level error code, or 0 if there was no error
    /// </summary>
    public short ErrorCode { get; set; }

    /// <summary>
    /// The top-level error message, or null if there was no error.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// The ongoing reassignments for each topic.
    /// </summary>
    public IReadOnlyCollection<OngoingTopicReassignmentMessage> Topics { get; set; }

    public ListPartitionReassignmentsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public ListPartitionReassignmentsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version0)
        {
        }
        else //no flexible version
        {
        }

    }

    public class OngoingTopicReassignmentMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ongoing reassignments for each partition.
        /// </summary>
        public IReadOnlyCollection<OngoingPartitionReassignmentMessage> Partitions { get; set; }

        public OngoingTopicReassignmentMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public OngoingTopicReassignmentMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version0)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class OngoingPartitionReassignmentMessage: Message
    {
        /// <summary>
        /// The index of the partition.
        /// </summary>
        public int PartitionIndex { get; set; }

        /// <summary>
        /// The current replica set.
        /// </summary>
        public IReadOnlyCollection<int> Replicas { get; set; }

        /// <summary>
        /// The set of replicas we are currently adding.
        /// </summary>
        public IReadOnlyCollection<int> AddingReplicas { get; set; }

        /// <summary>
        /// The set of replicas we are currently removing.
        /// </summary>
        public IReadOnlyCollection<int> RemovingReplicas { get; set; }

        public OngoingPartitionReassignmentMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public OngoingPartitionReassignmentMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version0)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}