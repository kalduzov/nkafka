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

public sealed class AlterUserScramCredentialsRequestMessage: RequestMessage, IEquatable<AlterUserScramCredentialsRequestMessage>
{
    /// <summary>
    /// The SCRAM credentials to remove.
    /// </summary>
    public List<ScramCredentialDeletionMessage> Deletions { get; set; } = new ();

    /// <summary>
    /// The SCRAM credentials to update/insert.
    /// </summary>
    public List<ScramCredentialUpsertionMessage> Upsertions { get; set; } = new ();

    public AlterUserScramCredentialsRequestMessage()
    {
        ApiKey = ApiKeys.AlterUserScramCredentials;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public AlterUserScramCredentialsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.AlterUserScramCredentials;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteVarUInt(Deletions.Count + 1);
        foreach (var element in Deletions)
        {
            element.Write(writer, version);
        }
        writer.WriteVarUInt(Upsertions.Count + 1);
        foreach (var element in Upsertions)
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
        return ReferenceEquals(this, obj) || obj is AlterUserScramCredentialsRequestMessage other && Equals(other);
    }

    public bool Equals(AlterUserScramCredentialsRequestMessage? other)
    {
        return true;
    }

    public sealed class ScramCredentialDeletionMessage: Message, IEquatable<ScramCredentialDeletionMessage>
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The SCRAM mechanism.
        /// </summary>
        public sbyte Mechanism { get; set; } = 0;

        public ScramCredentialDeletionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public ScramCredentialDeletionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteSByte(Mechanism);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is ScramCredentialDeletionMessage other && Equals(other);
        }

        public bool Equals(ScramCredentialDeletionMessage? other)
        {
            return true;
        }
    }

    public sealed class ScramCredentialUpsertionMessage: Message, IEquatable<ScramCredentialUpsertionMessage>
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The SCRAM mechanism.
        /// </summary>
        public sbyte Mechanism { get; set; } = 0;

        /// <summary>
        /// The number of iterations.
        /// </summary>
        public int Iterations { get; set; } = 0;

        /// <summary>
        /// A random salt generated by the client.
        /// </summary>
        public byte[] Salt { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// The salted password.
        /// </summary>
        public byte[] SaltedPassword { get; set; } = Array.Empty<byte>();

        public ScramCredentialUpsertionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public ScramCredentialUpsertionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteSByte(Mechanism);
            writer.WriteInt(Iterations);
            writer.WriteVarUInt(Salt.Length + 1);
            writer.WriteBytes(Salt);
            writer.WriteVarUInt(SaltedPassword.Length + 1);
            writer.WriteBytes(SaltedPassword);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is ScramCredentialUpsertionMessage other && Equals(other);
        }

        public bool Equals(ScramCredentialUpsertionMessage? other)
        {
            return true;
        }
    }
}
