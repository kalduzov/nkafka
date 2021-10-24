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
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed partial class DescribeClusterResponseMessage: IResponseMessage, IEquatable<DescribeClusterResponseMessage>
{
    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The top-level error code, or 0 if there was no error
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The top-level error message, or null if there was no error.
    /// </summary>
    public string? ErrorMessage { get; set; } = null;

    /// <summary>
    /// The cluster ID that responding broker belongs to.
    /// </summary>
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the controller broker.
    /// </summary>
    public int ControllerId { get; set; } = -1;

    /// <summary>
    /// Each broker in the response.
    /// </summary>
    public DescribeClusterBrokerCollection Brokers { get; set; } = new ();

    /// <summary>
    /// 32-bit bitfield to represent authorized operations for this cluster.
    /// </summary>
    public int ClusterAuthorizedOperations { get; set; } = -2147483648;

    public DescribeClusterResponseMessage()
    {
    }

    public DescribeClusterResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
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
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field ClusterId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ClusterId had invalid length {length}");
            }
            else
            {
                ClusterId = reader.ReadString(length);
            }
        }
        ControllerId = reader.ReadInt();
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Brokers was serialized as null");
            }
            else
            {
                var newCollection = new DescribeClusterBrokerCollection(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new DescribeClusterBrokerMessage(reader, version));
                }
                Brokers = newCollection;
            }
        }
        ClusterAuthorizedOperations = reader.ReadInt();
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
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
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
        {
            var stringBytes = Encoding.UTF8.GetBytes(ClusterId);
            writer.WriteVarUInt(stringBytes.Length + 1);
            writer.WriteBytes(stringBytes);
        }
        writer.WriteInt(ControllerId);
        writer.WriteVarUInt(Brokers.Count + 1);
        foreach (var element in Brokers)
        {
            element.Write(writer, version);
        }
        writer.WriteInt(ClusterAuthorizedOperations);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is DescribeClusterResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeClusterResponseMessage? other)
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
        if (ClusterId is null)
        {
            if (other.ClusterId is not null)
            {
                return false;
            }
        }
        else
        {
            if (!ClusterId.Equals(other.ClusterId))
            {
                return false;
            }
        }
        if (ControllerId != other.ControllerId)
        {
            return false;
        }
        if (Brokers is null)
        {
            if (other.Brokers is not null)
            {
                return false;
            }
        }
        else
        {
            if (!Brokers.SequenceEqual(other.Brokers))
            {
                return false;
            }
        }
        if (ClusterAuthorizedOperations != other.ClusterAuthorizedOperations)
        {
            return false;
        }
        return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, ErrorCode, ErrorMessage, ClusterId, ControllerId, Brokers, ClusterAuthorizedOperations);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeClusterResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ", ErrorCode=" + ErrorCode
            + ", ErrorMessage=" + (string.IsNullOrWhiteSpace(ErrorMessage) ? "null" : ErrorMessage)
            + ", ClusterId=" + (string.IsNullOrWhiteSpace(ClusterId) ? "null" : ClusterId)
            + ", ControllerId=" + ControllerId
            + ", Brokers=" + Brokers.DeepToString()
            + ", ClusterAuthorizedOperations=" + ClusterAuthorizedOperations
            + ")";
    }

    public sealed partial class DescribeClusterBrokerMessage: IMessage, IEquatable<DescribeClusterBrokerMessage>
    {
        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The broker ID.
        /// </summary>
        public int BrokerId { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The broker port.
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// The rack of the broker, or null if it has not been assigned to a rack.
        /// </summary>
        public string? Rack { get; set; } = null;

        public DescribeClusterBrokerMessage()
        {
        }

        public DescribeClusterBrokerMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribeClusterBrokerMessage");
            }
            BrokerId = reader.ReadInt();
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
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    Rack = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Rack had invalid length {length}");
                }
                else
                {
                    Rack = reader.ReadString(length);
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(BrokerId);
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteInt(Port);
            if (Rack is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(Rack);
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
            return ReferenceEquals(this, obj) || obj is DescribeClusterBrokerMessage other && Equals(other);
        }

        public bool Equals(DescribeClusterBrokerMessage? other)
        {
            if (other is null)
            {
                return false;
            }
            if (BrokerId != other.BrokerId)
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
            if (Rack is null)
            {
                if (other.Rack is not null)
                {
                    return false;
                }
            }
            else
            {
                if (!Rack.Equals(other.Rack))
                {
                    return false;
                }
            }
            return UnknownTaggedFields.CompareRawTaggedFields(other.UnknownTaggedFields);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, BrokerId);
            return hashCode;
        }

        public override string ToString()
        {
            return "DescribeClusterBrokerMessage("
                + "BrokerId=" + BrokerId
                + ", Host=" + (string.IsNullOrWhiteSpace(Host) ? "null" : Host)
                + ", Port=" + Port
                + ", Rack=" + (string.IsNullOrWhiteSpace(Rack) ? "null" : Rack)
                + ")";
        }
    }

    public sealed partial class DescribeClusterBrokerCollection: HashSet<DescribeClusterBrokerMessage>
    {
        public DescribeClusterBrokerCollection()
        {
        }

        public DescribeClusterBrokerCollection(int capacity)
            : base(capacity)
        {
        }
        public override bool Equals(object? obj)
        {
            return SetEquals((IEnumerable<DescribeClusterBrokerMessage>)obj);
        }
    }
}
