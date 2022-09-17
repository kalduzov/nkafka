// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace NKafka.Protocol.Records;

public class RecordBatch: List<Record>
{
    /// <summary>
    /// Denotes the first offset in the RecordBatch. The 'offsetDelta' of each Record in the batch would be be computed relative to this FirstOffset.
    /// In particular, the offset of each Record in the Batch is its 'OffsetDelta' + 'FirstOffset'.
    /// </summary>
    public long FirstOffset { get; set; }

    public int Length { get; set; }

    /// <summary>
    /// Introduced with KIP-101, this is set by the broker upon receipt of a produce request and is used to ensure no loss of data when there are
    /// leader changes with log truncation. Client developers do not need to worry about setting this value.
    /// </summary>
    public int PartitionLeaderEpoch { get; set; }

    public sbyte Magic { get; set; }

    public int CRC { get; set; }

    public short Attributes { get; set; }

    /// <summary>
    /// The offset of the last message in the RecordBatch. This is used by the broker to ensure correct behavior even when Records within
    /// a batch are compacted out.
    /// </summary>
    public int LastOffsetDelta { get; set; }

    /// <summary>
    /// The timestamp of the first Record in the batch. The timestamp of each Record in the RecordBatch is its 'TimestampDelta' + 'FirstTimestamp'.
    /// </summary>
    public long FirstTimestamp { get; set; }

    /// <summary>
    /// The timestamp of the last Record in the batch. This is used by the broker to ensure the correct
    /// behavior even when Records within the batch are compacted out.
    /// </summary>
    public long MaxTimestamp { get; set; }

    /// <summary>
    /// Introduced in 0.11.0.0 for KIP-98, this is the broker assigned producerId received by the 'InitProducerId' request.
    /// Clients which want to support idempotent message delivery and transactions must set this field.
    /// </summary>
    public long ProducerId { get; set; }

    /// <summary>
    /// Introduced in 0.11.0.0 for KIP-98, this is the broker assigned producerEpoch received by the 'InitProducerId' request.
    /// Clients which want to support idempotent message delivery and transactions must set this field.
    /// </summary>
    public short ProducerEpoch { get; set; }

    /// <summary>
    /// Introduced in 0.11.0.0 for KIP-98, this is the producer assigned sequence number which is used by the broker to deduplicate messages.
    /// Clients which want to support idempotent message delivery and transactions must set this field.
    /// The sequence number for each Record in the RecordBatch is its OffsetDelta + FirstSequence.
    /// </summary>
    public int FirstSequence { get; set; }

    public IReadOnlyCollection<Record> Records => this;

    public byte[] Buffer { get; set; }
}