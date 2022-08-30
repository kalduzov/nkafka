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

public sealed class DescribeClientQuotasRequestMessage: IRequestMessage, IEquatable<DescribeClientQuotasRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.DescribeClientQuotas;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// Filter components to apply to quota entities.
    /// </summary>
    public List<ComponentDataMessage> Components { get; set; } = new ();

    /// <summary>
    /// Whether the match is strict, i.e. should exclude entities with unspecified entity types.
    /// </summary>
    public bool Strict { get; set; } = false;

    public DescribeClientQuotasRequestMessage()
    {
    }

    public DescribeClientQuotasRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            if (version >= ApiVersion.Version1)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Components was serialized as null");
                }
                else
                {
                    var newCollection = new List<ComponentDataMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ComponentDataMessage(reader, version));
                    }
                    Components = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Components was serialized as null");
                }
                else
                {
                    var newCollection = new List<ComponentDataMessage>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ComponentDataMessage(reader, version));
                    }
                    Components = newCollection;
                }
            }
        }
        Strict = reader.ReadByte() != 0;
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version1)
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        if (version >= ApiVersion.Version1)
        {
            writer.WriteVarUInt(Components.Count + 1);
            foreach (var element in Components)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Components.Count);
            foreach (var element in Components)
            {
                element.Write(writer, version);
            }
        }
        writer.WriteBool(Strict);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version1)
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
        return ReferenceEquals(this, obj) || obj is DescribeClientQuotasRequestMessage other && Equals(other);
    }

    public bool Equals(DescribeClientQuotasRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Components, Strict);
        return hashCode;
    }

    public override string ToString()
    {
        return "DescribeClientQuotasRequestMessage("
            + ", Strict=" + (Strict ? "true" : "false")
            + ")";
    }

    public sealed class ComponentDataMessage: IMessage, IEquatable<ComponentDataMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The entity type that the filter component applies to.
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// How to match the entity {0 = exact name, 1 = default name, 2 = any specified name}.
        /// </summary>
        public sbyte MatchType { get; set; } = 0;

        /// <summary>
        /// The string to match against, or null if unused for the match type.
        /// </summary>
        public string Match { get; set; } = string.Empty;

        public ComponentDataMessage()
        {
        }

        public ComponentDataMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of ComponentDataMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version1)
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
            MatchType = reader.ReadSByte();
            {
                int length;
                if (version >= ApiVersion.Version1)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    Match = null;
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Match had invalid length {length}");
                }
                else
                {
                    Match = reader.ReadString(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version1)
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(EntityType);
                if (version >= ApiVersion.Version1)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            writer.WriteSByte(MatchType);
            if (Match is null)
            {
                if (version >= ApiVersion.Version1)
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
                var stringBytes = Encoding.UTF8.GetBytes(Match);
                if (version >= ApiVersion.Version1)
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
            if (version >= ApiVersion.Version1)
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
            return ReferenceEquals(this, obj) || obj is ComponentDataMessage other && Equals(other);
        }

        public bool Equals(ComponentDataMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, EntityType, MatchType, Match);
            return hashCode;
        }

        public override string ToString()
        {
            return "ComponentDataMessage("
                + ", MatchType=" + MatchType
                + ")";
        }
    }
}
