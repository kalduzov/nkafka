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

public sealed class JoinGroupRequestMessage: RequestMessage, IEquatable<JoinGroupRequestMessage>
{
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
        ApiKey = ApiKeys.JoinGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    public JoinGroupRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.JoinGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version9;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        {
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
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
        writer.WriteInt(SessionTimeoutMs);
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(RebalanceTimeoutMs);
        }
        {
            var stringBytes = Encoding.UTF8.GetBytes(MemberId);
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
        if (version >= ApiVersions.Version5)
        {
            if (GroupInstanceId is null)
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
                var stringBytes = Encoding.UTF8.GetBytes(GroupInstanceId);
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
        else
        {
            if (GroupInstanceId is not null)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default GroupInstanceId at version {version}");
            }
        }
        {
            var stringBytes = Encoding.UTF8.GetBytes(ProtocolType);
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
        if (version >= ApiVersions.Version8)
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
        return ReferenceEquals(this, obj) || obj is JoinGroupRequestMessage other && Equals(other);
    }

    public bool Equals(JoinGroupRequestMessage? other)
    {
        return true;
    }

    public sealed class JoinGroupRequestProtocolMessage: Message, IEquatable<JoinGroupRequestProtocolMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        public JoinGroupRequestProtocolMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version9;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
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
                writer.WriteVarUInt(Metadata.Length + 1);
            }
            else
            {
                writer.WriteInt(Metadata.Length);
            }
            writer.WriteBytes(Metadata);
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
            return ReferenceEquals(this, obj) || obj is JoinGroupRequestProtocolMessage other && Equals(other);
        }

        public bool Equals(JoinGroupRequestProtocolMessage? other)
        {
            return true;
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
