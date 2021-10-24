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

using System.Text;

using NKafka.Config;
using NKafka.Protocol.Extensions;

namespace NKafka.Protocol.Records;

public sealed class Records: IRecords
{
    internal const int RECORD_BATCH_OVERHEAD = 61;
    private const int _MAX_RECORD_OVERHEAD = 21;
    internal const long NO_TIMESTAMP = -1;
    private const long _NO_PRODUCER_ID = -1;
    private const int _NO_SEQUENCE = -1;
    private const int _NO_PARTITION_LEADER_EPOCH = -1;
    private const short _NO_PRODUCER_EPOCH = -1;



    public Records(BufferReader reader)
    {
        Read(reader);
    }

    /// <summary>
    /// The size of these records in bytes.
    /// </summary>
    public int SizeInBytes { get; set; }

    public byte[] Buffer { get; set; }

    private void Read(BufferReader reader)
    {
    }

    internal static RecordsBuilder Builder(BufferWriter buffer, CompressionType compressionType, TimestampType timestampType, long baseOffset)
    {
        var logAppendTime = NO_TIMESTAMP;

        if (timestampType == TimestampType.LogAppendTime)
        {
            logAppendTime = Timestamp.DateTimeToUnixTimestampMs(DateTime.Now);
        }

        return Builder(buffer,
            compressionType,
            timestampType,
            baseOffset,
            logAppendTime,
            _NO_PRODUCER_ID,
            _NO_PRODUCER_EPOCH,
            _NO_SEQUENCE,
            isTransactional: false,
            _NO_PARTITION_LEADER_EPOCH);
    }

    internal static RecordsBuilder Builder(BufferWriter buffer,
        CompressionType compressionType,
        TimestampType timestampType,
        long baseOffset,
        long logAppendTime,
        long producerId,
        short producerEpoch,
        int baseSequence,
        bool isTransactional,
        int partitionLeaderEpoch)
    {
        return Builder(buffer,
            compressionType,
            timestampType,
            baseOffset,
            logAppendTime,
            producerId,
            producerEpoch,
            baseSequence,
            isTransactional,
            isControlBatch: false,
            partitionLeaderEpoch);
    }

    internal static RecordsBuilder Builder(BufferWriter buffer,
        CompressionType compressionType,
        TimestampType timestampType,
        long baseOffset,
        long logAppendTime,
        long producerId,
        short producerEpoch,
        int baseSequence,
        bool isTransactional,
        bool isControlBatch,
        int partitionLeaderEpoch)
    {
        return new RecordsBuilder(buffer,
            compressionType,
            timestampType,
            baseOffset,
            logAppendTime,
            producerId,
            producerEpoch,
            baseSequence,
            isTransactional,
            isControlBatch,
            partitionLeaderEpoch,
            buffer.Remaining);
    }

    /// <summary>
    /// Вычисляем верхний порог размера записи
    /// </summary>
    internal static int EstimateSizeInBytesUpperBound(byte[]? serializedKey, byte[]? serializedValue, Headers headers)
    {
        var keySize = serializedKey?.Length ?? -1;
        var valueSize = serializedValue?.Length ?? -1;

        return RECORD_BATCH_OVERHEAD + _MAX_RECORD_OVERHEAD + Record.SizeOf(keySize, valueSize, headers);
    }



    public override string ToString()
    {
        return "Records";
    }
}