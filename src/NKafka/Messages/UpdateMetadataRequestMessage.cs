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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class UpdateMetadataRequestMessage: RequestMessage
{
    /// <summary>
    /// The controller id.
    /// </summary>
    public int ControllerId { get; set; } = 0;

    /// <summary>
    /// The controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; } = 0;

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// In older versions of this RPC, each partition that we would like to update.
    /// </summary>
    public List<UpdateMetadataPartitionStateMessage> UngroupedPartitionStates { get; set; } = new ();

    /// <summary>
    /// In newer versions of this RPC, each topic that we would like to update.
    /// </summary>
    public List<UpdateMetadataTopicStateMessage> TopicStates { get; set; } = new ();

    /// <summary>
    /// 
    /// </summary>
    public List<UpdateMetadataBrokerMessage> LiveBrokers { get; set; } = new ();

    public UpdateMetadataRequestMessage()
    {
        ApiKey = ApiKeys.UpdateMetadata;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public UpdateMetadataRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.UpdateMetadata;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ControllerId);
        writer.WriteInt(ControllerEpoch);
        if (version >= ApiVersions.Version5)
        {
            writer.WriteLong(BrokerEpoch);
        }
        if (version <= ApiVersions.Version4)
        {
            writer.WriteInt(UngroupedPartitionStates.Count);
            foreach (var element in UngroupedPartitionStates)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (UngroupedPartitionStates.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default UngroupedPartitionStates at version {version}");
            }
        }
        if (version >= ApiVersions.Version5)
        {
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(TopicStates.Count + 1);
                foreach (var element in TopicStates)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(TopicStates.Count);
                foreach (var element in TopicStates)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (TopicStates.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TopicStates at version {version}");
            }
        }
        if (version >= ApiVersions.Version6)
        {
            writer.WriteVarUInt(LiveBrokers.Count + 1);
            foreach (var element in LiveBrokers)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(LiveBrokers.Count);
            foreach (var element in LiveBrokers)
            {
                element.Write(writer, version);
            }
        }
    }

    public sealed class UpdateMetadataTopicStateMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = "";

        /// <summary>
        /// The topic id.
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partition that we would like to update.
        /// </summary>
        public List<UpdateMetadataPartitionStateMessage> PartitionStates { get; set; } = new ();

        public UpdateMetadataTopicStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public UpdateMetadataTopicStateMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of UpdateMetadataTopicStateMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version7)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(PartitionStates.Count + 1);
                foreach (var element in PartitionStates)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(PartitionStates.Count);
                foreach (var element in PartitionStates)
                {
                    element.Write(writer, version);
                }
            }
        }
    }

    public sealed class UpdateMetadataBrokerMessage: Message
    {
        /// <summary>
        /// The broker id.
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public string V0Host { get; set; } = "";

        /// <summary>
        /// The broker port.
        /// </summary>
        public int V0Port { get; set; } = 0;

        /// <summary>
        /// The broker endpoints.
        /// </summary>
        public List<UpdateMetadataEndpointMessage> Endpoints { get; set; } = new ();

        /// <summary>
        /// The rack which this broker belongs to.
        /// </summary>
        public string Rack { get; set; } = "";

        public UpdateMetadataBrokerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public UpdateMetadataBrokerMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Id);
            if (version <= ApiVersions.Version0)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(V0Host);
                    writer.WriteShort((short)stringBytes.Length);
                    writer.WriteBytes(stringBytes);
                }
            }
            if (version <= ApiVersions.Version0)
            {
                writer.WriteInt(V0Port);
            }
            if (version >= ApiVersions.Version1)
            {
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(Endpoints.Count + 1);
                    foreach (var element in Endpoints)
                    {
                        element.Write(writer, version);
                    }
                }
                else
                {
                    writer.WriteInt(Endpoints.Count);
                    foreach (var element in Endpoints)
                    {
                        element.Write(writer, version);
                    }
                }
            }
            if (version >= ApiVersions.Version2)
            {
                if (Rack is null)
                {
                    if (version >= ApiVersions.Version6)
                    {
                        writer.WriteVarUInt(0);
                    }
                    else
                    {
                        writer.WriteShort(-1);
                    }
                }
                else
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Rack);
                    if (version >= ApiVersions.Version6)
                    {
                        writer.WriteVarUInt(stringBytes.Length + 1);
                    }
                    else
                    {
                        writer.WriteShort((short)stringBytes.Length);
                    }
                    writer.WriteBytes(stringBytes);
                }
            }
        }
    }

    public sealed class UpdateMetadataEndpointMessage: Message
    {
        /// <summary>
        /// The port of this endpoint
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// The hostname of this endpoint
        /// </summary>
        public string Host { get; set; } = "";

        /// <summary>
        /// The listener name.
        /// </summary>
        public string Listener { get; set; } = "";

        /// <summary>
        /// The security protocol type.
        /// </summary>
        public short SecurityProtocol { get; set; } = 0;

        public UpdateMetadataEndpointMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public UpdateMetadataEndpointMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of UpdateMetadataEndpointMessage");
            }
            var numTaggedFields = 0;
            writer.WriteInt(Port);
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version3)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Listener);
                    if (version >= ApiVersions.Version6)
                    {
                        writer.WriteVarUInt(stringBytes.Length + 1);
                    }
                    else
                    {
                        writer.WriteShort((short)stringBytes.Length);
                    }
                    writer.WriteBytes(stringBytes);
                }
            }
            writer.WriteShort(SecurityProtocol);
        }
    }

    public sealed class UpdateMetadataPartitionStateMessage: Message
    {
        /// <summary>
        /// In older versions of this RPC, the topic name.
        /// </summary>
        public string TopicName { get; set; } = "";

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The controller epoch.
        /// </summary>
        public int ControllerEpoch { get; set; } = 0;

        /// <summary>
        /// The ID of the broker which is the current partition leader.
        /// </summary>
        public int Leader { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The brokers which are in the ISR for this partition.
        /// </summary>
        public List<int> Isr { get; set; } = new ();

        /// <summary>
        /// The Zookeeper version.
        /// </summary>
        public int ZkVersion { get; set; } = 0;

        /// <summary>
        /// All the replicas of this partition.
        /// </summary>
        public List<int> Replicas { get; set; } = new ();

        /// <summary>
        /// The replicas of this partition which are offline.
        /// </summary>
        public List<int> OfflineReplicas { get; set; } = new ();

        public UpdateMetadataPartitionStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        public UpdateMetadataPartitionStateMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version7;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            if (version <= ApiVersions.Version4)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                    writer.WriteShort((short)stringBytes.Length);
                    writer.WriteBytes(stringBytes);
                }
            }
            writer.WriteInt(PartitionIndex);
            writer.WriteInt(ControllerEpoch);
            writer.WriteInt(Leader);
            writer.WriteInt(LeaderEpoch);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(Isr.Count + 1);
            }
            else
            {
                writer.WriteInt(Isr.Count);
            }
            foreach (var element in Isr)
            {
                writer.WriteInt(element);
            }
            writer.WriteInt(ZkVersion);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(Replicas.Count + 1);
            }
            else
            {
                writer.WriteInt(Replicas.Count);
            }
            foreach (var element in Replicas)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersions.Version4)
            {
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(OfflineReplicas.Count + 1);
                }
                else
                {
                    writer.WriteInt(OfflineReplicas.Count);
                }
                foreach (var element in OfflineReplicas)
                {
                    writer.WriteInt(element);
                }
            }
        }
    }
}
