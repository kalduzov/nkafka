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

public sealed class AddPartitionsToTxnResponseMessage: IResponseMessage, IEquatable<AddPartitionsToTxnResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// Duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The results for each topic.
    /// </summary>
    public AddPartitionsToTxnTopicResultCollection Results { get; set; } = new ();

    public AddPartitionsToTxnResponseMessage()
    {
    }

    public AddPartitionsToTxnResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ThrottleTimeMs = reader.ReadInt();
        {
            if (version >= ApiVersion.Version3)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Results was serialized as null");
                }
                else
                {
                    var newCollection = new AddPartitionsToTxnTopicResultCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new AddPartitionsToTxnTopicResultMessage(reader, version));
                    }
                    Results = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Results was serialized as null");
                }
                else
                {
                    var newCollection = new AddPartitionsToTxnTopicResultCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new AddPartitionsToTxnTopicResultMessage(reader, version));
                    }
                    Results = newCollection;
                }
            }
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

    public void Write(BufferWriter writer, ApiVersion version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
        if (version >= ApiVersion.Version3)
        {
            writer.WriteVarUInt(Results.Count + 1);
            foreach (var element in Results)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Results.Count);
            foreach (var element in Results)
            {
                element.Write(writer, version);
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

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is AddPartitionsToTxnResponseMessage other && Equals(other);
    }

    public bool Equals(AddPartitionsToTxnResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ThrottleTimeMs, Results);
        return hashCode;
    }

    public override string ToString()
    {
        return "AddPartitionsToTxnResponseMessage("
            + "ThrottleTimeMs=" + ThrottleTimeMs
            + ")";
    }

    public sealed class AddPartitionsToTxnTopicResultMessage: IMessage, IEquatable<AddPartitionsToTxnTopicResultMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The results for each partition
        /// </summary>
        public AddPartitionsToTxnPartitionResultCollection Results { get; set; } = new ();

        public AddPartitionsToTxnTopicResultMessage()
        {
        }

        public AddPartitionsToTxnTopicResultMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of AddPartitionsToTxnTopicResultMessage");
            }
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
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
            {
                if (version >= ApiVersion.Version3)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Results was serialized as null");
                    }
                    else
                    {
                        var newCollection = new AddPartitionsToTxnPartitionResultCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AddPartitionsToTxnPartitionResultMessage(reader, version));
                        }
                        Results = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Results was serialized as null");
                    }
                    else
                    {
                        var newCollection = new AddPartitionsToTxnPartitionResultCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AddPartitionsToTxnPartitionResultMessage(reader, version));
                        }
                        Results = newCollection;
                    }
                }
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
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
            if (version >= ApiVersion.Version3)
            {
                writer.WriteVarUInt(Results.Count + 1);
                foreach (var element in Results)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Results.Count);
                foreach (var element in Results)
                {
                    element.Write(writer, version);
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


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is AddPartitionsToTxnTopicResultMessage other && Equals(other);
        }

        public bool Equals(AddPartitionsToTxnTopicResultMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "AddPartitionsToTxnTopicResultMessage("
                + ")";
        }
    }

    public sealed class AddPartitionsToTxnPartitionResultMessage: IMessage, IEquatable<AddPartitionsToTxnPartitionResultMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The partition indexes.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        /// <summary>
        /// The response error code.
        /// </summary>
        public short ErrorCode { get; set; } = 0;

        /// <inheritdoc />
        public ErrorCodes Code => (ErrorCodes)ErrorCode;

        public AddPartitionsToTxnPartitionResultMessage()
        {
        }

        public AddPartitionsToTxnPartitionResultMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of AddPartitionsToTxnPartitionResultMessage");
            }
            PartitionIndex = reader.ReadInt();
            ErrorCode = reader.ReadShort();
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

        public void Write(BufferWriter writer, ApiVersion version)
        {
            var numTaggedFields = 0;
            writer.WriteInt(PartitionIndex);
            writer.WriteShort((short)ErrorCode);
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


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is AddPartitionsToTxnPartitionResultMessage other && Equals(other);
        }

        public bool Equals(AddPartitionsToTxnPartitionResultMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, PartitionIndex);
            return hashCode;
        }

        public override string ToString()
        {
            return "AddPartitionsToTxnPartitionResultMessage("
                + "PartitionIndex=" + PartitionIndex
                + ", ErrorCode=" + ErrorCode
                + ")";
        }
    }

    public sealed class AddPartitionsToTxnPartitionResultCollection: HashSet<AddPartitionsToTxnPartitionResultMessage>
    {
        public AddPartitionsToTxnPartitionResultCollection()
        {
        }

        public AddPartitionsToTxnPartitionResultCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class AddPartitionsToTxnTopicResultCollection: HashSet<AddPartitionsToTxnTopicResultMessage>
    {
        public AddPartitionsToTxnTopicResultCollection()
        {
        }

        public AddPartitionsToTxnTopicResultCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
