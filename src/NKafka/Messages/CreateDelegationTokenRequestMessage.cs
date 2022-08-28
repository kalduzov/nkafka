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

public sealed class CreateDelegationTokenRequestMessage: RequestMessage, IEquatable<CreateDelegationTokenRequestMessage>
{
    /// <summary>
    /// The principal type of the owner of the token. If it's null it defaults to the token request principal.
    /// </summary>
    public string OwnerPrincipalType { get; set; } = string.Empty;

    /// <summary>
    /// The principal name of the owner of the token. If it's null it defaults to the token request principal.
    /// </summary>
    public string OwnerPrincipalName { get; set; } = string.Empty;

    /// <summary>
    /// A list of those who are allowed to renew this token before it expires.
    /// </summary>
    public List<CreatableRenewersMessage> Renewers { get; set; } = new ();

    /// <summary>
    /// The maximum lifetime of the token in milliseconds, or -1 to use the server side default.
    /// </summary>
    public long MaxLifetimeMs { get; set; } = 0;

    public CreateDelegationTokenRequestMessage()
    {
        ApiKey = ApiKeys.CreateDelegationToken;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public CreateDelegationTokenRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.CreateDelegationToken;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version3)
        {
            if (OwnerPrincipalType is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(OwnerPrincipalType);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (OwnerPrincipalType is null || !OwnerPrincipalType.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default OwnerPrincipalType at version {version}");
            }
        }
        if (version >= ApiVersions.Version3)
        {
            if (OwnerPrincipalName is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(OwnerPrincipalName);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
        }
        else
        {
            if (OwnerPrincipalName is null || !OwnerPrincipalName.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default OwnerPrincipalName at version {version}");
            }
        }
        if (version >= ApiVersions.Version2)
        {
            writer.WriteVarUInt(Renewers.Count + 1);
            foreach (var element in Renewers)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Renewers.Count);
            foreach (var element in Renewers)
            {
                element.Write(writer, version);
            }
        }
        writer.WriteLong(MaxLifetimeMs);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version2)
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
        return ReferenceEquals(this, obj) || obj is CreateDelegationTokenRequestMessage other && Equals(other);
    }

    public bool Equals(CreateDelegationTokenRequestMessage? other)
    {
        return true;
    }

    public sealed class CreatableRenewersMessage: Message, IEquatable<CreatableRenewersMessage>
    {
        /// <summary>
        /// The type of the Kafka principal.
        /// </summary>
        public string PrincipalType { get; set; } = string.Empty;

        /// <summary>
        /// The name of the Kafka principal.
        /// </summary>
        public string PrincipalName { get; set; } = string.Empty;

        public CreatableRenewersMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public CreatableRenewersMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(PrincipalType);
                if (version >= ApiVersions.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            {
                var stringBytes = Encoding.UTF8.GetBytes(PrincipalName);
                if (version >= ApiVersions.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version2)
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
            return ReferenceEquals(this, obj) || obj is CreatableRenewersMessage other && Equals(other);
        }

        public bool Equals(CreatableRenewersMessage? other)
        {
            return true;
        }
    }
}
