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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class ApiVersionsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top-level error code.
    /// </summary>
    public short ErrorCode { get; set; }

    /// <summary>
    /// The APIs supported by the broker.
    /// </summary>
    public IReadOnlyCollection<ApiVersionMessage> ApiKeys { get; set; }

    /// <summary>
    /// Features supported by the broker.
    /// </summary>
    public IReadOnlyCollection<SupportedFeatureKeyMessage>? SupportedFeatures { get; set; }

    /// <summary>
    /// The monotonically increasing epoch for the finalized features information. Valid values are >= 0. A value of -1 is special and represents unknown epoch.
    /// </summary>
    public long? FinalizedFeaturesEpoch { get; set; } = -1;

    /// <summary>
    /// List of cluster-wide finalized features. The information is valid only if FinalizedFeaturesEpoch >= 0.
    /// </summary>
    public IReadOnlyCollection<FinalizedFeatureKeyMessage>? FinalizedFeatures { get; set; }

    public ApiVersionsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public ApiVersionsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version3)
        {
        }
        else //no flexible version
        {
        }

        //flexible version
        if (Version >= ApiVersions.Version3)
        {
        }
        else //no flexible version
        {
        }

        //flexible version
        if (Version >= ApiVersions.Version3)
        {
        }
        else //no flexible version
        {
        }

    }

    public class ApiVersionMessage: Message
    {
        /// <summary>
        /// The API index.
        /// </summary>
        public short ApiKey { get; set; }

        /// <summary>
        /// The minimum supported version, inclusive.
        /// </summary>
        public short MinVersion { get; set; }

        /// <summary>
        /// The maximum supported version, inclusive.
        /// </summary>
        public short MaxVersion { get; set; }

        public ApiVersionMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public ApiVersionMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class SupportedFeatureKeyMessage: Message
    {
        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimum supported version for the feature.
        /// </summary>
        public short MinVersion { get; set; }

        /// <summary>
        /// The maximum supported version for the feature.
        /// </summary>
        public short MaxVersion { get; set; }

        public SupportedFeatureKeyMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public SupportedFeatureKeyMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class FinalizedFeatureKeyMessage: Message
    {
        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cluster-wide finalized max version level for the feature.
        /// </summary>
        public short MaxVersionLevel { get; set; }

        /// <summary>
        /// The cluster-wide finalized min version level for the feature.
        /// </summary>
        public short MinVersionLevel { get; set; }

        public FinalizedFeatureKeyMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public FinalizedFeatureKeyMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}