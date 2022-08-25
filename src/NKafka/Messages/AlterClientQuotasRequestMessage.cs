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
public sealed partial class AlterClientQuotasRequestMessage: RequestMessage
{
    /// <summary>
    /// The quota configuration entries to alter.
    /// </summary>
    public List<EntryData> Entries { get; set; } = new();

    /// <summary>
    /// Whether the alteration should be validated, but not performed.
    /// </summary>
    public bool ValidateOnly { get; set; } = false;

    public AlterClientQuotasRequestMessage()
    {
        ApiKey = ApiKeys.AlterClientQuotas;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public AlterClientQuotasRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.AlterClientQuotas;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class EntryData: Message
    {
        /// <summary>
        /// The quota entity to alter.
        /// </summary>
        public List<EntityData> Entity { get; set; } = new();

        /// <summary>
        /// An individual quota configuration entry to alter.
        /// </summary>
        public List<OpData> Ops { get; set; } = new();

        public EntryData()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public EntryData(BufferReader reader, ApiVersions version)
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
    public class EntityData: Message
    {
        /// <summary>
        /// The entity type.
        /// </summary>
        public string EntityType { get; set; } = null!;

        /// <summary>
        /// The name of the entity, or null if the default.
        /// </summary>
        public string EntityName { get; set; } = null!;

        public EntityData()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public EntityData(BufferReader reader, ApiVersions version)
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
    public class OpData: Message
    {
        /// <summary>
        /// The quota configuration key.
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// The value to set, otherwise ignored if the value is to be removed.
        /// </summary>
        public double Value { get; set; } = 0;

        /// <summary>
        /// Whether the quota configuration value should be removed, otherwise set.
        /// </summary>
        public bool Remove { get; set; } = false;

        public OpData()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public OpData(BufferReader reader, ApiVersions version)
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