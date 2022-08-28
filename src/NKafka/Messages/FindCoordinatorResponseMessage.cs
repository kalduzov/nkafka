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

public sealed class FindCoordinatorResponseMessage: ResponseMessage, IEquatable<FindCoordinatorResponseMessage>
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The error message, or null if there was no error.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// The node id.
    /// </summary>
    public int NodeId { get; set; } = 0;

    /// <summary>
    /// The host name.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// The port.
    /// </summary>
    public int Port { get; set; } = 0;

    /// <summary>
    /// Each coordinator result in the response
    /// </summary>
    public List<CoordinatorMessage> Coordinators { get; set; } = new ();

    public FindCoordinatorResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public FindCoordinatorResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version <= ApiVersions.Version3)
        {
            writer.WriteShort(ErrorCode);
        }
        else
        {
            if (ErrorCode != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ErrorCode at version {version}");
            }
        }
        if (version >= ApiVersions.Version1 && version <= ApiVersions.Version3)
        {
            if (ErrorMessage is null)
            {
                if (version >= ApiVersions.Version3)
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
                var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
                if (version >= ApiVersions.Version3)
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
        if (version <= ApiVersions.Version3)
        {
            writer.WriteInt(NodeId);
        }
        else
        {
            if (NodeId != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default NodeId at version {version}");
            }
        }
        if (version <= ApiVersions.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                if (version >= ApiVersions.Version3)
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
            if (!Host.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Host at version {version}");
            }
        }
        if (version <= ApiVersions.Version3)
        {
            writer.WriteInt(Port);
        }
        else
        {
            if (Port != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Port at version {version}");
            }
        }
        if (version >= ApiVersions.Version4)
        {
            writer.WriteVarUInt(Coordinators.Count + 1);
            foreach (var element in Coordinators)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (Coordinators.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Coordinators at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version3)
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
        return ReferenceEquals(this, obj) || obj is FindCoordinatorResponseMessage other && Equals(other);
    }

    public bool Equals(FindCoordinatorResponseMessage? other)
    {
        return true;
    }

    public sealed class CoordinatorMessage: Message, IEquatable<CoordinatorMessage>
    {
        /// <summary>
        /// The coordinator key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// The node id.
        /// </summary>
        public int NodeId { get; set; } = 0;

        /// <summary>
        /// The host name.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The port.
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <summary>
        /// The error message, or null if there was no error.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        public CoordinatorMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public CoordinatorMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version4)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of CoordinatorMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Key);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(NodeId);
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(Port);
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
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is CoordinatorMessage other && Equals(other);
        }

        public bool Equals(CoordinatorMessage? other)
        {
            return true;
        }
    }
}
