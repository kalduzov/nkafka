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

public partial class AlterClientQuotasRequestMessage: RequestMessage
{
    /// <summary>
    /// The quota configuration entries to alter.
    /// </summary>
    public IReadOnlyCollection<EntryDataMessage> Entries { get; set; }

    /// <summary>
    /// Whether the alteration should be validated, but not performed.
    /// </summary>
    public bool ValidateOnly { get; set; }

    public AlterClientQuotasRequestMessage()
    {
        ApiKey = ApiKeys.AlterClientQuotas;
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

    public class EntryDataMessage: Message
    {
        /// <summary>
        /// The quota entity to alter.
        /// </summary>
        public IReadOnlyCollection<EntityDataMessage> Entity { get; set; }

        /// <summary>
        /// An individual quota configuration entry to alter.
        /// </summary>
        public IReadOnlyCollection<OpDataMessage> Ops { get; set; }

        public EntryDataMessage()
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
    public class EntityDataMessage: Message
    {
        /// <summary>
        /// The entity type.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// The name of the entity, or null if the default.
        /// </summary>
        public string EntityName { get; set; }

        public EntityDataMessage()
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
    public class OpDataMessage: Message
    {
        /// <summary>
        /// The quota configuration key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value to set, otherwise ignored if the value is to be removed.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Whether the quota configuration value should be removed, otherwise set.
        /// </summary>
        public bool Remove { get; set; }

        public OpDataMessage()
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