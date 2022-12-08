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

using System.Text;

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;

namespace NKafka.Messages;

public sealed class ApiVersionsResponseMessage: IResponseMessage, IEquatable<ApiVersionsResponseMessage>
{
    public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

    public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

    public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

    public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The top-level error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// The APIs supported by the broker.
    /// </summary>
    public ApiVersionCollection ApiKeys { get; set; } = new();

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// Features supported by the broker.
    /// </summary>
    public SupportedFeatureKeyCollection SupportedFeatures { get; set; } = new();

    /// <summary>
    /// The monotonically increasing epoch for the finalized features information. Valid values are >= 0. A value of -1 is special and represents unknown epoch.
    /// </summary>
    public long FinalizedFeaturesEpoch { get; set; } = -1;

    /// <summary>
    /// List of cluster-wide finalized features. The information is valid only if FinalizedFeaturesEpoch >= 0.
    /// </summary>
    public FinalizedFeatureKeyCollection FinalizedFeatures { get; set; } = new();

    public ApiVersionsResponseMessage()
    {
    }

    public ApiVersionsResponseMessage(BufferReader reader, ApiVersion version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersion version)
    {
        ErrorCode = reader.ReadShort();
        {
            if (version >= ApiVersion.Version3)
            {
                int arrayLength;
                arrayLength = reader.ReadVarUInt() - 1;
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field ApiKeys was serialized as null");
                }
                else
                {
                    var newCollection = new ApiVersionCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ApiVersionMessage(reader, version));
                    }
                    ApiKeys = newCollection;
                }
            }
            else
            {
                int arrayLength;
                arrayLength = reader.ReadInt();
                if (arrayLength < 0)
                {
                    throw new Exception("non-nullable field ApiKeys was serialized as null");
                }
                else
                {
                    var newCollection = new ApiVersionCollection(arrayLength);
                    for (var i = 0; i < arrayLength; i++)
                    {
                        newCollection.Add(new ApiVersionMessage(reader, version));
                    }
                    ApiKeys = newCollection;
                }
            }
        }
        if (version >= ApiVersion.Version1)
        {
            ThrottleTimeMs = reader.ReadInt();
        }
        else
        {
            ThrottleTimeMs = 0;
        }
        {
            SupportedFeatures = new();
        }
        FinalizedFeaturesEpoch = -1;
        {
            FinalizedFeatures = new();
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
                    case 0:
                        {
                            int arrayLength;
                            arrayLength = reader.ReadVarUInt() - 1;
                            if (arrayLength < 0)
                            {
                                throw new Exception("non-nullable field SupportedFeatures was serialized as null");
                            }
                            else
                            {
                                var newCollection = new SupportedFeatureKeyCollection(arrayLength);
                                for (var i = 0; i < arrayLength; i++)
                                {
                                    newCollection.Add(new SupportedFeatureKeyMessage(reader, version));
                                }
                                SupportedFeatures = newCollection;
                            }
                            break;
                        }
                    case 1:
                        {
                            FinalizedFeaturesEpoch = reader.ReadLong();
                            break;
                        }
                    case 2:
                        {
                            int arrayLength;
                            arrayLength = reader.ReadVarUInt() - 1;
                            if (arrayLength < 0)
                            {
                                throw new Exception("non-nullable field FinalizedFeatures was serialized as null");
                            }
                            else
                            {
                                var newCollection = new FinalizedFeatureKeyCollection(arrayLength);
                                for (var i = 0; i < arrayLength; i++)
                                {
                                    newCollection.Add(new FinalizedFeatureKeyMessage(reader, version));
                                }
                                FinalizedFeatures = newCollection;
                            }
                            break;
                        }
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
        writer.WriteShort((short)ErrorCode);
        if (version >= ApiVersion.Version3)
        {
            writer.WriteVarUInt(ApiKeys.Count + 1);
            foreach (var element in ApiKeys)
            {
                element.Write(writer, version);
            }
        }
        else
        {
            writer.WriteInt(ApiKeys.Count);
            foreach (var element in ApiKeys)
            {
                element.Write(writer, version);
            }
        }
        if (version >= ApiVersion.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersion.Version3)
        {
            if (SupportedFeatures.Count != 0)
            {
                numTaggedFields++;
            }
        }
        if (version >= ApiVersion.Version3)
        {
            if (FinalizedFeaturesEpoch != -1)
            {
                numTaggedFields++;
            }
        }
        if (version >= ApiVersion.Version3)
        {
            if (FinalizedFeatures.Count != 0)
            {
                numTaggedFields++;
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersion.Version3)
        {
            writer.WriteVarUInt(numTaggedFields);
            {
                if (SupportedFeatures.Count != 0)
                {
                    writer.WriteVarUInt(0);
                    writer.WriteVarUInt(SupportedFeatures.Count + 1);
                    foreach (var element in SupportedFeatures)
                    {
                        element.Write(writer, version);
                    }
                }
            }
            {
                if (FinalizedFeaturesEpoch != -1)
                {
                    writer.WriteVarUInt(1);
                    writer.WriteVarUInt(8);
                    writer.WriteLong(FinalizedFeaturesEpoch);
                }
            }
            {
                if (FinalizedFeatures.Count != 0)
                {
                    writer.WriteVarUInt(2);
                    writer.WriteVarUInt(FinalizedFeatures.Count + 1);
                    foreach (var element in FinalizedFeatures)
                    {
                        element.Write(writer, version);
                    }
                }
            }
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
        return ReferenceEquals(this, obj) || obj is ApiVersionsResponseMessage other && Equals(other);
    }

    public bool Equals(ApiVersionsResponseMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, ErrorCode, ApiKeys, ThrottleTimeMs, SupportedFeatures, FinalizedFeaturesEpoch, FinalizedFeatures);
        return hashCode;
    }

    public override string ToString()
    {
        return "ApiVersionsResponseMessage("
            + "ErrorCode=" + ErrorCode
            + ", ThrottleTimeMs=" + ThrottleTimeMs
            + ", FinalizedFeaturesEpoch=" + FinalizedFeaturesEpoch
            + ")";
    }

    public sealed class ApiVersionMessage: IMessage, IEquatable<ApiVersionMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The API index.
        /// </summary>
        public short ApiKey { get; set; } = 0;

        /// <summary>
        /// The minimum supported version, inclusive.
        /// </summary>
        public short MinVersion { get; set; } = 0;

        /// <summary>
        /// The maximum supported version, inclusive.
        /// </summary>
        public short MaxVersion { get; set; } = 0;

        public ApiVersionMessage()
        {
        }

        public ApiVersionMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of ApiVersionMessage");
            }
            ApiKey = reader.ReadShort();
            MinVersion = reader.ReadShort();
            MaxVersion = reader.ReadShort();
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
            writer.WriteShort(ApiKey);
            writer.WriteShort(MinVersion);
            writer.WriteShort(MaxVersion);
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
            return ReferenceEquals(this, obj) || obj is ApiVersionMessage other && Equals(other);
        }

        public bool Equals(ApiVersionMessage? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = HashCode.Combine(hashCode, ApiKey);
            return hashCode;
        }

        public override string ToString()
        {
            return "ApiVersionMessage("
                + "ApiKey=" + ApiKey
                + ", MinVersion=" + MinVersion
                + ", MaxVersion=" + MaxVersion
                + ")";
        }
    }

    public sealed class ApiVersionCollection: HashSet<ApiVersionMessage>
    {
        public ApiVersionCollection()
        {
        }

        public ApiVersionCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class SupportedFeatureKeyMessage: IMessage, IEquatable<SupportedFeatureKeyMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The minimum supported version for the feature.
        /// </summary>
        public short MinVersion { get; set; } = 0;

        /// <summary>
        /// The maximum supported version for the feature.
        /// </summary>
        public short MaxVersion { get; set; } = 0;

        public SupportedFeatureKeyMessage()
        {
        }

        public SupportedFeatureKeyMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of SupportedFeatureKeyMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
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
            MinVersion = reader.ReadShort();
            MaxVersion = reader.ReadShort();
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
            if (version < ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of SupportedFeatureKeyMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteShort(MinVersion);
            writer.WriteShort(MaxVersion);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is SupportedFeatureKeyMessage other && Equals(other);
        }

        public bool Equals(SupportedFeatureKeyMessage? other)
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
            return "SupportedFeatureKeyMessage("
                + ", MinVersion=" + MinVersion
                + ", MaxVersion=" + MaxVersion
                + ")";
        }
    }

    public sealed class SupportedFeatureKeyCollection: HashSet<SupportedFeatureKeyMessage>
    {
        public SupportedFeatureKeyCollection()
        {
        }

        public SupportedFeatureKeyCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class FinalizedFeatureKeyMessage: IMessage, IEquatable<FinalizedFeatureKeyMessage>
    {
        public const ApiVersion LOWEST_SUPPORTED_VERSION = ApiVersion.Version0;

        public const ApiVersion HIGHEST_SUPPORTED_VERSION = ApiVersion.Version3;

        public ApiVersion LowestSupportedVersion => LOWEST_SUPPORTED_VERSION;

        public ApiVersion HighestSupportedVersion => HIGHEST_SUPPORTED_VERSION;

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The cluster-wide finalized max version level for the feature.
        /// </summary>
        public short MaxVersionLevel { get; set; } = 0;

        /// <summary>
        /// The cluster-wide finalized min version level for the feature.
        /// </summary>
        public short MinVersionLevel { get; set; } = 0;

        public FinalizedFeatureKeyMessage()
        {
        }

        public FinalizedFeatureKeyMessage(BufferReader reader, ApiVersion version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersion version)
        {
            if (version > ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of FinalizedFeatureKeyMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
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
            MaxVersionLevel = reader.ReadShort();
            MinVersionLevel = reader.ReadShort();
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
            if (version < ApiVersion.Version3)
            {
                throw new UnsupportedVersionException($"Can't write version {version} of FinalizedFeatureKeyMessage");
            }
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteShort(MaxVersionLevel);
            writer.WriteShort(MinVersionLevel);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is FinalizedFeatureKeyMessage other && Equals(other);
        }

        public bool Equals(FinalizedFeatureKeyMessage? other)
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
            return "FinalizedFeatureKeyMessage("
                + ", MaxVersionLevel=" + MaxVersionLevel
                + ", MinVersionLevel=" + MinVersionLevel
                + ")";
        }
    }

    public sealed class FinalizedFeatureKeyCollection: HashSet<FinalizedFeatureKeyMessage>
    {
        public FinalizedFeatureKeyCollection()
        {
        }

        public FinalizedFeatureKeyCollection(int capacity)
            : base(capacity)
        {
        }
    }
}