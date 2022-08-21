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

public partial class DescribeTransactionsResponseMessage: ResponseMessage
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyCollection<TransactionStateMessage> TransactionStates { get; set; }

    public DescribeTransactionsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public DescribeTransactionsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        //flexible version
        if (Version >= ApiVersions.Version0)
        {
        }
        else //no flexible version
        {
        }

    }

    public class TransactionStateMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TransactionalId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TransactionState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TransactionTimeoutMs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long TransactionStartTimeMs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long ProducerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public short ProducerEpoch { get; set; }

        /// <summary>
        /// The set of partitions included in the current transaction (if active). When a transaction is preparing to commit or abort, this will include only partitions which do not have markers.
        /// </summary>
        public IReadOnlyCollection<TopicDataMessage> Topics { get; set; }

        public TransactionStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public TransactionStateMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version0)
            {
            }
            else //no flexible version
            {
            }

        }
    }
    public class TopicDataMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<int> Partitions { get; set; }

        public TopicDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public TopicDataMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
            //flexible version
            if (Version >= ApiVersions.Version0)
            {
            }
            else //no flexible version
            {
            }

        }
    }
}