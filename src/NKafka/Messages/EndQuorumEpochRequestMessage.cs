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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class EndQuorumEpochRequestMessage: IRequestMessage, IEquatable<EndQuorumEpochRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiKeys ApiKey => ApiKeys.EndQuorumEpoch;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public string? ClusterId { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public List<TopicDataMessage> Topics { get; set; } = new ();

    public EndQuorumEpochRequestMessage()
    {
    }

    public EndQuorumEpochRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            int length;
            length = reader.ReadShort();
            if (length < 0)
            {
                ClusterId = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ClusterId had invalid length {length}");
            }
            else
            {
                ClusterId = reader.ReadString(length);
            }
        }
        {
            int arrayLength;
            arrayLength = reader.ReadInt();
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Topics was serialized as null");
            }
            else
            {
                var newCollection = new List<TopicDataMessage>(arrayLength);
                for (var i = 0; i< arrayLength; i++)
                {
                    newCollection.Add(new TopicDataMessage(reader, version));
                }
                Topics = newCollection;
            }
        }
        UnknownTaggedFields = null;
    }

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (ClusterId is null)
        {
            writer.WriteShort(-1);
        }
        else
        {
            var stringBytes = Encoding.UTF8.GetBytes(ClusterId);
            writer.WriteShort((short)stringBytes.Length);
            writer.WriteBytes(stringBytes);
        }
        writer.WriteInt(Topics.Count);
        foreach (var element in Topics)
        {
            element.Write(writer, version);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (numTaggedFields > 0)
        {
            throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
        }
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is EndQuorumEpochRequestMessage other && Equals(other);
    }

    public bool Equals(EndQuorumEpochRequestMessage? other)
    {
        return true;
    }

    public sealed class TopicDataMessage: IMessage, IEquatable<TopicDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public List<PartitionDataMessage> Partitions { get; set; } = new ();

        public TopicDataMessage()
        {
        }

        public TopicDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of TopicDataMessage");
            }
            {
                int length;
                length = reader.ReadShort();
                if (length < 0)
                {
                    throw new Exception("non-nullable field TopicName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TopicName had invalid length {length}");
                }
                else
                {
                    TopicName = reader.ReadString(length);
                }
            }
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<PartitionDataMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new PartitionDataMessage(reader, version));
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(Partitions.Count);
            foreach (var element in Partitions)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is TopicDataMessage other && Equals(other);
        }

        public bool Equals(TopicDataMessage? other)
        {
            return true;
        }
    }

    public sealed class PartitionDataMessage: IMessage, IEquatable<PartitionDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The current leader ID that is resigning
        /// </summary>
        public int LeaderId { get; set; } = 0;

        /// <summary>
        /// The current epoch
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// A sorted list of preferred successors to start the election
        /// </summary>
        public List<int> PreferredSuccessors { get; set; } = new ();

        public PartitionDataMessage()
        {
        }

        public PartitionDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of PartitionDataMessage");
            }
            PartitionIndex = reader.ReadInt();
            LeaderId = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field PreferredSuccessors was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    PreferredSuccessors = newCollection;
                }
            }
            UnknownTaggedFields = null;
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteInt(LeaderId);
            writer.WriteInt(LeaderEpoch);
            writer.WriteInt(PreferredSuccessors.Count);
            foreach (var element in PreferredSuccessors)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (numTaggedFields > 0)
            {
                throw new UnsupportedVersionException($"Tagged fields were set, but version {version} of this message does not support them.");
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is PartitionDataMessage other && Equals(other);
        }

        public bool Equals(PartitionDataMessage? other)
        {
            return true;
        }
    }
}
