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

public sealed class DescribeClientQuotasResponseMessage: IResponseMessage, IEquatable<DescribeClientQuotasResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The error code, or `0` if the quota description succeeded.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The error message, or `null` if the quota description succeeded.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// A result entry.
    /// </summary>
    public List<EntryDataMessage> Entries { get; set; } = new ();

    public DescribeClientQuotasResponseMessage()
    {
    }

    public DescribeClientQuotasResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ThrottleTimeMs = reader.ReadInt();
        ErrorCode = reader.ReadShort();
        {
            int length;
            if (version >= ApiVersions.Version1)
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
        {
            if (version >= ApiVersions.Version1)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    Entries = null;
                }
                else
                {
                    var newCollection = new List<EntryDataMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new EntryDataMessage(reader, version));
                    }
                    Entries = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    Entries = null;
                }
                else
                {
                    var newCollection = new List<EntryDataMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new EntryDataMessage(reader, version));
                    }
                    Entries = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version1)
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
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteShort((short)ErrorCode);
        if (ErrorMessage is null)
        {
            if (version >= ApiVersions.Version1)
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
            if (version >= ApiVersions.Version1)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersions.Version1)
        {
            if (Entries is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteVarUInt(Entries.Count + 1);
                foreach (var element in Entries)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (Entries is null)
            {
                writer.WriteInt(-1);
            }
            else
            {
                writer.WriteInt(Entries.Count);
                foreach (var element in Entries)
                {
                    element.Write(writer, version);
                }
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version1)
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
        return ReferenceEquals(this, obj) || obj is DescribeClientQuotasResponseMessage other && Equals(other);
    }

    public bool Equals(DescribeClientQuotasResponseMessage? other)
    {
        return true;
    }

    public sealed class EntryDataMessage: IMessage, IEquatable<EntryDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The quota entity description.
        /// </summary>
        public List<EntityDataMessage> Entity { get; set; } = new ();

        /// <summary>
        /// The quota values for the entity.
        /// </summary>
        public List<ValueDataMessage> Values { get; set; } = new ();

        public EntryDataMessage()
        {
        }

        public EntryDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of EntryDataMessage");
            }
            {
                if (version >= ApiVersions.Version1)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Entity was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<EntityDataMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new EntityDataMessage(reader, version));
                        }
                        Entity = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Entity was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<EntityDataMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new EntityDataMessage(reader, version));
                        }
                        Entity = newCollection;
                    }
                }
            }
            {
                if (version >= ApiVersions.Version1)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Values was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<ValueDataMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new ValueDataMessage(reader, version));
                        }
                        Values = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Values was serialized as null");
                    }
                    else
                    {
                        var newCollection = new List<ValueDataMessage>(arrayLength);
                        for (var i = 0; i< arrayLength; i++)
                        {
                            newCollection.Add(new ValueDataMessage(reader, version));
                        }
                        Values = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version1)
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
            if (version >= ApiVersions.Version1)
            {
                writer.WriteVarUInt(Entity.Count + 1);
                foreach (var element in Entity)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Entity.Count);
                foreach (var element in Entity)
                {
                    element.Write(writer, version);
                }
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteVarUInt(Values.Count + 1);
                foreach (var element in Values)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Values.Count);
                foreach (var element in Values)
                {
                    element.Write(writer, version);
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version1)
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
            return ReferenceEquals(this, obj) || obj is EntryDataMessage other && Equals(other);
        }

        public bool Equals(EntryDataMessage? other)
        {
            return true;
        }
    }

    public sealed class EntityDataMessage: IMessage, IEquatable<EntityDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The entity type.
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// The entity name, or null if the default.
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        public EntityDataMessage()
        {
        }

        public EntityDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of EntityDataMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version1)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field EntityType was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field EntityType had invalid length {length}");
                }
                else
                {
                    EntityType = reader.ReadString(length);
                }
            }
            {
                int length;
                if (version >= ApiVersions.Version1)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    EntityName = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field EntityName had invalid length {length}");
                }
                else
                {
                    EntityName = reader.ReadString(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version1)
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(EntityType);
                if (version >= ApiVersions.Version1)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (EntityName is null)
            {
                if (version >= ApiVersions.Version1)
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
                var stringBytes = Encoding.UTF8.GetBytes(EntityName);
                if (version >= ApiVersions.Version1)
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
            if (version >= ApiVersions.Version1)
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
            return ReferenceEquals(this, obj) || obj is EntityDataMessage other && Equals(other);
        }

        public bool Equals(EntityDataMessage? other)
        {
            return true;
        }
    }

    public sealed class ValueDataMessage: IMessage, IEquatable<ValueDataMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version1;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The quota configuration key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// The quota configuration value.
        /// </summary>
        public double Value { get; set; } = 0.0;

        public ValueDataMessage()
        {
        }

        public ValueDataMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of ValueDataMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version1)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
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
            Value = reader.ReadDouble();
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version1)
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Key);
                if (version >= ApiVersions.Version1)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            writer.WriteDouble(Value);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersions.Version1)
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
            return ReferenceEquals(this, obj) || obj is ValueDataMessage other && Equals(other);
        }

        public bool Equals(ValueDataMessage? other)
        {
            return true;
        }
    }
}
