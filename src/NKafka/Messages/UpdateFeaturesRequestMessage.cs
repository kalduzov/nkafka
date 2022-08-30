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

public sealed class UpdateFeaturesRequestMessage: IRequestMessage, IEquatable<UpdateFeaturesRequestMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public ApiKeys ApiKey => ApiKeys.UpdateFeatures;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// How long to wait in milliseconds before timing out the request.
    /// </summary>
    public int timeoutMs { get; set; } = 60000;

    /// <summary>
    /// The list of updates to finalized features.
    /// </summary>
    public FeatureUpdateKeyCollection FeatureUpdates { get; set; } = new ();

    /// <summary>
    /// True if we should validate the request, but not perform the upgrade or downgrade.
    /// </summary>
    public bool ValidateOnly { get; set; } = false;

    public UpdateFeaturesRequestMessage()
    {
    }

    public UpdateFeaturesRequestMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        timeoutMs = reader.ReadInt();
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field FeatureUpdates was serialized as null");
            }
            else
            {
                var newCollection = new FeatureUpdateKeyCollection(arrayLength);
                for (var i = 0; i < arrayLength; i++)
                {
                    newCollection.Add(new FeatureUpdateKeyMessage(reader, version));
                }
                FeatureUpdates = newCollection;
            }
        }
        if (version >= ApiVersion.Version1)
        {
            ValidateOnly = reader.ReadByte() != 0;
        }
        else
        {
            ValidateOnly = false;
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
        writer.WriteInt(timeoutMs);
        writer.WriteVarUInt(FeatureUpdates.Count + 1);
        foreach (var element in FeatureUpdates)
        {
            element.Write(writer, version);
        }
        if (version >= ApiVersion.Version1)
        {
            writer.WriteBool(ValidateOnly);
        }
        else
        {
            if (ValidateOnly)
            {
                throw new UnsupportedVersionException($"Attempted to write a non-default ValidateOnly at version {version}");
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is UpdateFeaturesRequestMessage other && Equals(other);
    }

    public bool Equals(UpdateFeaturesRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, timeoutMs, FeatureUpdates, ValidateOnly);
        return hashCode;
    }

    public override string ToString()
    {
        return "UpdateFeaturesRequestMessage("
            + "timeoutMs=" + timeoutMs
            + ", ValidateOnly=" + (ValidateOnly ? "true" : "false")
            + ")";
    }

    public sealed class FeatureUpdateKeyMessage: IMessage, IEquatable<FeatureUpdateKeyMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version1;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the finalized feature to be updated.
        /// </summary>
        public string Feature { get; set; } = string.Empty;

        /// <summary>
        /// The new maximum version level for the finalized feature. A value >= 1 is valid. A value < 1, is special, and can be used to request the deletion of the finalized feature.
        /// </summary>
        public short MaxVersionLevel { get; set; } = 0;

        /// <summary>
        /// DEPRECATED in version 1 (see DowngradeType). When set to true, the finalized feature version level is allowed to be downgraded/deleted. The downgrade request will fail if the new maximum version level is a value that's not lower than the existing maximum finalized version level.
        /// </summary>
        public bool AllowDowngrade { get; set; } = false;

        /// <summary>
        /// Determine which type of upgrade will be performed: 1 will perform an upgrade only (default), 2 is safe downgrades only (lossless), 3 is unsafe downgrades (lossy).
        /// </summary>
        public sbyte UpgradeType { get; set; } = 1;

        public FeatureUpdateKeyMessage()
        {
        }

        public FeatureUpdateKeyMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version1)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of FeatureUpdateKeyMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field Feature was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Feature had invalid length {length}");
                }
                else
                {
                    Feature = reader.ReadString(length);
                }
            }
            MaxVersionLevel = reader.ReadShort();
            if (version <= ApiVersion.Version0)
            {
                AllowDowngrade = reader.ReadByte() != 0;
            }
            else
            {
                AllowDowngrade = false;
            }
            if (version >= ApiVersion.Version1)
            {
                UpgradeType = reader.ReadSByte();
            }
            else
            {
                UpgradeType = 1;
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Feature);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteShort(MaxVersionLevel);
            if (version <= ApiVersion.Version0)
            {
                writer.WriteBool(AllowDowngrade);
            }
            else
            {
                if (AllowDowngrade)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default AllowDowngrade at version {version}");
                }
            }
            if (version >= ApiVersion.Version1)
            {
                writer.WriteSByte(UpgradeType);
            }
            else
            {
                if (UpgradeType != 1)
                {
                    throw new UnsupportedVersionException($"Attempted to write a non-default UpgradeType at version {version}");
                }
            }
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is FeatureUpdateKeyMessage other && Equals(other);
        }

        public bool Equals(FeatureUpdateKeyMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, Feature);
            return hashCode;
        }

        public override string ToString()
        {
            return "FeatureUpdateKeyMessage("
                + ", MaxVersionLevel=" + MaxVersionLevel
                + ", AllowDowngrade=" + (AllowDowngrade ? "true" : "false")
                + ", UpgradeType=" + UpgradeType
                + ")";
        }
    }

    public sealed class FeatureUpdateKeyCollection: HashSet<FeatureUpdateKeyMessage>
    {
        public FeatureUpdateKeyCollection()
        {
        }

        public FeatureUpdateKeyCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
