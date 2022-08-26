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

public sealed class UpdateFeaturesResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top-level error code, or `0` if there was no top-level error.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The top-level error message, or `null` if there was no top-level error.
    /// </summary>
    public string ErrorMessageMessage { get; set; } = "";

    /// <summary>
    /// Results for each feature update.
    /// </summary>
    public List<UpdatableFeatureResultMessage> ResultsMessage { get; set; } = new ();

    public UpdateFeaturesResponseMessage()
    {
        ApiKey = ApiKeys.UpdateFeatures;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public UpdateFeaturesResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.UpdateFeatures;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class UpdatableFeatureResultMessage: Message
    {
        /// <summary>
        /// The name of the finalized feature.
        /// </summary>
        public Dictionary<string,> FeatureMessage { get; set; } = "";

        /// <summary>
        /// The feature update error code or `0` if the feature update succeeded.
        /// </summary>
        public Dictionary<short,> ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The feature update error, or `null` if the feature update succeeded.
        /// </summary>
        public Dictionary<string,> ErrorMessageMessage { get; set; } = "";

        public UpdatableFeatureResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public UpdatableFeatureResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }
}
