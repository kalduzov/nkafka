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

public sealed class DeleteTopicsResponseMessage: IResponseMessage, IEquatable<DeleteTopicsResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version6;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The results for each topic we tried to delete.
    /// </summary>
    public DeletableTopicResultCollection Responses { get; set; } = new ();

    public DeleteTopicsResponseMessage()
    {
    }

    public DeleteTopicsResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version >= ApiVersions.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        {
            if (version >= ApiVersions.Version4)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Responses was serialized as null");
                }
                else
                {
                    DeletableTopicResultCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new DeletableTopicResultMessage(reader, version));
                    }
                    Responses = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Responses was serialized as null");
                }
                else
                {
                    DeletableTopicResultCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new DeletableTopicResultMessage(reader, version));
                    }
                    Responses = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version4)
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
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersions.Version4)
        {
            writer.WriteVarUInt(Responses.Count + 1);
            foreach (var element in Responses)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Responses.Count);
            foreach (var element in Responses)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version4)
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
        return ReferenceEquals(this, obj) || obj is DeleteTopicsResponseMessage other && Equals(other);
    }

    public bool Equals(DeleteTopicsResponseMessage? other)
    {
        return true;
    }

    public sealed class DeletableTopicResultMessage: IMessage, IEquatable<DeletableTopicResultMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version6;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// the unique topic ID
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The deletion error, or 0 if the deletion succeeded.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The error message, or null if there was no error.
        /// </summary>
        public string? ErrorMessage { get; set; } = null;

        public DeletableTopicResultMessage()
        {
        }

        public DeletableTopicResultMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version6)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DeletableTopicResultMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version4)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    if (version >= ApiVersions.Version6)
                    {
                        Name = null;
                    }
                    else
                    {
                        throw new Exception("non-nullable field Name was serialized as null");
                    }
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
            if (version >= ApiVersions.Version6)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            ErrorCode = reader.ReadShort();
            if (version >= ApiVersions.Version5)
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    ErrorMessage = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field ErrorMessage had invalid length {length}");
                }
                else
                {
                    ErrorMessage = reader.ReadString(length);
                }
            }
            else
            {
                ErrorMessage = null;
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version4)
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
            if (Name is null)
            {
                if (version >= ApiVersions.Version6)
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
                if (version >= ApiVersions.Version4)
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
                writer.WriteGuid(TopicId);
            }
            writer.WriteShort((short)ErrorCode);
            if (version >= ApiVersions.Version5)
            {
                if (ErrorMessage is null)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
                    writer.WriteVarUInt(stringBytes.Length + 1);
                    writer.WriteBytes(stringBytes);
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version4)
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
            return ReferenceEquals(this, obj) || obj is DeletableTopicResultMessage other && Equals(other);
        }

        public bool Equals(DeletableTopicResultMessage? other)
        {
            return true;
        }
    }

    public sealed class DeletableTopicResultCollection: HashSet<DeletableTopicResultMessage>
    {
        public DeletableTopicResultCollection()
        {
        }

        public DeletableTopicResultCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
