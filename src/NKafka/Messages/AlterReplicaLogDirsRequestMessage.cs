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

public sealed class AlterReplicaLogDirsRequestMessage: IRequestMessage, IEquatable<AlterReplicaLogDirsRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version2;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.AlterReplicaLogDirs;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The alterations to make for each directory.
    /// </summary>
    public AlterReplicaLogDirCollection Dirs { get; set; } = new ();

    public AlterReplicaLogDirsRequestMessage()
    {
    }

    public AlterReplicaLogDirsRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        {
            if (version >= ApiVersion.Version2)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Dirs was serialized as null");
                }
                else
                {
                    var newCollection = new AlterReplicaLogDirCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new AlterReplicaLogDirMessage(reader, version));
                    }
                    Dirs = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Dirs was serialized as null");
                }
                else
                {
                    var newCollection = new AlterReplicaLogDirCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new AlterReplicaLogDirMessage(reader, version));
                    }
                    Dirs = newCollection;
                }
            }
        }
        UnknownTaggedFields = null;
        if (version >= ApiVersion.Version2)
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
        if (version >= ApiVersion.Version2)
        {
            writer.WriteVarUInt(Dirs.Count + 1);
            foreach (var element in Dirs)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(Dirs.Count);
            foreach (var element in Dirs)
            {
                element.Write(writer, version);
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version2)
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
        return ReferenceEquals(this, obj) || obj is AlterReplicaLogDirsRequestMessage other && Equals(other);
    }

    public bool Equals(AlterReplicaLogDirsRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, Dirs);
        return hashCode;
    }

    public override string ToString()
    {
        return "AlterReplicaLogDirsRequestMessage("
            + ")";
    }

    public sealed class AlterReplicaLogDirMessage: IMessage, IEquatable<AlterReplicaLogDirMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version2;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The absolute directory path.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// The topics to add to the directory.
        /// </summary>
        public AlterReplicaLogDirTopicCollection Topics { get; set; } = new ();

        public AlterReplicaLogDirMessage()
        {
        }

        public AlterReplicaLogDirMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version2)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of AlterReplicaLogDirMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version2)
                {
                    length = reader.ReadVarUInt() - 1;
                }
                else
                {
                    length = reader.ReadShort();
                }
                if (length < 0)
                {
                    throw new Exception("non-nullable field Path was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Path had invalid length {length}");
                }
                else
                {
                    Path = reader.ReadString(length);
                }
            }
            {
                if (version >= ApiVersion.Version2)
                {
                    int arrayLength;
                    arrayLength = reader.ReadVarUInt() - 1;
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Topics was serialized as null");
                    }
                    else
                    {
                        var newCollection = new AlterReplicaLogDirTopicCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AlterReplicaLogDirTopicMessage(reader, version));
                        }
                        Topics = newCollection;
                    }
                }
                else
                {
                    int arrayLength;
                    arrayLength = reader.ReadInt();
                    if (arrayLength < 0)
                    {
                        throw new Exception("non-nullable field Topics was serialized as null");
                    }
                    else
                    {
                        var newCollection = new AlterReplicaLogDirTopicCollection(arrayLength);
                        for (var i = 0; i < arrayLength; i++)
                        {
                            newCollection.Add(new AlterReplicaLogDirTopicMessage(reader, version));
                        }
                        Topics = newCollection;
                    }
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version2)
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
                var stringBytes = Encoding.UTF8.GetBytes(Path);
                if (version >= ApiVersion.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version2)
            {
                writer.WriteVarUInt(Topics.Count + 1);
                foreach (var element in Topics)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Topics.Count);
                foreach (var element in Topics)
                {
                    element.Write(writer, version);
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version2)
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
            return ReferenceEquals(this, obj) || obj is AlterReplicaLogDirMessage other && Equals(other);
        }

        public bool Equals(AlterReplicaLogDirMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Path);
            return hashCode;
        }

        public override string ToString()
        {
            return "AlterReplicaLogDirMessage("
                + ")";
        }
    }

    public sealed class AlterReplicaLogDirTopicMessage: IMessage, IEquatable<AlterReplicaLogDirTopicMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version2;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The partition indexes.
        /// </summary>
        public List<int> Partitions { get; set; } = new ();

        public AlterReplicaLogDirTopicMessage()
        {
        }

        public AlterReplicaLogDirTopicMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version2)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of AlterReplicaLogDirTopicMessage");
            }
            {
                int length;
                if (version >= ApiVersion.Version2)
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
                int arrayLength;
                if (version >= ApiVersion.Version2)
                {
                    arrayLength = reader.ReadVarUInt() - 1;
                }
                else
                {
                    arrayLength = reader.ReadInt();
                }
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field Partitions was serialized as null");
                }
                else
                {
                    var newCollection = new List<int>(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(reader.ReadInt());
                    }
                    Partitions = newCollection;
                }
            }
            UnknownTaggedFields = null;
            if (version >= ApiVersion.Version2)
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
                if (version >= ApiVersion.Version2)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (version >= ApiVersion.Version2)
            {
                writer.WriteVarUInt(Partitions.Count + 1);
            }
            else
            {
                writer.WriteInt(Partitions.Count);
            }
            foreach (var element in Partitions)
            {
                writer.WriteInt(element);
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            if (version >= ApiVersion.Version2)
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
            return ReferenceEquals(this, obj) || obj is AlterReplicaLogDirTopicMessage other && Equals(other);
        }

        public bool Equals(AlterReplicaLogDirTopicMessage? other)
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
            return "AlterReplicaLogDirTopicMessage("
                + ")";
        }
    }

    public sealed class AlterReplicaLogDirTopicCollection: HashSet<AlterReplicaLogDirTopicMessage>
    {
        public AlterReplicaLogDirTopicCollection()
        {
        }

        public AlterReplicaLogDirTopicCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class AlterReplicaLogDirCollection: HashSet<AlterReplicaLogDirMessage>
    {
        public AlterReplicaLogDirCollection()
        {
        }

        public AlterReplicaLogDirCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
