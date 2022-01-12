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

public partial class LeaderAndIsrRequestMessage: RequestMessage
{
    /// <summary>
    /// The current controller ID.
    /// </summary>
    public int ControllerId { get; set; }

    /// <summary>
    /// The current controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; }

    /// <summary>
    /// The current broker epoch.
    /// </summary>
    public long? BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// The type that indicates whether all topics are included in the request
    /// </summary>
    public sbyte Type { get; set; }

    /// <summary>
    /// The state of each partition, in a v0 or v1 message.
    /// </summary>
    public IReadOnlyCollection<LeaderAndIsrPartitionStateMessage> UngroupedPartitionStates { get; set; }

    /// <summary>
    /// Each topic.
    /// </summary>
    public IReadOnlyCollection<LeaderAndIsrTopicStateMessage> TopicStates { get; set; }

    /// <summary>
    /// The current live leaders.
    /// </summary>
    public IReadOnlyCollection<LeaderAndIsrLiveLeaderMessage> LiveLeaders { get; set; }

    public LeaderAndIsrRequestMessage()
    {
        ApiKey = ApiKeys.LeaderAndIsr;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class LeaderAndIsrPartitionStateMessage: Message
    {

            public LeaderAndIsrPartitionStateMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version5;
            }

            public override void Read(BufferReader reader, ApiVersions version)
            {
            }

            public override void Write(BufferWriter writer, ApiVersions version)
            {
            }
        }
        public class LeaderAndIsrTopicStateMessage: Message
        {
            /// <summary>
            /// The topic name.
            /// </summary>
            public string TopicName { get; set; }

            /// <summary>
            /// The unique topic ID.
            /// </summary>
            public Guid? TopicId { get; set; }

            /// <summary>
            /// The state of each partition
            /// </summary>
            public IReadOnlyCollection<LeaderAndIsrPartitionStateMessage> PartitionStates { get; set; }

            public LeaderAndIsrTopicStateMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version5;
            }

            public override void Read(BufferReader reader, ApiVersions version)
            {
            }

            public override void Write(BufferWriter writer, ApiVersions version)
            {
            }
        }
        public class LeaderAndIsrLiveLeaderMessage: Message
        {
            /// <summary>
            /// The leader's broker ID.
            /// </summary>
            public int BrokerId { get; set; }

            /// <summary>
            /// The leader's hostname.
            /// </summary>
            public string HostName { get; set; }

            /// <summary>
            /// The leader's port.
            /// </summary>
            public int Port { get; set; }

            public LeaderAndIsrLiveLeaderMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version5;
            }

            public override void Read(BufferReader reader, ApiVersions version)
            {
            }

            public override void Write(BufferWriter writer, ApiVersions version)
            {
            }
        }
}