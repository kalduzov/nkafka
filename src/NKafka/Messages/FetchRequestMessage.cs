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

public sealed class FetchRequestMessage: IRequestMessage, IEquatable<FetchRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version13;

    public ApiKeys ApiKey => ApiKeys.Fetch;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The clusterId if known. This is used to validate metadata fetches prior to broker registration.
    /// </summary>
    public string? ClusterId { get; set; } = null;

    /// <summary>
    /// The broker ID of the follower, of -1 if this request is from a consumer.
    /// </summary>
    public int ReplicaId { get; set; } = 0;

    /// <summary>
    /// The maximum time in milliseconds to wait for the response.
    /// </summary>
    public int MaxWaitMs { get; set; } = 0;

    /// <summary>
    /// The minimum bytes to accumulate in the response.
    /// </summary>
    public int MinBytes { get; set; } = 0;

    /// <summary>
    /// The maximum bytes to fetch.  See KIP-74 for cases where this limit may not be honored.
    /// </summary>
    public int MaxBytes { get; set; } = 2147483647;

    /// <summary>
    /// This setting controls the visibility of transactional records. Using READ_UNCOMMITTED (isolation_level = 0) makes all records visible. With READ_COMMITTED (isolation_level = 1), non-transactional and COMMITTED transactional records are visible. To be more concrete, READ_COMMITTED returns all data from offsets smaller than the current LSO (last stable offset), and enables the inclusion of the list of aborted transactions in the result, which allows consumers to discard ABORTED transactional records
    /// </summary>
    public sbyte IsolationLevel { get; set; } = 0;

    /// <summary>
    /// The fetch session ID.
    /// </summary>
    public int SessionId { get; set; } = 0;

    /// <summary>
    /// The fetch session epoch, which is used for ordering requests in a session.
    /// </summary>
    public int SessionEpoch { get; set; } = -1;

    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<FetchTopicMessage> Topics { get; set; } = new ();

    /// <summary>
    /// In an incremental fetch request, the partitions to remove.
    /// </summary>
    public List<ForgottenTopicMessage> ForgottenTopicsData { get; set; } = new ();

    /// <summary>
    /// Rack ID of the consumer making this request
    /// </summary>
    public string RackId { get; set; } = string.Empty;

    public FetchRequestMessage()
    {
    }

    public FetchRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            ClusterId = null;
        }
        ReplicaId = reader.ReadInt();
        MaxWaitMs = reader.ReadInt();
        MinBytes = reader.ReadInt();
        if (version >= ApiVersions.Version3)
        {
            MaxBytes = reader.ReadInt();
        }
        else
        {
            MaxBytes = 2147483647;
        }
        if (version >= ApiVersions.Version4)
        {
            IsolationLevel = reader.ReadSByte();
        }
        else
        {
            IsolationLevel = 0;
        }
        if (version >= ApiVersions.Version7)
        {
            SessionId = reader.ReadInt();
        }
        else
        {
            SessionId = 0;
        }
        if (version >= ApiVersions.Version7)
        {
            SessionEpoch = reader.ReadInt();
        }
        else
        {
            SessionEpoch = -1;
        }
        {
            if (version >= ApiVersions.Version12)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new List<FetchTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new FetchTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new List<FetchTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new FetchTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        if (version >= ApiVersions.Version7)
        {
            if (version >= ApiVersions.Version12)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field ForgottenTopicsData was serialized as null");
                }
                else
                {
                    var newCollection = new List<ForgottenTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ForgottenTopicMessage(reader, version));
                    }
                    ForgottenTopicsData = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field ForgottenTopicsData was serialized as null");
                }
                else
                {
                    var newCollection = new List<ForgottenTopicMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ForgottenTopicMessage(reader, version));
                    }
                    ForgottenTopicsData = newCollection;
                }
            }
        }
        else
        {
            ForgottenTopicsData = new ();
        }
        if (version >= ApiVersions.Version11)
        {
            int length;
            if (version >= ApiVersions.Version12)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field RackId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field RackId had invalid length {length}");
            }
            else
            {
                RackId = reader.ReadString(length);
            }
        }
        else
        {
            RackId = string.Empty;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version12)
        {
            var numTaggedFields = reader.ReadVarUInt();
            for (var t = 0; t < numTaggedFields; t++)
            {
                var tag = reader.ReadVarUInt();
                var size = reader.ReadVarUInt();
                switch (tag)
                {
                    case 0:
                    {
                        int length;
                        length = reader.ReadVarUInt() - 1;
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
                        break;
                    }
                    default:
                        UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                        break;
                }
            }
        }
    }

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version12)
        {
            if (ClusterId is not null)
            {
                numTaggedFields++;
            }
        }
        writer.WriteInt(ReplicaId);
        writer.WriteInt(MaxWaitMs);
        writer.WriteInt(MinBytes);
        if (version >= ApiVersions.Version3)
        {
            writer.WriteInt(MaxBytes);
        }
        if (version >= ApiVersions.Version4)
        {
            writer.WriteSByte(IsolationLevel);
        }
        if (version >= ApiVersions.Version7)
        {
            writer.WriteInt(SessionId);
        }
        if (version >= ApiVersions.Version7)
        {
            writer.WriteInt(SessionEpoch);
        }
        if (version >= ApiVersions.Version12)
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
        if (version >= ApiVersions.Version7)
        {
            if (version >= ApiVersions.Version12)
            {
                writer.WriteVarUInt(ForgottenTopicsData.Count + 1);
                foreach (var element in ForgottenTopicsData)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(ForgottenTopicsData.Count);
                foreach (var element in ForgottenTopicsData)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (ForgottenTopicsData.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ForgottenTopicsData at version {version}");
            }
        }
        if (version >= ApiVersions.Version11)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(RackId);
                if (version >= ApiVersions.Version12)
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
        if (version >= ApiVersions.Version12)
        {
            writer.WriteVarUInt(numTaggedFields);
            if (ClusterId is not null)
            {
                writer.WriteVarUInt(0);
                var stringBytes = Encoding.UTF8.GetBytes(ClusterId);
                writer.WriteVarUInt(stringBytes.Length + (stringBytes.Length + 1).SizeOfVarUInt());
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
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
        return ReferenceEquals(this, obj) || obj is FetchRequestMessage other && Equals(other);
    }

    public bool Equals(FetchRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ClusterId, ReplicaId, MaxWaitMs, MinBytes, MaxBytes, IsolationLevel, SessionId);
        hashCode = HashCode.Combine(hashCode, SessionEpoch, Topics, ForgottenTopicsData, RackId);
        return hashCode;
    }

    public override string ToString()
    {
        return "FetchRequestMessage("
            + ", ReplicaId=" + ReplicaId
            + ", MaxWaitMs=" + MaxWaitMs
            + ", MinBytes=" + MinBytes
            + ", MaxBytes=" + MaxBytes
            + ", IsolationLevel=" + IsolationLevel
            + ", SessionId=" + SessionId
            + ", SessionEpoch=" + SessionEpoch
            + ")";
    }

    public sealed class FetchTopicMessage: IMessage, IEquatable<FetchTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version13;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the topic to fetch.
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partitions to fetch.
        /// </summary>
        public List<FetchPartitionMessage> Partitions { get; set; } = new ();

        public FetchTopicMessage()
        {
        }

        public FetchTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of FetchTopicMessage");
            }
            if (version <= ApiVersions.Version12)
            {
                int length;
                if (version >= ApiVersions.Version12)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Topic was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Topic had invalid length {length}");
                }
                else
                {
                    Topic = reader.ReadString(length);
                }
            }
            else
            {
                Topic = string.Empty;
            }
            if (version >= ApiVersions.Version13)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                if (version >= ApiVersions.Version12)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<FetchPartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new FetchPartitionMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<FetchPartitionMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new FetchPartitionMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version12)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            if (version <= ApiVersions.Version12)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Topic);
                    if (version >= ApiVersions.Version12)
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
            if (version >= ApiVersions.Version13)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version12)
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version12)
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
            return ReferenceEquals(this, obj) || obj is FetchTopicMessage other && Equals(other);
        }

        public bool Equals(FetchTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Topic, TopicId, Partitions);
            return hashCode;
        }

        public override string ToString()
        {
            return "FetchTopicMessage("
                + ", TopicId=" + TopicId
                + ")";
        }
    }

    public sealed class FetchPartitionMessage: IMessage, IEquatable<FetchPartitionMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version13;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int Partition { get; set; } = 0;

        /// <summary>
        /// The current leader epoch of the partition.
        /// </summary>
        public int CurrentLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The message offset.
        /// </summary>
        public long FetchOffset { get; set; } = 0;

        /// <summary>
        /// The epoch of the last fetched record or -1 if there is none
        /// </summary>
        public int LastFetchedEpoch { get; set; } = -1;

        /// <summary>
        /// The earliest available offset of the follower replica.  The field is only used when the request is sent by the follower.
        /// </summary>
        public long LogStartOffset { get; set; } = -1;

        /// <summary>
        /// The maximum bytes to fetch from this partition.  See KIP-74 for cases where this limit may not be honored.
        /// </summary>
        public int PartitionMaxBytes { get; set; } = 0;

        public FetchPartitionMessage()
        {
        }

        public FetchPartitionMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of FetchPartitionMessage");
            }
            Partition = reader.ReadInt();
            if (version >= ApiVersions.Version9)
            {
                CurrentLeaderEpoch = reader.ReadInt();
            }
            else
            {
                CurrentLeaderEpoch = -1;
            }
            FetchOffset = reader.ReadLong();
            if (version >= ApiVersions.Version12)
            {
                LastFetchedEpoch = reader.ReadInt();
            }
            else
            {
                LastFetchedEpoch = -1;
            }
            if (version >= ApiVersions.Version5)
            {
                LogStartOffset = reader.ReadLong();
            }
            else
            {
                LogStartOffset = -1;
            }
            PartitionMaxBytes = reader.ReadInt();
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version12)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(Partition);
            if (version >= ApiVersions.Version9)
            {
                writer.WriteInt(CurrentLeaderEpoch);
            }
            writer.WriteLong(FetchOffset);
            if (version >= ApiVersions.Version12)
            {
                writer.WriteInt(LastFetchedEpoch);
            }
            else
            {
                if (LastFetchedEpoch != -1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default LastFetchedEpoch at version {version}");
                }
            }
            if (version >= ApiVersions.Version5)
            {
                writer.WriteLong(LogStartOffset);
            }
            writer.WriteInt(PartitionMaxBytes);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version12)
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
            return ReferenceEquals(this, obj) || obj is FetchPartitionMessage other && Equals(other);
        }

        public bool Equals(FetchPartitionMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Partition, CurrentLeaderEpoch, FetchOffset, LastFetchedEpoch, LogStartOffset, PartitionMaxBytes);
            return hashCode;
        }

        public override string ToString()
        {
            return "FetchPartitionMessage("
                + "Partition=" + Partition
                + ", CurrentLeaderEpoch=" + CurrentLeaderEpoch
                + ", FetchOffset=" + FetchOffset
                + ", LastFetchedEpoch=" + LastFetchedEpoch
                + ", LogStartOffset=" + LogStartOffset
                + ", PartitionMaxBytes=" + PartitionMaxBytes
                + ")";
        }
    }

    public sealed class ForgottenTopicMessage: IMessage, IEquatable<ForgottenTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version13;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partitions indexes to forget.
        /// </summary>
        public List<int> Partitions { get; set; } = new ();

        public ForgottenTopicMessage()
        {
        }

        public ForgottenTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version13)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of ForgottenTopicMessage");
            }
            if (version <= ApiVersions.Version12)
            {
                int length;
                if (version >= ApiVersions.Version12)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Topic was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Topic had invalid length {length}");
                }
                else
                {
                    Topic = reader.ReadString(length);
                }
            }
            else
            {
                Topic = string.Empty;
            }
            if (version >= ApiVersions.Version13)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                int arrayLength;
                if (version >= ApiVersions.Version12)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version12)
            {
                var numTaggedFields = reader.ReadVarUInt();
                for (var t = 0; t < numTaggedFields; t++)
                {
                    var tag = reader.ReadVarUInt();
                    var size = reader.ReadVarUInt();
                    switch (tag)
                    {
                        default:
                            UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                            break;
                    }
                }
            }
        }

        public void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of ForgottenTopicMessage");
            }
            var numTaggedFields = 0;
            if (version <= ApiVersions.Version12)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Topic);
                    if (version >= ApiVersions.Version12)
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
            if (version >= ApiVersions.Version13)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version12)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
            }
            else
            {
                writer.WriteInt(Partitions.Count);
            }
            foreach (var element in Partitions)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version12)
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
            return ReferenceEquals(this, obj) || obj is ForgottenTopicMessage other && Equals(other);
        }

        public bool Equals(ForgottenTopicMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Topic, TopicId, Partitions);
            return hashCode;
        }

        public override string ToString()
        {
            return "ForgottenTopicMessage("
                + ", TopicId=" + TopicId
                + ")";
        }
    }
}
