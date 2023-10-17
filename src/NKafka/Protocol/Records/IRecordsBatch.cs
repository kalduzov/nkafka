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
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Runtime.CompilerServices;

using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Protocol.Records;

/// <summary>
/// 
/// </summary>
public interface IRecordsBatch
{
    /// <summary>
    /// The size of these records in bytes.
    /// </summary>
    int SizeInBytes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    BufferWriter Buffer { get; set; }

    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<Record> Records { get; }

    /// <summary>
    /// 
    /// </summary>
    int CountRecords { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int BaseSequence { get; set; }

    /// <summary>
    /// Get the producer epoch for this log record batch
    /// </summary>
    short ProducerEpoch { get; set; }

    /// <summary>
    /// Get the producer id for this log record batch 
    /// </summary>
    long ProducerId { get; set; }

    /// <summary>
    /// Get the max timestamp or log append time of this record batch 
    /// </summary>
    long MaxTimestamp { get; set; }

    /// <summary>
    ///  
    /// </summary>
    long BaseTimestamp { get; set; }

    /// <summary>
    /// Get the last offset in this record batch (inclusive)
    /// </summary>
    int LastOffsetDelta { get; set; }

    /// <summary>
    /// 
    /// </summary>
    short Attributes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int PartitionLeaderEpoch { get; set; }

    /// <summary>
    /// Get the record format version of this record batch (i.e its magic value). This libs support only v2.  
    /// </summary>
    byte Magic { get; set; }

    /// <summary>
    /// Get the size in bytes of this batch, including the size of the record and the batch overhead 
    /// </summary>
    int Length { get; set; }

    /// <summary>
    /// 
    /// </summary>
    uint Crc { get; set; }

    /// <summary>
    /// Get the base offset contained in this record batch.
    /// </summary>
    long BaseOffset { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool IsControlBatch => CheckBit(BitMask.ControlBit);

    /// <summary>
    /// 
    /// </summary>
    bool IsTransactional => CheckBit(BitMask.TransactionalBit);

    /// <summary>
    /// 
    /// </summary>
    CompressionType CompressionType => GetCompressionType();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CompressionType GetCompressionType()
    {
        var attr = Attributes & 0b0000_00011;

        return attr switch
        {
            0 => CompressionType.None,
            1 => CompressionType.Gzip,
            2 => CompressionType.Snappy,
            3 => CompressionType.Lz4,
            4 => CompressionType.ZStd,
            _ => throw new InvalidCompressionTypeException("Unsupported compression type")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckBit(BitMask mask)
    {
        return (Attributes & (short)mask) == (short)mask;
    }
}