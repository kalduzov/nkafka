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

public partial class DescribeClientQuotasRequestMessage: RequestMessage
{
    /// <summary>
    /// Filter components to apply to quota entities.
    /// </summary>
    public IReadOnlyCollection<ComponentDataMessage> Components { get; set; }

    /// <summary>
    /// Whether the match is strict, i.e. should exclude entities with unspecified entity types.
    /// </summary>
    public bool Strict { get; set; }

    public DescribeClientQuotasRequestMessage()
    {
        ApiKey = ApiKeys.DescribeClientQuotas;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version1)
        {
        }
        else //no flexible version
        {
        }

    }

    public class ComponentDataMessage: Message
    {
        /// <summary>
        /// The entity type that the filter component applies to.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// How to match the entity {0 = exact name, 1 = default name, 2 = any specified name}.
        /// </summary>
        public sbyte MatchType { get; set; }

        /// <summary>
        /// The string to match against, or null if unused for the match type.
        /// </summary>
        public string Match { get; set; }

        public ComponentDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version1)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}