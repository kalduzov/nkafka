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

public sealed class DescribeConfigsRequestMessage: RequestMessage
{
    /// <summary>
    /// The resources whose configurations we want to describe.
    /// </summary>
    public List<DescribeConfigsResourceMessage> ResourcesMessage { get; set; } = new ();

    /// <summary>
    /// True if we should include all synonyms.
    /// </summary>
    public bool IncludeSynonymsMessage { get; set; } = false;

    /// <summary>
    /// True if we should include configuration documentation.
    /// </summary>
    public bool IncludeDocumentationMessage { get; set; } = false;

    public DescribeConfigsRequestMessage()
    {
        ApiKey = ApiKeys.DescribeConfigs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public DescribeConfigsRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeConfigs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class DescribeConfigsResourceMessage: Message
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceTypeMessage { get; set; } = 0;

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceNameMessage { get; set; } = "";

        /// <summary>
        /// The configuration keys to list, or null to list all configuration keys.
        /// </summary>
        public List<string> ConfigurationKeysMessage { get; set; } = new ();

        public DescribeConfigsResourceMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public DescribeConfigsResourceMessage(BufferReader reader, ApiVersions version)
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
