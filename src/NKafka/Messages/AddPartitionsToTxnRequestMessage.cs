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

public partial class AddPartitionsToTxnRequestMessage: RequestMessage
{
    /// <summary>
    /// The transactional id corresponding to the transaction.
    /// </summary>
    public string TransactionalId { get; set; }

    /// <summary>
    /// Current producer id in use by the transactional id.
    /// </summary>
    public long ProducerId { get; set; }

    /// <summary>
    /// Current epoch associated with the producer id.
    /// </summary>
    public short ProducerEpoch { get; set; }

    /// <summary>
    /// The partitions to add to the transaction.
    /// </summary>
    public IReadOnlyCollection<AddPartitionsToTxnTopicMessage> Topics { get; set; }

    public AddPartitionsToTxnRequestMessage()
    {
        ApiKey = ApiKeys.AddPartitionsToTxn;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version3;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version3)
        {
        }
        else //no flexible version
        {
        }

    }

    public class AddPartitionsToTxnTopicMessage: Message
    {
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The partition indexes to add to the transaction
        /// </summary>
        public IReadOnlyCollection<int> Partitions { get; set; }

        public AddPartitionsToTxnTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version3;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version3)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}