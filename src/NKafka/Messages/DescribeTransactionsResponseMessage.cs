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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class DescribeTransactionsResponseMessage: ResponseMessage
{
    /// <summary>
    /// 
    /// </summary>
    public List<TransactionStateMessage> TransactionStatesMessage { get; set; } = new ();

    public DescribeTransactionsResponseMessage()
    {
        ApiKey = ApiKeys.DescribeTransactions;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public DescribeTransactionsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.DescribeTransactions;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class TransactionStateMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public string TransactionalIdMessage { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string TransactionStateMessage { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public int TransactionTimeoutMsMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long TransactionStartTimeMsMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long ProducerIdMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public short ProducerEpochMessage { get; set; } = 0;

        /// <summary>
        /// The set of partitions included in the current transaction (if active). When a transaction is preparing to commit or abort, this will include only partitions which do not have markers.
        /// </summary>
        public List<TopicDataMessage> TopicsMessage { get; set; } = new ();

        public TransactionStateMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public TransactionStateMessage(BufferReader reader, ApiVersions version)
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

    public sealed class TopicDataMessage: Message
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string,> TopicMessage { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<int> PartitionsMessage { get; set; } = new ();

        public TopicDataMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public TopicDataMessage(BufferReader reader, ApiVersions version)
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
