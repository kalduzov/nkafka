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

public sealed class WriteTxnMarkersRequestMessage: RequestMessage
{
    /// <summary>
    /// The transaction markers to be written.
    /// </summary>
    public List<WritableTxnMarkerMessage> Markers { get; set; } = new ();

    public WriteTxnMarkersRequestMessage()
    {
        ApiKey = ApiKeys.WriteTxnMarkers;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    public WriteTxnMarkersRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.WriteTxnMarkers;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version1;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class WritableTxnMarkerMessage: Message
    {
        /// <summary>
        /// The current producer ID.
        /// </summary>
        public long ProducerId { get; set; } = 0;

        /// <summary>
        /// The current epoch associated with the producer ID.
        /// </summary>
        public short ProducerEpoch { get; set; } = 0;

        /// <summary>
        /// The result of the transaction to write to the partitions (false = ABORT, true = COMMIT).
        /// </summary>
        public bool TransactionResult { get; set; } = false;

        /// <summary>
        /// Each topic that we want to write transaction marker(s) for.
        /// </summary>
        public List<WritableTxnMarkerTopicMessage> Topics { get; set; } = new ();

        /// <summary>
        /// Epoch associated with the transaction state partition hosted by this transaction coordinator
        /// </summary>
        public int CoordinatorEpoch { get; set; } = 0;

        public WritableTxnMarkerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public WritableTxnMarkerMessage(BufferReader reader, ApiVersions version)
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
        }
    }

    public sealed class WritableTxnMarkerTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The indexes of the partitions to write transaction markers for.
        /// </summary>
        public List<int> PartitionIndexes { get; set; } = new ();

        public WritableTxnMarkerTopicMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version1;
        }

        public WritableTxnMarkerTopicMessage(BufferReader reader, ApiVersions version)
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
        }
    }
}
