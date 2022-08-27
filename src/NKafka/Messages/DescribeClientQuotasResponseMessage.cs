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

using NKafka.Exceptions;
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
    public short ErrorCode { get; set; } = 0;

    /// <summary>
    /// The error message, or `null` if the quota description succeeded.
    /// </summary>
    public string ErrorMessage { get; set; } = "";

    /// <summary>
    /// A result entry.
    /// </summary>
    public List<EntryDataMessage> Entries { get; set; } = new ();

    public DescribeClientQuotasResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public DescribeClientQuotasResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteShort(ErrorCode);
        if (ErrorMessage is null)
        {
            if (version >= ApiVersions.Version1)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteShort(-1);
            }
        }
        else
        {
            var stringBytes = Encoding.UTF8.GetBytes(ErrorMessage);
            if (version >= ApiVersions.Version1)
            {
                writer.WriteVarUInt(stringBytes.Length + 1);
            }
            else
            {
                writer.WriteShort((short)stringBytes.Length);
            }
            writer.WriteBytes(stringBytes);
        }
        if (version >= ApiVersions.Version1)
        {
            if (Entries is null)
            {
                writer.WriteVarUInt(0);
            }
            else
            {
                writer.WriteVarUInt(Entries.Count + 1);
                foreach (var element in Entries)
                {
                    element.Write(writer, version);
                }
            }
        }
        else
        {
            if (Entries is null)
            {
                writer.WriteInt(-1);
            }
            else
            {
                writer.WriteInt(Entries.Count);
                foreach (var element in Entries)
                {
                    element.Write(writer, version);
                }
            }
        }
    }

    public sealed class EntryDataMessage: Message
    {
        /// <summary>
        /// The quota entity description.
        /// </summary>
        public List<EntityDataMessage> Entity { get; set; } = new ();

        /// <summary>
        /// The quota values for the entity.
        /// </summary>
        public List<ValueDataMessage> Values { get; set; } = new ();

        public EntryDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public EntryDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            if (version >= ApiVersions.Version1)
            {
                writer.WriteVarUInt(Entity.Count + 1);
                foreach (var element in Entity)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Entity.Count);
                foreach (var element in Entity)
                {
                    element.Write(writer, version);
                }
            }
            if (version >= ApiVersions.Version1)
            {
                writer.WriteVarUInt(Values.Count + 1);
                foreach (var element in Values)
                {
                    element.Write(writer, version);
                }
            }
            else
            {
                writer.WriteInt(Values.Count);
                foreach (var element in Values)
                {
                    element.Write(writer, version);
                }
            }
        }
    }

    public sealed class EntityDataMessage: Message
    {
        /// <summary>
        /// The entity type.
        /// </summary>
        public string EntityType { get; set; } = "";

        /// <summary>
        /// The entity name, or null if the default.
        /// </summary>
        public string EntityName { get; set; } = "";

        public EntityDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public EntityDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(EntityType);
                if (version >= ApiVersions.Version1)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            if (EntityName is null)
            {
                if (version >= ApiVersions.Version1)
                {
                    writer.WriteVarUInt(0);
                }
                else
                {
                    writer.WriteShort(-1);
                }
            }
            else
            {
                var stringBytes = Encoding.UTF8.GetBytes(EntityName);
                if (version >= ApiVersions.Version1)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
        }
    }

    public sealed class ValueDataMessage: Message
    {
        /// <summary>
        /// The quota configuration key.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// The quota configuration value.
        /// </summary>
        public double Value { get; set; } = 0.0;

        public ValueDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public ValueDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
            var numTaggedFields = 0;
            {
                var stringBytes = Encoding.UTF8.GetBytes(Key);
                if (version >= ApiVersions.Version1)
                {
                    writer.WriteVarUInt(stringBytes.Length + 1);
                }
                else
                {
                    writer.WriteShort((short)stringBytes.Length);
                }
                writer.WriteBytes(stringBytes);
            }
            writer.WriteDouble(Value);
        }
    }
}
