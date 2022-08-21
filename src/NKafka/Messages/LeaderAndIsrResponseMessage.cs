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

public partial class LeaderAndIsrResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; }

    /// <summary>
    /// Each partition in v0 to v4 message.
    /// </summary>
    public IReadOnlyCollection<LeaderAndIsrPartitionErrorMessage> PartitionErrors { get; set; }

    /// <summary>
    /// Each topic
    /// </summary>
    public IReadOnlyCollection<LeaderAndIsrTopicErrorMessage> Topics { get; set; }

    public LeaderAndIsrResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
    }

    public LeaderAndIsrResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version5;
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

        //flexible version
        if (Version >= ApiVersions.Version4)
        {
        }
        else //no flexible version
        {
        }

    }

    public class LeaderAndIsrPartitionErrorMessage: Message
    {

            public LeaderAndIsrPartitionErrorMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version5;
            }

            public LeaderAndIsrPartitionErrorMessage(BufferReader reader, ApiVersions version)
                : base(reader, version)
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version5;
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

                //flexible version
                if (Version >= ApiVersions.Version4)
                {
                }
                else //no flexible version
                {
                }

            }
        }
        public class LeaderAndIsrTopicErrorMessage: Message
        {
            /// <summary>
            /// The unique topic ID
            /// </summary>
            public Guid TopicId { get; set; }

            /// <summary>
            /// Each partition.
            /// </summary>
            public IReadOnlyCollection<LeaderAndIsrPartitionErrorMessage> PartitionErrors { get; set; }

            public LeaderAndIsrTopicErrorMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version5;
            }

            public LeaderAndIsrTopicErrorMessage(BufferReader reader, ApiVersions version)
                : base(reader, version)
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version5;
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