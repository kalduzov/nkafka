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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class ApiVersionsResponseMessage: ResponseMessage
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
    }

    public sealed class ApiVersionMessage: Message
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
        }
    }

    public sealed class ApiVersionCollection: HashSet<ApiVersionMessage>
    {
    }

    public sealed class SupportedFeatureKeyMessage: Message
    {
        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string Name { get; set; } = "";

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
        }
    }

    public sealed class SupportedFeatureKeyCollection: HashSet<SupportedFeatureKeyMessage>
    {
    }

    public sealed class FinalizedFeatureKeyMessage: Message
    {
        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string Name { get; set; } = "";

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
        }
    }

    public sealed class FinalizedFeatureKeyCollection: HashSet<FinalizedFeatureKeyMessage>
    {
    }
}
