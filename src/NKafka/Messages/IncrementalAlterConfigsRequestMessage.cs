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

public sealed class IncrementalAlterConfigsRequestMessage: RequestMessage
{
    /// <summary>
    /// The incremental updates for each resource.
    /// </summary>
    public List<AlterConfigsResourceMessage> ResourcesMessage { get; set; } = new ();

    /// <summary>
    /// True if we should validate the request, but not change the configurations.
    /// </summary>
    public bool ValidateOnlyMessage { get; set; } = false;

    public IncrementalAlterConfigsRequestMessage()
    {
        ApiKey = ApiKeys.IncrementalAlterConfigs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public IncrementalAlterConfigsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.IncrementalAlterConfigs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class AlterConfigsResourceMessage: Message
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        public Dictionary<sbyte,> ResourceTypeMessage { get; set; } = 0;

        /// <summary>
        /// The resource name.
        /// </summary>
        public Dictionary<string,> ResourceNameMessage { get; set; } = "";

        /// <summary>
        /// The configurations.
        /// </summary>
        public List<AlterableConfigMessage> ConfigsMessage { get; set; } = new ();

        public AlterConfigsResourceMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public AlterConfigsResourceMessage(BufferReader reader, ApiVersions version)
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

    public sealed class AlterableConfigMessage: Message
    {
        /// <summary>
        /// The configuration key name.
        /// </summary>
        public Dictionary<string,> NameMessage { get; set; } = "";

        /// <summary>
        /// The type (Set, Delete, Append, Subtract) of operation.
        /// </summary>
        public Dictionary<sbyte,> ConfigOperationMessage { get; set; } = 0;

        /// <summary>
        /// The value to set for the configuration key.
        /// </summary>
        public Dictionary<string,> ValueMessage { get; set; } = "";

        public AlterableConfigMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public AlterableConfigMessage(BufferReader reader, ApiVersions version)
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
