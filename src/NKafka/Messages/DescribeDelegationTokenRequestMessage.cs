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

public sealed class DescribeDelegationTokenRequestMessage: IRequestMessage, IEquatable<DescribeDelegationTokenRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

    public ApiKeys ApiKey => ApiKeys.DescribeDelegationToken;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// Each owner that we want to describe delegation tokens for, or null to describe all tokens.
    /// </summary>
    public List<DescribeDelegationTokenOwnerMessage> Owners { get; set; } = new ();

    public DescribeDelegationTokenRequestMessage()
    {
    }

    public DescribeDelegationTokenRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        {
            if (version >= ApiVersions.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    Owners = null;
                }
                else
                {
                    var newCollection = new List<DescribeDelegationTokenOwnerMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new DescribeDelegationTokenOwnerMessage(reader, version));
                    }
                    Owners = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    Owners = null;
                }
                else
                {
                    var newCollection = new List<DescribeDelegationTokenOwnerMessage>(arrayLength);
                    for (var i = 0; i< arrayLength; i++)
                    {
                        newCollection.Add(new DescribeDelegationTokenOwnerMessage(reader, version));
                    }
                    Owners = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersions.Version2)
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
        if (version >= ApiVersions.Version2)
        {
            if (Owners is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteVarUInt(Owners.Count + 1);
                foreach (var element in Owners)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (Owners is null)
            {
                writer.WriteInt(-1);
            }
            else
            {
                writer.WriteInt(Owners.Count);
                foreach (var element in Owners)
                {
                    element.Write(writer, version);
                }
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version2)
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
        return ReferenceEquals(this, obj) || obj is DescribeDelegationTokenRequestMessage other && Equals(other);
    }

    public bool Equals(DescribeDelegationTokenRequestMessage? other)
    {
        return true;
    }

    public sealed class DescribeDelegationTokenOwnerMessage: IMessage, IEquatable<DescribeDelegationTokenOwnerMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version3;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The owner principal type.
        /// </summary>
        public string PrincipalType { get; set; } = string.Empty;

        /// <summary>
        /// The owner principal name.
        /// </summary>
        public string PrincipalName { get; set; } = string.Empty;

        public DescribeDelegationTokenOwnerMessage()
        {
        }

        public DescribeDelegationTokenOwnerMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of DescribeDelegationTokenOwnerMessage");
            }
            {
                int length;
                if (version >= ApiVersions.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field PrincipalType was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field PrincipalType had invalid length {length}");
                }
                else
                {
                    PrincipalType = reader.ReadString(length);
                }
            }
            {
                int length;
                if (version >= ApiVersions.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field PrincipalName was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field PrincipalName had invalid length {length}");
                }
                else
                {
                    PrincipalName = reader.ReadString(length);
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersions.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(PrincipalType);
                if (version >= ApiVersions.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            {
                var stringBytes = Encoding.UTF8.GetBytes(PrincipalName);
                if (version >= ApiVersions.Version2)
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
            if (version >= ApiVersions.Version2)
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
            return ReferenceEquals(this, obj) || obj is DescribeDelegationTokenOwnerMessage other && Equals(other);
        }

        public bool Equals(DescribeDelegationTokenOwnerMessage? other)
        {
            return true;
        }
    }
}
