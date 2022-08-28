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

public sealed class DescribeUserScramCredentialsResponseMessage: ResponseMessage, IEquatable<DescribeUserScramCredentialsResponseMessage>
{
    /// <summary>
    /// The message-level error code, 0 except for user authorization or infrastructure issues.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The message-level error message, if any.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// The results for descriptions, one per user.
    /// </summary>
    public List<DescribeUserScramCredentialsResultMessage> Results { get; set; } = new ();

    public DescribeUserScramCredentialsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public DescribeUserScramCredentialsResponseMessage(BufferReader reader, ApiVersions version)
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
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteShort(ErrorCode);
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
        writer.WriteVarUInt(Results.Count + 1);
        foreach (var element in Results)
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
        return ReferenceEquals(this, obj) || obj is DescribeUserScramCredentialsResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeUserScramCredentialsResponseMessage? other)
    {
        return true;
    }

    public sealed class DescribeUserScramCredentialsResultMessage: Message, IEquatable<DescribeUserScramCredentialsResultMessage>
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// The user-level error code.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The user-level error message, if any.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// The mechanism and related information associated with the user's SCRAM credentials.
        /// </summary>
        public List<CredentialInfoMessage> CredentialInfos { get; set; } = new ();

        public DescribeUserScramCredentialsResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public DescribeUserScramCredentialsResultMessage(BufferReader reader, ApiVersions version)
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
                var stringBytes = Encoding.UTF8.GetBytes(User);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteShort(ErrorCode);
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
            writer.WriteVarUInt(CredentialInfos.Count + 1);
            foreach (var element in CredentialInfos)
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
            return ReferenceEquals(this, obj) || obj is DescribeUserScramCredentialsResultMessage other && Equals(other);
        }

        public bool Equals(DescribeUserScramCredentialsResultMessage? other)
        {
            return true;
        }
    }

    public sealed class CredentialInfoMessage: Message, IEquatable<CredentialInfoMessage>
    {
        /// <summary>
        /// The SCRAM mechanism.
        /// </summary>
        public sbyte Mechanism { get; set; } = 0;

        /// <summary>
        /// The number of iterations used in the SCRAM credential.
        /// </summary>
        public int Iterations { get; set; } = 0;

        public CredentialInfoMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public CredentialInfoMessage(BufferReader reader, ApiVersions version)
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
            writer.WriteSByte(Mechanism);
            writer.WriteInt(Iterations);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is CredentialInfoMessage other && Equals(other);
        }

        public bool Equals(CredentialInfoMessage? other)
        {
            return true;
        }
    }
}
