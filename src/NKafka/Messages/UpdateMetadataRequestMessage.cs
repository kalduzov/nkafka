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

public partial class UpdateMetadataRequestMessage: RequestMessage
{
    /// <summary>
    /// The controller id.
    /// </summary>
    public int ControllerId { get; set; }

    /// <summary>
    /// The controller epoch.
    /// </summary>
    public int ControllerEpoch { get; set; }

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long? BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// In older versions of this RPC, each partition that we would like to update.
    /// </summary>
    public IReadOnlyCollection<UpdateMetadataPartitionStateMessage> UngroupedPartitionStates { get; set; }

    /// <summary>
    /// In newer versions of this RPC, each topic that we would like to update.
    /// </summary>
    public IReadOnlyCollection<UpdateMetadataTopicStateMessage> TopicStates { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyCollection<UpdateMetadataBrokerMessage> LiveBrokers { get; set; }

    public UpdateMetadataRequestMessage()
    {
        ApiKey = ApiKeys.UpdateMetadata;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version7;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version6)
        {
        }
        else //no flexible version
        {
        }

        //flexible version
        if (Version >= ApiVersions.Version6)
        {
        }
        else //no flexible version
        {
        }

        //flexible version
        if (Version >= ApiVersions.Version6)
        {
        }
        else //no flexible version
        {
        }

    }

    public class UpdateMetadataPartitionStateMessage: Message
    {

            public UpdateMetadataPartitionStateMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version7;
            }

            public override void Read(BufferReader reader, ApiVersions version)
            {
            }

            public override void Write(BufferWriter writer, ApiVersions version)
            {
                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

            }
        }
        public class UpdateMetadataTopicStateMessage: Message
        {
            /// <summary>
            /// The topic name.
            /// </summary>
            public string TopicName { get; set; }

            /// <summary>
            /// The topic id.
            /// </summary>
            public Guid? TopicId { get; set; }

            /// <summary>
            /// The partition that we would like to update.
            /// </summary>
            public IReadOnlyCollection<UpdateMetadataPartitionStateMessage> PartitionStates { get; set; }

            public UpdateMetadataTopicStateMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version7;
            }

            public override void Read(BufferReader reader, ApiVersions version)
            {
            }

            public override void Write(BufferWriter writer, ApiVersions version)
            {
                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

            }
        }
        public class UpdateMetadataBrokerMessage: Message
        {
            /// <summary>
            /// The broker id.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// The broker hostname.
            /// </summary>
            public string? V0Host { get; set; }

            /// <summary>
            /// The broker port.
            /// </summary>
            public int? V0Port { get; set; }

            /// <summary>
            /// The broker endpoints.
            /// </summary>
            public IReadOnlyCollection<UpdateMetadataEndpointMessage>? Endpoints { get; set; }

            /// <summary>
            /// The rack which this broker belongs to.
            /// </summary>
            public string? Rack { get; set; }

            public UpdateMetadataBrokerMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version7;
            }

            public override void Read(BufferReader reader, ApiVersions version)
            {
            }

            public override void Write(BufferWriter writer, ApiVersions version)
            {
                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

            }
        }
        public class UpdateMetadataEndpointMessage: Message
        {
            /// <summary>
            /// The port of this endpoint
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// The hostname of this endpoint
            /// </summary>
            public string Host { get; set; }

            /// <summary>
            /// The listener name.
            /// </summary>
            public string? Listener { get; set; }

            /// <summary>
            /// The security protocol type.
            /// </summary>
            public short SecurityProtocol { get; set; }

            public UpdateMetadataEndpointMessage()
            {
                LowestSupportedVersion = ApiVersions.Version0;
                HighestSupportedVersion = ApiVersions.Version7;
            }

            public override void Read(BufferReader reader, ApiVersions version)
            {
            }

            public override void Write(BufferWriter writer, ApiVersions version)
            {
                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

                //flexible version
                if (Version >= ApiVersions.Version6)
                {
                }
                else //no flexible version
                {
                }

            }
        }
}