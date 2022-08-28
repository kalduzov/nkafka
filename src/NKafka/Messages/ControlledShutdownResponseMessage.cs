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

public sealed class ControlledShutdownResponseMessage: IResponseMessage, IEquatable<ControlledShutdownResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The top-level error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The partitions that the broker still leads.
    /// </summary>
    public RemainingPartitionCollection RemainingPartitions { get; set; } = new ();

    public ControlledShutdownResponseMessage()
    {
    }

    public ControlledShutdownResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ErrorCode = reader.ReadShort();
        {
            if (version >= ApiVersions.Version3)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field RemainingPartitions was serialized as null");
                }
                else
                {
                    RemainingPartitionCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new RemainingPartitionMessage(reader, version));
                    }
                    RemainingPartitions = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field RemainingPartitions was serialized as null");
                }
                else
                {
                    RemainingPartitionCollection newCollection = new(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new RemainingPartitionMessage(reader, version));
                    }
                    RemainingPartitions = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version3)
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
        writer.WriteShort((short)ErrorCode);
        if (version >= ApiVersions.Version3)
        {
            writer.WriteVarUInt(RemainingPartitions.Count + 1);
            foreach (var element in RemainingPartitions)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(RemainingPartitions.Count);
            foreach (var element in RemainingPartitions)
            {
                element.Write(writer, version);
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
        return ReferenceEquals(this, obj) || obj is ControlledShutdownResponseMessage other && Equals(other);
    }

    public bool Equals(ControlledShutdownResponseMessage? other)
    {
        return true;
    }

    public sealed class RemainingPartitionMessage: IMessage, IEquatable<RemainingPartitionMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The index of the partition.
        /// </summary>
        public int PartitionIndex { get; set; } = 0;

        public RemainingPartitionMessage()
        {
        }

        public RemainingPartitionMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of RemainingPartitionMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version3)
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
            PartitionIndex = reader.ReadInt();
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version3)
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
                var stringBytes = Encoding.UTF8.GetBytes(TopicName);
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
            writer.WriteInt(PartitionIndex);
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
            return ReferenceEquals(this, obj) || obj is RemainingPartitionMessage other && Equals(other);
        }

        public bool Equals(RemainingPartitionMessage? other)
        {
            return true;
        }
    }

    public sealed class RemainingPartitionCollection: HashSet<RemainingPartitionMessage>
    {
        public RemainingPartitionCollection()
        {
        }

        public RemainingPartitionCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
