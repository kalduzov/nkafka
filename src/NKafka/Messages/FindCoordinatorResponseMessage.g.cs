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
// <auto-generated> THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT. </auto-generated>

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

#nullable enable

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

/// <summary>
/// Describes the contract for message FindCoordinatorResponseMessage
/// </summary>
public sealed partial class FindCoordinatorResponseMessage: IResponseMessage, IEquatable<FindCoordinatorResponseMessage>
{
    /// <inheritdoc />
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <inheritdoc />
    public int IncomingBufferLength { get; private set; } = 0;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

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

    /// <summary>
    /// The basic constructor of the message FindCoordinatorResponseMessage
    /// </summary>
    public FindCoordinatorResponseMessage()
    {
    }

    /// <summary>
    /// Base constructor for deserializing message FindCoordinatorResponseMessage
    /// </summary>
    public FindCoordinatorResponseMessage(ref BufferReader reader, ApiVersion version)
        : this()
    {
        IncomingBufferLength = reader.Length;
        Read(ref reader, version);
    }

    /// <inheritdoc />
    public void Read(ref BufferReader reader, ApiVersion version)
    {
        if (version >= ApiVersion.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        if (version <= ApiVersion.Version3)
        {
            ErrorCode = reader.ReadShort();
        }
        else
        {
            ErrorCode = 0;
        }
        if (version >= ApiVersion.Version1 && version <= ApiVersion.Version3)
        {
            int length;
            if (version >= ApiVersion.Version3)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
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
            ErrorMessage = string.Empty;
        }
        if (version <= ApiVersion.Version3)
        {
            NodeId = reader.ReadInt();
        }
        else
        {
            NodeId = 0;
        }
        if (version <= ApiVersion.Version3)
        {
            int length;
            if (version >= ApiVersion.Version3)
            {
                length = reader.ReadVarUInt() - 1;
            }
            else
            {
                length = reader.ReadShort();
            }
            if (length < 0)
            {
                throw new Exception("non-nullable field Host was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field Host had invalid length {length}");
            }
            else
            {
                Host = reader.ReadString(length);
            }
        }
        else
        {
            Host = string.Empty;
        }
        if (version <= ApiVersion.Version3)
        {
            Port = reader.ReadInt();
        }
        else
        {
            Port = 0;
        }
        if (version >= ApiVersion.Version4)
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Coordinators was serialized as null");
            }
            else
            {
                var newCollection = new List<CoordinatorMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new CoordinatorMessage(ref reader, version));
                }
                Coordinators = newCollection;
            }
        }
        else
        {
            Coordinators = new ();
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version3)
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

    /// <inheritdoc />
    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersion.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version <= ApiVersion.Version3)
        {
            writer.WriteShort((short)ErrorCode);
        }
        else
        {
            if (ErrorCode != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ErrorCode at version {version}");
            }
        }
        if (version >= ApiVersion.Version1 && version <= ApiVersion.Version3)
        {
            if (ErrorMessage is null)
            {
                if (version >= ApiVersion.Version3)
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
                if (version >= ApiVersion.Version3)
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
        if (version <= ApiVersion.Version3)
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
        if (version <= ApiVersion.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                if (version >= ApiVersion.Version3)
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
        if (version <= ApiVersion.Version3)
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
        if (version >= ApiVersion.Version4)
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
        if (version >= ApiVersion.Version3)
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

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is FindCoordinatorResponseMessage other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(FindCoordinatorResponseMessage? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ThrottleTimeMs != other.ThrottleTimeMs)
        {
            return false;
        }
        if (ErrorCode != other.ErrorCode)
        {
            return false;
        }
        if (ErrorMessage is null)
        {
            if (other.ErrorMessage is not null)
            {
                return false;
            }
        }
        else
        {
            if (!ErrorMessage.Equals(other.ErrorMessage))
            {
                return false;
            }
        }
        if (NodeId != other.NodeId)
        {
            return false;
        }
        if (Host is null)
        {
            if (other.Host is not null)
            {
                return false;
            }
        }
        else
        {
            if (!Host.Equals(other.Host))
            {
                return false;
            }
        }
        if (Port != other.Port)
        {
            return false;
        }
        if (Coordinators is null)
        {
            if (other.Coordinators is not null)
            {
                return false;
            }
        }
        else
        {
            if (!Coordinators.SequenceEqual(other.Coordinators))
            {
                return false;
            }
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, ErrorMessage, NodeId, Host, Port, Coordinators);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "FindCoordinatorResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ", ErrorMessage=" + (string.IsNullOrWhiteSpace(ErrorMessage) ? "null" : ErrorMessage)
            + ", NodeId=" + NodeId
            + ", Host=" + (string.IsNullOrWhiteSpace(Host) ? "null" : Host)
            + ", Port=" + Port
            + ", Coordinators=" + Coordinators.DeepToString()
            + ")";
    }

    /// <summary>
    /// Describes the contract for message CoordinatorMessage
    /// </summary>
    public sealed partial class CoordinatorMessage: IMessage, IEquatable<CoordinatorMessage>
    {
        /// <inheritdoc />
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <inheritdoc />
        public int IncomingBufferLength { get; private set; } = 0;

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

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        /// <summary>
        /// The error message, or null if there was no error.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// The basic constructor of the message CoordinatorMessage
        /// </summary>
        public CoordinatorMessage()
        {
        }

        /// <summary>
        /// Base constructor for deserializing message CoordinatorMessage
        /// </summary>
        public CoordinatorMessage(ref BufferReader reader, ApiVersion version)
            : this()
        {
            IncomingBufferLength = reader.Length;
            Read(ref reader, version);
        }

        /// <inheritdoc />
        public void Read(ref BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version4)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of CoordinatorMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field Key was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Key had invalid length {length}");
                }
                else
                {
                    Key = reader.ReadString(length);
                }
            }
            NodeId = reader.ReadInt();
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field Host was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Host had invalid length {length}");
                }
                else
                {
                    Host = reader.ReadString(length);
                }
            }
            Port = reader.ReadInt();
            ErrorCode = reader.ReadShort();
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

        /// <inheritdoc />
        public void Write(BufferWriter writer, ApiVersion version)
        {
            if (version < ApiVersion.Version4)
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
            writer.WriteShort((short)ErrorCode);
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is CoordinatorMessage other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(CoordinatorMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (Key is null)
            {
                if (other.Key is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Key.Equals(other.Key))
                {
                    return false;
                }
            }
            if (NodeId != other.NodeId)
            {
                return false;
            }
            if (Host is null)
            {
                if (other.Host is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Host.Equals(other.Host))
                {
                    return false;
                }
            }
            if (Port != other.Port)
            {
                return false;
            }
            if (ErrorCode != other.ErrorCode)
            {
                return false;
            }
            if (ErrorMessage is null)
            {
                if (other.ErrorMessage is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!ErrorMessage.Equals(other.ErrorMessage))
                {
                    return false;
                }
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Key, NodeId, Host, Port, ErrorCode, ErrorMessage);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "CoordinatorMessage("
                + "Key=" + (string.IsNullOrWhiteSpace(Key) ? "null" : Key)
                + ", NodeId=" + NodeId
                + ", Host=" + (string.IsNullOrWhiteSpace(Host) ? "null" : Host)
                + ", Port=" + Port
                + ", ErrorCode=" + ErrorCode
                + ", ErrorMessage=" + (string.IsNullOrWhiteSpace(ErrorMessage) ? "null" : ErrorMessage)
                + ")";
        }
    }
}
