﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

public partial class DescribeConfigsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The results for each resource.
    /// </summary>
    public IReadOnlyCollection<DescribeConfigsResultMessage> Results { get; set; }

    public DescribeConfigsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public DescribeConfigsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
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

    public class DescribeConfigsResultMessage: Message
    {
        /// <summary>
        /// The error code, or 0 if we were able to successfully describe the configurations.
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// The error message, or null if we were able to successfully describe the configurations.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; }

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Each listed configuration.
        /// </summary>
        public IReadOnlyCollection<DescribeConfigsResourceResultMessage> Configs { get; set; }

        public DescribeConfigsResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public DescribeConfigsResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
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
    public class DescribeConfigsResourceResultMessage: Message
    {
        /// <summary>
        /// The configuration name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The configuration value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// True if the configuration is read-only.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// True if the configuration is not set.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// The configuration source.
        /// </summary>
        public sbyte? ConfigSource { get; set; } = -1;

        /// <summary>
        /// True if this configuration is sensitive.
        /// </summary>
        public bool IsSensitive { get; set; }

        /// <summary>
        /// The synonyms for this configuration key.
        /// </summary>
        public IReadOnlyCollection<DescribeConfigsSynonymMessage>? Synonyms { get; set; }

        /// <summary>
        /// The configuration data type. Type can be one of the following values - BOOLEAN, STRING, INT, SHORT, LONG, DOUBLE, LIST, CLASS, PASSWORD
        /// </summary>
        public sbyte? ConfigType { get; set; } = 0;

        /// <summary>
        /// The configuration documentation.
        /// </summary>
        public string? Documentation { get; set; }

        public DescribeConfigsResourceResultMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public DescribeConfigsResourceResultMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
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
    public class DescribeConfigsSynonymMessage: Message
    {
        /// <summary>
        /// The synonym name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The synonym value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The synonym source.
        /// </summary>
        public sbyte Source { get; set; }

        public DescribeConfigsSynonymMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public DescribeConfigsSynonymMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
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