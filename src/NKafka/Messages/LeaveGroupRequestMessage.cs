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

public sealed class LeaveGroupRequestMessage: RequestMessage, IEquatable<LeaveGroupRequestMessage>
{
    /// <summary>
    /// The ID of the group to leave.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// The member ID to remove from the group.
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// List of leaving member identities.
    /// </summary>
    public List<MemberIdentityMessage> Members { get; set; } = new ();

    public LeaveGroupRequestMessage()
    {
        ApiKey = ApiKeys.LeaveGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public LeaveGroupRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.LeaveGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        {
            var stringBytes = Encoding.UTF8.GetBytes(GroupId);
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
        if (version <= ApiVersions.Version2)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
                writer.WriteShort((short)stringBytes.Length);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (!MemberId.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default MemberId at version {version}");
            }
        }
        if (version >= ApiVersions.Version3)
        {
            if (version >= ApiVersions.Version4)
            {
                writer.WriteVarUInt(Members.Count + 1);
                foreach (var element in Members)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Members.Count);
                foreach (var element in Members)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (Members.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Members at version {version}");
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
        return ReferenceEquals(this, obj) || obj is LeaveGroupRequestMessage other && Equals(other);
    }

    public bool Equals(LeaveGroupRequestMessage? other)
    {
        return true;
    }

    public sealed class MemberIdentityMessage: Message, IEquatable<MemberIdentityMessage>
    {
        /// <summary>
        /// The member ID to remove from the group.
        /// </summary>
        public string MemberId { get; set; } = string.Empty;

        /// <summary>
        /// The group instance ID to remove from the group.
        /// </summary>
        public string? GroupInstanceId { get; set; } = null;

        /// <summary>
        /// The reason why the member left the group.
        /// </summary>
        public string? Reason { get; set; } = null;

        public MemberIdentityMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        public MemberIdentityMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version5;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of MemberIdentityMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(MemberId);
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
            if (GroupInstanceId is null)
            {
                if (version >= ApiVersions.Version4)
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
            if (version >= ApiVersions.Version5)
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
            return ReferenceEquals(this, obj) || obj is MemberIdentityMessage other && Equals(other);
        }

        public bool Equals(MemberIdentityMessage? other)
        {
            return true;
        }
    }
}
