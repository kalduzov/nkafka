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

public sealed class UpdateMetadataRequestMessage: IRequestMessage, IEquatable<UpdateMetadataRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

    public ApiKeys ApiKey => ApiKeys.UpdateMetadata;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The controller id.
    /// </summary>
    public int ControllerId { get; set; } = 0;

    /// <summary>
    /// The controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; } = 0;

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// In older versions of this RPC, each partition that we would like to update.
    /// </summary>
    public List<UpdateMetadataPartitionStateMessage> UngroupedPartitionStates { get; set; } = new ();

    /// <summary>
    /// In newer versions of this RPC, each topic that we would like to update.
    /// </summary>
    public List<UpdateMetadataTopicStateMessage> TopicStates { get; set; } = new ();

    /// <summary>
    /// 
    /// </summary>
    public List<UpdateMetadataBrokerMessage> LiveBrokers { get; set; } = new ();

    public UpdateMetadataRequestMessage()
    {
    }

    public UpdateMetadataRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ControllerId = reader.ReadInt();
        ControllerEpoch = reader.ReadInt();
        if (version >= ApiVersions.Version5)
        {
            BrokerEpoch = reader.ReadLong();
        }
        else
        {
            BrokerEpoch = -1;
        }
        if (version <= ApiVersions.Version4)
        {
            int arrayLength;
            arrayLength = reader.ReadInt();
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field UngroupedPartitionStates was serialized as null");
            }
            else
            {
                var newCollection = new List<UpdateMetadataPartitionStateMessage>(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new UpdateMetadataPartitionStateMessage(reader, version));
                }
                UngroupedPartitionStates = newCollection;
            }
        }
        else
        {
            UngroupedPartitionStates = new ();
        }
        if (version >= ApiVersions.Version5)
        {
            if (version >= ApiVersions.Version6)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicStates was serialized as null");
                }
                else
                {
                    var newCollection = new List<UpdateMetadataTopicStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new UpdateMetadataTopicStateMessage(reader, version));
                    }
                    TopicStates = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field TopicStates was serialized as null");
                }
                else
                {
                    var newCollection = new List<UpdateMetadataTopicStateMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new UpdateMetadataTopicStateMessage(reader, version));
                    }
                    TopicStates = newCollection;
                }
            }
        }
        else
        {
            TopicStates = new ();
        }
        {
            if (version >= ApiVersions.Version6)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field LiveBrokers was serialized as null");
                }
                else
                {
                    var newCollection = new List<UpdateMetadataBrokerMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new UpdateMetadataBrokerMessage(reader, version));
                    }
                    LiveBrokers = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field LiveBrokers was serialized as null");
                }
                else
                {
                    var newCollection = new List<UpdateMetadataBrokerMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new UpdateMetadataBrokerMessage(reader, version));
                    }
                    LiveBrokers = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version6)
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
        writer.WriteInt(ControllerId);
        writer.WriteInt(ControllerEpoch);
        if (version >= ApiVersions.Version5)
        {
            writer.WriteLong(BrokerEpoch);
        }
        if (version <= ApiVersions.Version4)
        {
            writer.WriteInt(UngroupedPartitionStates.Count);
            foreach (var element in UngroupedPartitionStates)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            if (UngroupedPartitionStates.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default UngroupedPartitionStates at version {version}");
            }
        }
        if (version >= ApiVersions.Version5)
        {
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(TopicStates.Count + 1);
                foreach (var element in TopicStates)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(TopicStates.Count);
                foreach (var element in TopicStates)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (TopicStates.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default TopicStates at version {version}");
            }
        }
        if (version >= ApiVersions.Version6)
        {
            writer.WriteVarUInt(LiveBrokers.Count + 1);
            foreach (var element in LiveBrokers)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(LiveBrokers.Count);
            foreach (var element in LiveBrokers)
            {
                element.Write(writer, version);
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
        return ReferenceEquals(this, obj) || obj is UpdateMetadataRequestMessage other && Equals(other);
    }

    public bool Equals(UpdateMetadataRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ControllerId, ControllerEpoch, BrokerEpoch, UngroupedPartitionStates, TopicStates, LiveBrokers);
        return hashCode;
    }

    public sealed class UpdateMetadataTopicStateMessage: IMessage, IEquatable<UpdateMetadataTopicStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The topic id.
        /// </summary>
        public Guid TopicId { get; set; } = Guid.Empty;

        /// <summary>
        /// The partition that we would like to update.
        /// </summary>
        public List<UpdateMetadataPartitionStateMessage> PartitionStates { get; set; } = new ();

        public UpdateMetadataTopicStateMessage()
        {
        }

        public UpdateMetadataTopicStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of UpdateMetadataTopicStateMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field TopicName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TopicName had invalid length {length}");
                }
                else
                {
                    TopicName = reader.ReadString(length);
                }
            }
            if (version >= ApiVersions.Version7)
            {
                TopicId = reader.ReadGuid();
            }
            else
            {
                TopicId = Guid.Empty;
            }
            {
                if (version >= ApiVersions.Version6)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionStates was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<UpdateMetadataPartitionStateMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new UpdateMetadataPartitionStateMessage(reader, version));
                        }
                        PartitionStates = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field PartitionStates was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<UpdateMetadataPartitionStateMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new UpdateMetadataPartitionStateMessage(reader, version));
                        }
                        PartitionStates = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
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
            if (version < ApiVersions.Version5)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of UpdateMetadataTopicStateMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
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
            if (version >= ApiVersions.Version7)
            {
                writer.WriteGuid(TopicId);
            }
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(PartitionStates.Count + 1);
                foreach (var element in PartitionStates)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(PartitionStates.Count);
                foreach (var element in PartitionStates)
                {
                    element.Write(writer, version);
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
            return ReferenceEquals(this, obj) || obj is UpdateMetadataTopicStateMessage other && Equals(other);
        }

        public bool Equals(UpdateMetadataTopicStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicName, TopicId, PartitionStates);
            return hashCode;
        }
    }

    public sealed class UpdateMetadataBrokerMessage: IMessage, IEquatable<UpdateMetadataBrokerMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The broker id.
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// The broker hostname.
        /// </summary>
        public string V0Host { get; set; } = string.Empty;

        /// <summary>
        /// The broker port.
        /// </summary>
        public int V0Port { get; set; } = 0;

        /// <summary>
        /// The broker endpoints.
        /// </summary>
        public List<UpdateMetadataEndpointMessage> Endpoints { get; set; } = new ();

        /// <summary>
        /// The rack which this broker belongs to.
        /// </summary>
        public string Rack { get; set; } = string.Empty;

        public UpdateMetadataBrokerMessage()
        {
        }

        public UpdateMetadataBrokerMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of UpdateMetadataBrokerMessage");
            }
            Id = reader.ReadInt();
            if (version <= ApiVersions.Version0)
            {
                int length;
                length = reader.ReadShort();
                if (length < 0)
                {
                    throw new Exception("non-nullable field V0Host was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field V0Host had invalid length {length}");
                }
                else
                {
                    V0Host = reader.ReadString(length);
                }
            }
            else
            {
                V0Host = string.Empty;
            }
            if (version <= ApiVersions.Version0)
            {
                V0Port = reader.ReadInt();
            }
            else
            {
                V0Port = 0;
            }
            if (version >= ApiVersions.Version1)
            {
                if (version >= ApiVersions.Version6)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Endpoints was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<UpdateMetadataEndpointMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new UpdateMetadataEndpointMessage(reader, version));
                        }
                        Endpoints = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Endpoints was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<UpdateMetadataEndpointMessage>(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new UpdateMetadataEndpointMessage(reader, version));
                        }
                        Endpoints = newCollection;
                    }
                }
            }
            else
            {
                Endpoints = new ();
            }
            if (version >= ApiVersions.Version2)
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
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
            else
            {
                Rack = string.Empty;
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
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
            writer.WriteInt(Id);
            if (version <= ApiVersions.Version0)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(V0Host);
                    writer.WriteShort((short)stringBytes.Length);
                    writer.WriteBytes(stringBytes);
                }
            }
            if (version <= ApiVersions.Version0)
            {
                writer.WriteInt(V0Port);
            }
            if (version >= ApiVersions.Version1)
            {
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(Endpoints.Count + 1);
                    foreach (var element in Endpoints)
                    {
                        element.Write(writer, version);
                    }
                }
                else
                {
                    writer.WriteInt(Endpoints.Count);
                    foreach (var element in Endpoints)
                    {
                        element.Write(writer, version);
                    }
                }
            }
            if (version >= ApiVersions.Version2)
            {
                if (Rack is null)
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
                    var stringBytes = Encoding.UTF8.GetBytes(Rack);
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
            return ReferenceEquals(this, obj) || obj is UpdateMetadataBrokerMessage other && Equals(other);
        }

        public bool Equals(UpdateMetadataBrokerMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Id, V0Host, V0Port, Endpoints, Rack);
            return hashCode;
        }
    }

    public sealed class UpdateMetadataEndpointMessage: IMessage, IEquatable<UpdateMetadataEndpointMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The port of this endpoint
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// The hostname of this endpoint
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The listener name.
        /// </summary>
        public string Listener { get; set; } = string.Empty;

        /// <summary>
        /// The security protocol type.
        /// </summary>
        public short SecurityProtocol { get; set; } = 0;

        public UpdateMetadataEndpointMessage()
        {
        }

        public UpdateMetadataEndpointMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of UpdateMetadataEndpointMessage");
            }
            Port = reader.ReadInt();
            {
                int length;
                if (version >= ApiVersions.Version6)
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
            if (version >= ApiVersions.Version3)
            {
                int length;
                if (version >= ApiVersions.Version6)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Listener was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Listener had invalid length {length}");
                }
                else
                {
                    Listener = reader.ReadString(length);
                }
            }
            else
            {
                Listener = string.Empty;
            }
            SecurityProtocol = reader.ReadShort();
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
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
            if (version < ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of UpdateMetadataEndpointMessage");
            }
            var numTaggedFields = 0;
            writer.WriteInt(Port);
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
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
            if (version >= ApiVersions.Version3)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(Listener);
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
            writer.WriteShort(SecurityProtocol);
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
            return ReferenceEquals(this, obj) || obj is UpdateMetadataEndpointMessage other && Equals(other);
        }

        public bool Equals(UpdateMetadataEndpointMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Port, Host, Listener, SecurityProtocol);
            return hashCode;
        }
    }

    public sealed class UpdateMetadataPartitionStateMessage: IMessage, IEquatable<UpdateMetadataPartitionStateMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version7;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// In older versions of this RPC, the topic name.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The partition index.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The controller epoch.
        /// </summary>
        public int ControllerEpoch { get; set; } = 0;

        /// <summary>
        /// The ID of the broker which is the current partition leader.
        /// </summary>
        public int Leader { get; set; } = 0;

        /// <summary>
        /// The leader epoch of this partition.
        /// </summary>
        public int LeaderEpoch { get; set; } = 0;

        /// <summary>
        /// The brokers which are in the ISR for this partition.
        /// </summary>
        public List<int> Isr { get; set; } = new ();

        /// <summary>
        /// The Zookeeper version.
        /// </summary>
        public int ZkVersion { get; set; } = 0;

        /// <summary>
        /// All the replicas of this partition.
        /// </summary>
        public List<int> Replicas { get; set; } = new ();

        /// <summary>
        /// The replicas of this partition which are offline.
        /// </summary>
        public List<int> OfflineReplicas { get; set; } = new ();

        public UpdateMetadataPartitionStateMessage()
        {
        }

        public UpdateMetadataPartitionStateMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version7)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of UpdateMetadataPartitionStateMessage");
            }
            if (version <= ApiVersions.Version4)
            {
                int length;
                length = reader.ReadShort();
                if (length < 0)
                {
                    throw new Exception("non-nullable field TopicName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field TopicName had invalid length {length}");
                }
                else
                {
                    TopicName = reader.ReadString(length);
                }
            }
            else
            {
                TopicName = string.Empty;
            }
            PartitionIndex = reader.ReadInt();
            ControllerEpoch = reader.ReadInt();
            Leader = reader.ReadInt();
            LeaderEpoch = reader.ReadInt();
            {
                int arrayLength;
                if (version >= ApiVersions.Version6)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Isr was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Isr = newCollection;
                }
            }
            ZkVersion = reader.ReadInt();
            {
                int arrayLength;
                if (version >= ApiVersions.Version6)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Replicas was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Replicas = newCollection;
                }
            }
            if (version >= ApiVersions.Version4)
            {
                int arrayLength;
                if (version >= ApiVersions.Version6)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field OfflineReplicas was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    OfflineReplicas = newCollection;
                }
            }
            else
            {
                OfflineReplicas = new ();
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version6)
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
            if (version <= ApiVersions.Version4)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(TopicName);
                    writer.WriteShort((short)stringBytes.Length);
                    writer.WriteBytes(stringBytes);
                }
            }
            writer.WriteInt(PartitionIndex);
            writer.WriteInt(ControllerEpoch);
            writer.WriteInt(Leader);
            writer.WriteInt(LeaderEpoch);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(Isr.Count + 1);
            }
            else
            {
                writer.WriteInt(Isr.Count);
            }
            foreach (var element in Isr)
            {
                writer.WriteInt(element);
            }
            writer.WriteInt(ZkVersion);
            if (version >= ApiVersions.Version6)
            {
                writer.WriteVarUInt(Replicas.Count + 1);
            }
            else
            {
                writer.WriteInt(Replicas.Count);
            }
            foreach (var element in Replicas)
            {
                writer.WriteInt(element);
            }
            if (version >= ApiVersions.Version4)
            {
                if (version >= ApiVersions.Version6)
                {
                    writer.WriteVarUInt(OfflineReplicas.Count + 1);
                }
                else
                {
                    writer.WriteInt(OfflineReplicas.Count);
                }
                foreach (var element in OfflineReplicas)
                {
                    writer.WriteInt(element);
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
            return ReferenceEquals(this, obj) || obj is UpdateMetadataPartitionStateMessage other && Equals(other);
        }

        public bool Equals(UpdateMetadataPartitionStateMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, TopicName, PartitionIndex, ControllerEpoch, Leader, LeaderEpoch, Isr, ZkVersion);
            hashCode = HashCode.Combine(hashCode, Replicas, OfflineReplicas);
            return hashCode;
        }
    }
}
