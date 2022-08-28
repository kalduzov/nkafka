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

public sealed class FindCoordinatorRequestMessage: IRequestMessage, IEquatable<FindCoordinatorRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version4;

    public ApiKeys ApiKey => ApiKeys.FindCoordinator;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The coordinator key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The coordinator key type. (Group, transaction, etc.)
    /// </summary>
    public sbyte KeyType { get; set; } = 0;

    /// <summary>
    /// The coordinator keys.
    /// </summary>
    public List<string> CoordinatorKeys { get; set; } = new ();

    public FindCoordinatorRequestMessage()
    {
    }

    public FindCoordinatorRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        if (version <= ApiVersions.Version3)
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
        else
        {
            Key = string.Empty;
        }
        if (version >= ApiVersions.Version1)
        {
            KeyType = reader.ReadSByte();
        }
        else
        {
            KeyType = 0;
        }
        if (version >= ApiVersions.Version4)
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field CoordinatorKeys was serialized as null");
            }
            else
            {
                var newCollection = new List<string>(arrayLength);
                for (var i = 0; i< arrayLength; i++)
                {
                    int length;
                    length = reader.ReadVarUInt() - 1;
                    if (length < 0)
                    {
                        throw new Exception("non-nullable field CoordinatorKeys element was serialized as null");
                    }
                    else if (length > 0x7fff)
                    {
                        throw new Exception($"string field CoordinatorKeys element had invalid length {length}");
                    }
                    else
                    {
                        newCollection.Add(reader.ReadString(length));
                    }
                }
                CoordinatorKeys = newCollection;
            }
        }
        else
        {
            CoordinatorKeys = new ();
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
        if (version <= ApiVersions.Version3)
        {
            {
                var stringBytes = Encoding.UTF8.GetBytes(Key);
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
            if (!Key.Equals(string.Empty))
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default Key at version {version}");
            }
        }
        if (version >= ApiVersions.Version1)
        {
            writer.WriteSByte(KeyType);
        }
        else
        {
            if (KeyType != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default KeyType at version {version}");
            }
        }
        if (version >= ApiVersions.Version4)
        {
            writer.WriteVarUInt(CoordinatorKeys.Count + 1);
            foreach (var element in CoordinatorKeys)
            {
                {
                    var stringBytes = Encoding.UTF8.GetBytes(element);
                    writer.WriteVarUInt(stringBytes.Length + 1);
                    writer.WriteBytes(stringBytes);
                }
            }
        }
        else
        {
            if (CoordinatorKeys.Count != 0)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default CoordinatorKeys at version {version}");
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
        return ReferenceEquals(this, obj) || obj is FindCoordinatorRequestMessage other && Equals(other);
    }

    public bool Equals(FindCoordinatorRequestMessage? other)
    {
        return true;
    }
}
