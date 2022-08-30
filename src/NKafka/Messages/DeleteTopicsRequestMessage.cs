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

public sealed class DeleteTopicsRequestMessage: IRequestMessage, IEquatable<DeleteTopicsRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version6;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.DeleteTopics;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The name or topic ID of the topic
    /// </summary>
    public List<DeleteTopicStateMessage> Topics { get; set; } = new ();

    /// <summary>
    /// The names of the topics to delete
    /// </summary>
    public List<string> TopicNames { get; set; } = new ();

    /// <summary>
    /// The length of time in milliseconds to wait for the deletions to complete.
    /// </summary>
    public int TimeoutMs { get; set; } = 0;

    public DeleteTopicsRequestMessage()
    {
    }

    public DeleteTopicsRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version6)
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Topics was serialized as null");
            }
            else
            {
                var newCollection = new List<DeleteTopicStateMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new DeleteTopicStateMessage(reader, version));
                }
                Topics = newCollection;
            }
        }
        else
        {
            Topics = new ();
        }
        if (version <= ApiVersion.Version5)
        {
            if (version >= ApiVersion.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicNames was serialized as null");
                }
                else
                {
                    var newCollection = new List<string>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        int length;
                        length = reader.ReadVarUInt() - 1;
                        if (length < 0)
                        {
                            throw new Exception("non-nullable field TopicNames element was serialized as null");
                        }
                        else if (length > 0x7fff)
                        {
                            throw new Exception($"string field TopicNames element had invalid length {length}");
                        }
                        else
                        {
                            newCollection.Add(reader.ReadString(length));
                        }
                    }
                    TopicNames = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicNames was serialized as null");
                }
                else
                {
                    var newCollection = new List<string>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        int length;
                        length = reader.ReadShort();
                        if (length < 0)
                        {
                            throw new Exception("non-nullable field TopicNames element was serialized as null");
                        }
                        else if (length > 0x7fff)
                        {
                            throw new Exception($"string field TopicNames element had invalid length {length}");
                        }
                        else
                        {
                            newCollection.Add(reader.ReadString(length));
                        }
                    }
                    TopicNames = newCollection;
                }
            }
        }
        else
        {
            TopicNames = new ();
        }
        TimeoutMs = reader.ReadInt();
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version4)
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersion.Version6)
        {
            writer.WriteVarUInt(Topics.Count + 1);
            foreach (var element in Topics)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (Topics.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Topics at version {version}");
            }
        }
        if (version <= ApiVersion.Version5)
        {
            if (version >= ApiVersion.Version4)
            {
                writer.WriteVarUInt(TopicNames.Count + 1);
                foreach (var element in TopicNames)
                {
                    {
                        var stringBytes = Encoding.UTF8.GetBytes(element);
                        writer.WriteVarUInt(stringBytes.Length + 1);
                        writer.WriteBytes(stringBytes);
                    }
                }
            }
            else
            {
                writer.WriteInt(TopicNames.Count);
                foreach (var element in TopicNames)
                {
                    {
                        var stringBytes = Encoding.UTF8.GetBytes(element);
                        writer.WriteShort((short)stringBytes.Length);
                        writer.WriteBytes(stringBytes);
                    }
                }
            }
        }
        writer.WriteInt(TimeoutMs);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version4)
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
        return ReferenceEquals(this, obj) || obj is DeleteTopicsRequestMessage other && Equals(other);
    }

    public bool Equals(DeleteTopicsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Topics, TopicNames, TimeoutMs);
        return hashCode;
    }

    public override string ToString()
    {
        return "DeleteTopicsRequestMessage("
            + ", TimeoutMs=" + TimeoutMs
            + ")";
    }

    public sealed class DeleteTopicStateMessage: IMessage, IEquatable<DeleteTopicStateMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version6;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name
        /// </summary>
        public string? Name { get; set; } = null;

        /// <summary>
        /// The unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        public DeleteTopicStateMessage()
        {
        }

        public DeleteTopicStateMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version6)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DeleteTopicStateMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    Name = null;
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
            TopicId = reader.ReadGuid();
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            if (version < ApiVersion.Version6)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of DeleteTopicStateMessage");
            }
            var numTaggedFields = 0;
            if (Name is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteGuid(TopicId);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is DeleteTopicStateMessage other && Equals(other);
        }

        public bool Equals(DeleteTopicStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name, TopicId);
            return hashCode;
        }

        public override string ToString()
        {
            return "DeleteTopicStateMessage("
                + ", TopicId=" + TopicId
                + ")";
        }
    }
}
