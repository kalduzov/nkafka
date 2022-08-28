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

public sealed class ApiVersionsResponseMessage: ResponseMessage, IEquatable<ApiVersionsResponseMessage>
{
    /// <summary>
    /// The top-level error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The APIs supported by the broker.
    /// </summary>
    public ApiVersionCollection ApiKeys { get; set; } = new ();

    /// <summary>
    /// Features supported by the broker.
    /// </summary>
    public SupportedFeatureKeyCollection SupportedFeatures { get; set; } = new ();

    /// <summary>
    /// The monotonically increasing epoch for the finalized features information. Valid values are >= 0. A value of -1 is special and represents unknown epoch.
    /// </summary>
    public long FinalizedFeaturesEpoch { get; set; } = -1;

    /// <summary>
    /// List of cluster-wide finalized features. The information is valid only if FinalizedFeaturesEpoch >= 0.
    /// </summary>
    public FinalizedFeatureKeyCollection FinalizedFeatures { get; set; } = new ();

    public ApiVersionsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public ApiVersionsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteShort(ErrorCode);
        if (version >= ApiVersions.Version3)
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
        if (version >= ApiVersions.Version1)
        {
            writer.WriteInt(ThrottleTimeMs);
        }
        if (version >= ApiVersions.Version3)
        {
            if (SupportedFeatures.Count != 0)
            {
                numTaggedFields++;
            }
        }
        if (version >= ApiVersions.Version3)
        {
            if (FinalizedFeaturesEpoch != -1)
            {
                numTaggedFields++;
            }
        }
        if (version >= ApiVersions.Version3)
        {
            if (FinalizedFeatures.Count != 0)
            {
                numTaggedFields++;
            }
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        if (version >= ApiVersions.Version3)
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

    public sealed class ApiVersionMessage: Message, IEquatable<ApiVersionMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public ApiVersionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            writer.WriteShort(ApiKey);
            writer.WriteShort(MinVersion);
            writer.WriteShort(MaxVersion);
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
            return ReferenceEquals(this, obj) || obj is ApiVersionMessage other && Equals(other);
        }

        public bool Equals(ApiVersionMessage? other)
        {
            return true;
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

    public sealed class SupportedFeatureKeyMessage: Message, IEquatable<SupportedFeatureKeyMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public SupportedFeatureKeyMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version3)
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

    public sealed class FinalizedFeatureKeyMessage: Message, IEquatable<FinalizedFeatureKeyMessage>
    {
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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public FinalizedFeatureKeyMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            if (version < ApiVersions.Version3)
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
