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

public sealed class JoinGroupRequestMessage: IRequestMessage, IEquatable<JoinGroupRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version9;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.JoinGroup;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The group identifier.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// The coordinator considers the consumer dead if it receives no heartbeat after this timeout in milliseconds.
    /// </summary>
    public int SessionTimeoutMs { get; set; } = 0;

    /// <summary>
    /// The maximum time in milliseconds that the coordinator will wait for each member to rejoin when rebalancing the group.
    /// </summary>
    public int RebalanceTimeoutMs { get; set; } = -1;

    /// <summary>
    /// The member id assigned by the group coordinator.
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = null;

    /// <summary>
    /// The unique name the for class of protocols implemented by the group we want to join.
    /// </summary>
    public string ProtocolType { get; set; } = string.Empty;

    /// <summary>
    /// The list of protocols that the member supports.
    /// </summary>
    public JoinGroupRequestProtocolCollection Protocols { get; set; } = new ();

    /// <summary>
    /// The reason why the member (re-)joins the group.
    /// </summary>
    public string? Reason { get; set; } = null;

    public JoinGroupRequestMessage()
    {
    }

    public JoinGroupRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            int length;
            if (version >= ApiVersion.Version6)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field GroupId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field GroupId had invalid length {length}");
            }
            else
            {
                GroupId = reader.ReadString(length);
            }
        }
        SessionTimeoutMs = reader.ReadInt();
        if (version >= ApiVersion.Version1)
        {
            RebalanceTimeoutMs = reader.ReadInt();
        }
        else
        {
            RebalanceTimeoutMs = -1;
        }
        {
            int length;
            if (version >= ApiVersion.Version6)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field MemberId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field MemberId had invalid length {length}");
            }
            else
            {
                MemberId = reader.ReadString(length);
            }
        }
        if (version >= ApiVersion.Version5)
        {
            int length;
            if (version >= ApiVersion.Version6)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                GroupInstanceId = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field GroupInstanceId had invalid length {length}");
            }
            else
            {
                GroupInstanceId = reader.ReadString(length);
            }
        }
        else
        {
            GroupInstanceId = null;
        }
        {
            int length;
            if (version >= ApiVersion.Version6)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field ProtocolType was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ProtocolType had invalid length {length}");
            }
            else
            {
                ProtocolType = reader.ReadString(length);
            }
        }
        {
            if (version >= ApiVersion.Version6)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Protocols was serialized as null");
                }
                else
                {
                    var newCollection = new JoinGroupRequestProtocolCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new JoinGroupRequestProtocolMessage(reader, version));
                    }
                    Protocols = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Protocols was serialized as null");
                }
                else
                {
                    var newCollection = new JoinGroupRequestProtocolCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new JoinGroupRequestProtocolMessage(reader, version));
                    }
                    Protocols = newCollection;
                }
            }
        }
        if (version >= ApiVersion.Version8)
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                Reason = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field Reason had invalid length {length}");
            }
            else
            {
                Reason = reader.ReadString(length);
            }
        }
        else
        {
            Reason = null;
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version6)
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
        {
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
            if (version >= ApiVersion.Version6)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        writer.WriteInt(SessionTimeoutMs);
        if (version >= ApiVersion.Version1)
        {
            writer.WriteInt(RebalanceTimeoutMs);
        }
        {
            var stringBytes = Encoding.UTF8.GetBytes(MemberId);
            if (version >= ApiVersion.Version6)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersion.Version5)
        {
            if (GroupInstanceId is null)
            {
                if (version >= ApiVersion.Version6)
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
                var stringBytes = Encoding.UTF8.GetBytes(GroupInstanceId);
                if (version >= ApiVersion.Version6)
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
        else
        {
            if (GroupInstanceId is not null)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default GroupInstanceId at version {version}");
            }
        }
        {
            var stringBytes = Encoding.UTF8.GetBytes(ProtocolType);
            if (version >= ApiVersion.Version6)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersion.Version6)
        {
            writer.WriteVarUInt(Protocols.Count + 1);
            foreach (var element in Protocols)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Protocols.Count);
            foreach (var element in Protocols)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersion.Version8)
        {
            if (Reason is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Reason);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version6)
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
        return ReferenceEquals(this, obj) || obj is JoinGroupRequestMessage other && Equals(other);
    }

    public bool Equals(JoinGroupRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, GroupId, SessionTimeoutMs, RebalanceTimeoutMs, MemberId, GroupInstanceId, ProtocolType, Protocols);
        hashCode = HashCode.Combine(hashCode, Reason);
        return hashCode;
    }

    public override string ToString()
    {
        return "JoinGroupRequestMessage("
            + ", SessionTimeoutMs=" + SessionTimeoutMs
            + ", RebalanceTimeoutMs=" + RebalanceTimeoutMs
            + ")";
    }

    public sealed class JoinGroupRequestProtocolMessage: IMessage, IEquatable<JoinGroupRequestProtocolMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version9;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The protocol name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The protocol metadata.
        /// </summary>
        public byte[] Metadata { get; set; } = Array.Empty<byte>();

        public JoinGroupRequestProtocolMessage()
        {
        }

        public JoinGroupRequestProtocolMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version9)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of JoinGroupRequestProtocolMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version6)
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
                int length;
                if (version >= ApiVersion.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadInt();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Metadata was serialized as null");
                }
                else
                {
                    Metadata = reader.ReadBytes(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version6)
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                if (version >= ApiVersion.Version6)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version6)
            {
                writer.WriteVarUInt(Metadata.Length + 1);
            }
            else
            {
                writer.WriteInt(Metadata.Length);
            }
            writer.WriteBytes(Metadata);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version6)
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
            return ReferenceEquals(this, obj) || obj is JoinGroupRequestProtocolMessage other && Equals(other);
        }

        public bool Equals(JoinGroupRequestProtocolMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "JoinGroupRequestProtocolMessage("
                + ")";
        }
    }

    public sealed class JoinGroupRequestProtocolCollection: HashSet<JoinGroupRequestProtocolMessage>
    {
        public JoinGroupRequestProtocolCollection()
        {
        }

        public JoinGroupRequestProtocolCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
