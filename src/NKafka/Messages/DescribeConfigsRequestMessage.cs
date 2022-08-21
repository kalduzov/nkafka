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

public partial class DescribeConfigsRequestMessage: RequestMessage
{
    /// <summary>
    /// The resources whose configurations we want to describe.
    /// </summary>
    public IReadOnlyCollection<DescribeConfigsResourceMessage> Resources { get; set; }

    /// <summary>
    /// True if we should include all synonyms.
    /// </summary>
    public bool IncludeSynonyms { get; set; } = false;

    /// <summary>
    /// True if we should include configuration documentation.
    /// </summary>
    public bool IncludeDocumentation { get; set; } = false;

    public DescribeConfigsRequestMessage()
    {
        ApiKey = ApiKeys.DescribeConfigs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version4)
        {
        }
        else //no flexible version
        {
        }

    }

    public class DescribeConfigsResourceMessage: Message
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; }

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// The configuration keys to list, or null to list all configuration keys.
        /// </summary>
        public IReadOnlyCollection<string> ConfigurationKeys { get; set; }

        public DescribeConfigsResourceMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version4)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}