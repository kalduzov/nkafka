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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class IncrementalAlterConfigsRequestMessage: RequestMessage
{
    /// <summary>
    /// The incremental updates for each resource.
    /// </summary>
    public List<AlterConfigsResource> Resources { get; set; } = new();

    /// <summary>
    /// True if we should validate the request, but not change the configurations.
    /// </summary>
    public bool ValidateOnly { get; set; } = false;

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

    public class AlterConfigsResource: Message
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; } = 0;

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; } = null!;

        /// <summary>
        /// The configurations.
        /// </summary>
        public List<AlterableConfig> Configs { get; set; } = new();

        public AlterConfigsResource()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public AlterConfigsResource(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class AlterableConfig: Message
    {
        /// <summary>
        /// The configuration key name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The type (Set, Delete, Append, Subtract) of operation.
        /// </summary>
        public sbyte ConfigOperation { get; set; } = 0;

        /// <summary>
        /// The value to set for the configuration key.
        /// </summary>
        public string Value { get; set; } = null!;

        public AlterableConfig()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public AlterableConfig(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}