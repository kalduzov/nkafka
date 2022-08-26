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

public sealed class DescribeClientQuotasResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or `0` if the quota description succeeded.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The error message, or `null` if the quota description succeeded.
    /// </summary>
    public string ErrorMessageMessage { get; set; } = "";

    /// <summary>
    /// A result entry.
    /// </summary>
    public List<EntryDataMessage> EntriesMessage { get; set; } = new ();

    public DescribeClientQuotasResponseMessage()
    {
        ApiKey = ApiKeys.DescribeClientQuotas;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public DescribeClientQuotasResponseMessage(BufferReader reader, ApiVersions version)
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


    public sealed class EntryDataMessage: Message
    {
        /// <summary>
        /// The quota entity description.
        /// </summary>
        public List<EntityDataMessage> EntityMessage { get; set; } = new ();

        /// <summary>
        /// The quota values for the entity.
        /// </summary>
        public List<ValueDataMessage> ValuesMessage { get; set; } = new ();

        public EntryDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public EntryDataMessage(BufferReader reader, ApiVersions version)
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

    public sealed class EntityDataMessage: Message
    {
        /// <summary>
        /// The entity type.
        /// </summary>
        public string EntityTypeMessage { get; set; } = "";

        /// <summary>
        /// The entity name, or null if the default.
        /// </summary>
        public string EntityNameMessage { get; set; } = "";

        public EntityDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public EntityDataMessage(BufferReader reader, ApiVersions version)
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

    public sealed class ValueDataMessage: Message
    {
        /// <summary>
        /// The quota configuration key.
        /// </summary>
        public string KeyMessage { get; set; } = "";

        /// <summary>
        /// The quota configuration value.
        /// </summary>
        public double ValueMessage { get; set; } = 0.0;

        public ValueDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public ValueDataMessage(BufferReader reader, ApiVersions version)
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
