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

public sealed class MetadataResponseMessage: ResponseMessage, IEquatable<MetadataResponseMessage>
{
    /// <summary>
    /// Each broker in the response.
    /// </summary>
    public MetadataResponseBrokerCollection Brokers { get; set; } = new ();

    /// <summary>
    /// The cluster ID that responding broker belongs to.
    /// </summary>
    public string? ClusterId { get; set; } = null;

    /// <summary>
    /// The ID of the controller broker.
    /// </summary>
    public int ControllerId { get; set; } = -1;

    /// <summary>
    /// Each topic in the response.
    /// </summary>
    public MetadataResponseTopicCollection Topics { get; set; } = new ();

    /// <summary>
    /// 32-bit bitfield to represent authorized operations for this cluster.
    /// </summary>
    public int ClusterAuthorizedOperations { get; set; } = -2147483648;

    public MetadataResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version12;
    }

    public MetadataResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version12;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version3)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersions.Version9)
        {
            writer.WriteVarUInt(Brokers.Count + 1);
            foreach (var element in Brokers)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Brokers.Count);
            foreach (var element in Brokers)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersions.Version2)
        {
            if (ClusterId is null)
            {
                if (version >= ApiVersions.Version9)
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
                var stringBytes = Encoding.UTF8.GetBytes(ClusterId);
                if (version >= ApiVersions.Version9)
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
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ControllerId);
        }
        if (version >= ApiVersions.Version9)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Topics.Count);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersions.Version8 && version <= ApiVersions.Version10)
        {
            writer.WriteInt(ClusterAuthorizedOperations);
        }
        else
        {
            if (ClusterAuthorizedOperations != -2147483648)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ClusterAuthorizedOperations at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version9)
        {
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }
        else
        {
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MetadataResponseMessage other && Equals(other);
    }

    public bool Equals(MetadataResponseMessage? other)
    {
        return true;
    }

    public sealed class MetadataResponseBrokerMessage: Message, IEquatable<MetadataResponseBrokerMessage>
    {
        /// <summary>
        /// The broker ID.
        /// </summary>
        public int NodeId { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The broker port.
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// The rack of the broker, or null if it has not been assigned to a rack.
        /// </summary>
        public string? Rack { get; set; } = null;

        public MetadataResponseBrokerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        public MetadataResponseBrokerMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(NodeId);
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(Port);
            if (version >= ApiVersions.Version1)
            {
                if (Rack is null)
                {
                    if (version >= ApiVersions.Version9)
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
                    if (version >= ApiVersions.Version9)
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(numTaggedFields);
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is MetadataResponseBrokerMessage other && Equals(other);
        }

        public bool Equals(MetadataResponseBrokerMessage? other)
        {
            return true;
        }
    }

    public sealed class MetadataResponseBrokerCollection: HashSet<MetadataResponseBrokerMessage>
    {
        public MetadataResponseBrokerCollection()
        {
        }

        public MetadataResponseBrokerCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class MetadataResponseTopicMessage: Message, IEquatable<MetadataResponseTopicMessage>
    {
        /// <summary>
        /// The topic error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The topic id.
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// True if the topic is internal.
        /// </summary>
        public bool IsInternal { get; set; } = false;

        /// <summary>
        /// Each partition in the topic.
        /// </summary>
        public List<MetadataResponsePartitionMessage> Partitions { get; set; } = new ();

        /// <summary>
        /// 32-bit bitfield to represent authorized operations for this topic.
        /// </summary>
        public int TopicAuthorizedOperations { get; set; } = -2147483648;

        public MetadataResponseTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        public MetadataResponseTopicMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteShort(ErrorCode);
            if (Name is null)
            {
                if (version >= ApiVersions.Version12)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    throw new NullReferenceException();                }
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersions.Version9)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersions.Version10)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteBool(IsInternal);
            }
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Partitions.Count);
                foreach (var element in Partitions)
                {
                    element.Write(writer, version);
                }
            }
            if (version >= ApiVersions.Version8)
            {
                writer.WriteInt(TopicAuthorizedOperations);
            }
            else
            {
                if (TopicAuthorizedOperations != -2147483648)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default TopicAuthorizedOperations at version {version}");
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(numTaggedFields);
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is MetadataResponseTopicMessage other && Equals(other);
        }

        public bool Equals(MetadataResponseTopicMessage? other)
        {
            return true;
        }
    }

    public sealed class MetadataResponsePartitionMessage: Message, IEquatable<MetadataResponsePartitionMessage>
    {
        /// <summary>
        /// The partition error, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The ID of the leader broker.
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int LeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The set of all nodes that host this partition.
        /// </summary>
        public List<int> ReplicaNodes { get; set; } = new ();

        /// <summary>
        /// The set of nodes that are in sync with the leader for this partition.
        /// </summary>
        public List<int> IsrNodes { get; set; } = new ();

        /// <summary>
        /// The set of offline replicas of this partition.
        /// </summary>
        public List<int> OfflineReplicas { get; set; } = new ();

        public MetadataResponsePartitionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        public MetadataResponsePartitionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version12;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteShort(ErrorCode);
            writer.WriteInt(PartitionIndex);
            writer.WriteInt(LeaderId);
            if (version >= ApiVersions.Version7)
            {
                writer.WriteInt(LeaderEpoch);
            }
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(ReplicaNodes.Count + 1);
            }
            else
            {
                writer.WriteInt(ReplicaNodes.Count);
            }
            foreach (var element in ReplicaNodes)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(IsrNodes.Count + 1);
            }
            else
            {
                writer.WriteInt(IsrNodes.Count);
            }
            foreach (var element in IsrNodes)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersions.Version5)
            {
                if (version >= ApiVersions.Version9)
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version9)
            {
                writer.WriteVarUInt(numTaggedFields);
                rawWriter.WriteRawTags(writer, int.MaxValue);
            }
            else
            {
                if (numTaggedFields > 0)
                {
                    throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is MetadataResponsePartitionMessage other && Equals(other);
        }

        public bool Equals(MetadataResponsePartitionMessage? other)
        {
            return true;
        }
    }

    public sealed class MetadataResponseTopicCollection: HashSet<MetadataResponseTopicMessage>
    {
        public MetadataResponseTopicCollection()
        {
        }

        public MetadataResponseTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
