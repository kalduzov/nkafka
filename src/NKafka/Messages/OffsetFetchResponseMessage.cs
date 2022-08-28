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

public sealed class OffsetFetchResponseMessage: IResponseMessage, IEquatable<OffsetFetchResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The responses per topic.
    /// </summary>
    public List<OffsetFetchResponseTopicMessage> Topics { get; set; } = new ();

    /// <summary>
    /// The top-level error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The responses per group id.
    /// </summary>
    public List<OffsetFetchResponseGroupMessage> Groups { get; set; } = new ();

    public OffsetFetchResponseMessage()
    {
    }

    public OffsetFetchResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version >= ApiVersions.Version3)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        if (version <= ApiVersions.Version7)
        {
            if (version >= ApiVersions.Version6)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new List<OffsetFetchResponseTopicMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchResponseTopicMessage(reader, version));
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
                    var newCollection = new List<OffsetFetchResponseTopicMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchResponseTopicMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
        }
        else
        {
            Topics = new ();
        }
        if (version >= ApiVersions.Version2 && version <= ApiVersions.Version7)
        {
            ErrorCode = reader.ReadShort();
        }
        else
        {
            ErrorCode = 0;
        }
        if (version >= ApiVersions.Version8)
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Groups was serialized as null");
            }
            else
            {
                var newCollection = new List<OffsetFetchResponseGroupMessage>(arrayLength);
                for (var i = 0; i< arrayLength; i++)
                {
                    newCollection.Add(new OffsetFetchResponseGroupMessage(reader, version));
                }
                Groups = newCollection;
            }
        }
        else
        {
            Groups = new ();
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version6)
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
        if (version >= ApiVersions.Version3)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version <= ApiVersions.Version7)
        {
            if (version >= ApiVersions.Version6)
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
        }
        else
        {
            if (Topics.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Topics at version {version}");
            }
        }
        if (version >= ApiVersions.Version2 && version <= ApiVersions.Version7)
        {
            writer.WriteShort((short)ErrorCode);
        }
        if (version >= ApiVersions.Version8)
        {
            writer.WriteVarUInt(Groups.Count + 1);
            foreach (var element in Groups)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (Groups.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Groups at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version6)
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
        return ReferenceEquals(this, obj) || obj is OffsetFetchResponseMessage other && Equals(other);
    }

    public bool Equals(OffsetFetchResponseMessage? other)
    {
        return true;
    }

    public sealed class OffsetFetchResponseTopicMessage: IMessage, IEquatable<OffsetFetchResponseTopicMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The responses per partition
        /// </summary>
        public List<OffsetFetchResponsePartitionMessage> Partitions { get; set; } = new ();

        public OffsetFetchResponseTopicMessage()
        {
        }

        public OffsetFetchResponseTopicMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
            {
                if (version >= ApiVersions.Version6)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Partitions was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<OffsetFetchResponsePartitionMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new OffsetFetchResponsePartitionMessage(reader, version));
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
                        var newCollection = new List<OffsetFetchResponsePartitionMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new OffsetFetchResponsePartitionMessage(reader, version));
                        }
                        Partitions = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
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
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of OffsetFetchResponseTopicMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            if (version >= ApiVersions.Version6)
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
            if (version >= ApiVersions.Version6)
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
            return ReferenceEquals(this, obj) || obj is OffsetFetchResponseTopicMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchResponseTopicMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetFetchResponsePartitionMessage: IMessage, IEquatable<OffsetFetchResponsePartitionMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The committed message offset.
        /// </summary>
        public long CommittedOffset { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int CommittedLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The partition metadata.
        /// </summary>
        public string Metadata { get; set; } = string.Empty;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        public OffsetFetchResponsePartitionMessage()
        {
        }

        public OffsetFetchResponsePartitionMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            PartitionIndex = reader.ReadInt();
            CommittedOffset = reader.ReadLong();
            if (version >= ApiVersions.Version5)
            {
                CommittedLeaderEpoch = reader.ReadInt();
            }
            else
            {
                CommittedLeaderEpoch = -1;
            }
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    Metadata = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Metadata had invalid length {length}");
                }
                else
                {
                    Metadata = reader.ReadString(length);
                }
            }
            ErrorCode = reader.ReadShort();
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
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
            writer.WriteInt(PartitionIndex);
            writer.WriteLong(CommittedOffset);
            if (version >= ApiVersions.Version5)
            {
                writer.WriteInt(CommittedLeaderEpoch);
            }
            if (Metadata is null)
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
                var stringBytes = Encoding.UTF8.GetBytes(Metadata);
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
            writer.WriteShort((short)ErrorCode);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version6)
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
            return ReferenceEquals(this, obj) || obj is OffsetFetchResponsePartitionMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchResponsePartitionMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetFetchResponseGroupMessage: IMessage, IEquatable<OffsetFetchResponseGroupMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The group ID.
        /// </summary>
        public string groupId { get; set; } = string.Empty;

        /// <summary>
        /// The responses per topic.
        /// </summary>
        public List<OffsetFetchResponseTopicsMessage> Topics { get; set; } = new ();

        /// <summary>
        /// The group-level error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        public OffsetFetchResponseGroupMessage()
        {
        }

        public OffsetFetchResponseGroupMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetFetchResponseGroupMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field groupId was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field groupId had invalid length {length}");
                }
                else
                {
                    groupId = reader.ReadString(length);
                }
            }
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Topics was serialized as null");
                }
                else
                {
                    var newCollection = new List<OffsetFetchResponseTopicsMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchResponseTopicsMessage(reader, version));
                    }
                    Topics = newCollection;
                }
            }
            ErrorCode = reader.ReadShort();
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of OffsetFetchResponseGroupMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(groupId);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
            writer.WriteShort((short)ErrorCode);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetFetchResponseGroupMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchResponseGroupMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetFetchResponseTopicsMessage: IMessage, IEquatable<OffsetFetchResponseTopicsMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version8;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The responses per partition
        /// </summary>
        public List<OffsetFetchResponsePartitionsMessage> Partitions { get; set; } = new ();

        public OffsetFetchResponseTopicsMessage()
        {
        }

        public OffsetFetchResponseTopicsMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetFetchResponseTopicsMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<OffsetFetchResponsePartitionsMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new OffsetFetchResponsePartitionsMessage(reader, version));
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteVarUInt(Partitions.Count + 1);
            foreach (var element in Partitions)
            {
                element.Write(writer, version);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetFetchResponseTopicsMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchResponseTopicsMessage? other)
        {
            return true;
        }
    }

    public sealed class OffsetFetchResponsePartitionsMessage: IMessage, IEquatable<OffsetFetchResponsePartitionsMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version8;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version8;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The committed message offset.
        /// </summary>
        public long CommittedOffset { get; set; } = 0;

        /// <summary>
        /// The leader epoch.
        /// </summary>
        public int CommittedLeaderEpoch { get; set; } = -1;

        /// <summary>
        /// The partition metadata.
        /// </summary>
        public string Metadata { get; set; } = string.Empty;

        /// <summary>
        /// The partition-level error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        public OffsetFetchResponsePartitionsMessage()
        {
        }

        public OffsetFetchResponsePartitionsMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version8)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of OffsetFetchResponsePartitionsMessage");
            }
            PartitionIndex = reader.ReadInt();
            CommittedOffset = reader.ReadLong();
            CommittedLeaderEpoch = reader.ReadInt();
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    Metadata = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Metadata had invalid length {length}");
                }
                else
                {
                    Metadata = reader.ReadString(length);
                }
            }
            ErrorCode = reader.ReadShort();
            UnknownTaggedFields = null;
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

        public void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteLong(CommittedOffset);
            writer.WriteInt(CommittedLeaderEpoch);
            if (Metadata is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Metadata);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteShort((short)ErrorCode);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is OffsetFetchResponsePartitionsMessage other && Equals(other);
        }

        public bool Equals(OffsetFetchResponsePartitionsMessage? other)
        {
            return true;
        }
    }
}
