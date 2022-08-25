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
public sealed partial class DescribeClientQuotasRequestMessage: RequestMessage
{
    /// <summary>
    /// Filter components to apply to quota entities.
    /// </summary>
    public List<ComponentData> Components { get; set; } = new();

    /// <summary>
    /// Whether the match is strict, i.e. should exclude entities with unspecified entity types.
    /// </summary>
    public bool Strict { get; set; } = false;

    public DescribeClientQuotasRequestMessage()
    {
        ApiKey = ApiKeys.DescribeClientQuotas;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public DescribeClientQuotasRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeClientQuotas;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class ComponentData: Message
    {
        /// <summary>
        /// The entity type that the filter component applies to.
        /// </summary>
        public string EntityType { get; set; } = null!;

        /// <summary>
        /// How to match the entity {0 = exact name, 1 = default name, 2 = any specified name}.
        /// </summary>
        public sbyte MatchType { get; set; } = 0;

        /// <summary>
        /// The string to match against, or null if unused for the match type.
        /// </summary>
        public string Match { get; set; } = null!;

        public ComponentData()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public ComponentData(BufferReader reader, ApiVersions version)
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