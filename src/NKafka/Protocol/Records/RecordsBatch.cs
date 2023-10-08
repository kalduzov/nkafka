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

using NKafka.Protocol.Buffers;

namespace NKafka.Protocol.Records;

/// <summary>
/// 
/// </summary>
public class RecordsBatch: IRecordsBatch
{
    /// <summary>
    /// Batch header length
    /// </summary>
    internal const int RECORD_BATCH_OVERHEAD = 61;

    private const int _MAX_RECORD_OVERHEAD = 21;
    internal const long NO_TIMESTAMP = -1;
    private const long _NO_PRODUCER_ID = -1;
    private const int _NO_SEQUENCE = -1;
    private const int _NO_PARTITION_LEADER_EPOCH = -1;
    private const short _NO_PRODUCER_EPOCH = -1;

    /// <summary>
    /// The size of these records in bytes.
    /// </summary>
    public int SizeInBytes { get; set; } = RECORD_BATCH_OVERHEAD;

    /// <summary>
    /// 
    /// </summary>
    public BufferWriter Buffer { get; set; } = new(Stream.Null);

    /// <inheritdoc />
    public int CountRecords { get; set; }

    /// <inheritdoc />
    public int BaseSequence { get; set; }

    /// <inheritdoc />
    public short ProducerEpoch { get; set; }

    /// <inheritdoc />
    public long ProducerId { get; set; }

    /// <inheritdoc />
    public long MaxTimestamp { get; set; }

    /// <inheritdoc />
    public long BaseTimestamp { get; set; }

    /// <inheritdoc />
    public int LastOffsetDelta { get; set; }

    /// <inheritdoc />
    public short Attributes { get; set; }

    /// <inheritdoc />
    public int PartitionLeaderEpoch { get; set; }

    /// <inheritdoc />
    public byte Magic { get; set; } = 2;

    /// <inheritdoc />
    public int Length { get; set; }

    /// <inheritdoc />
    public uint Crc { get; set; }

    /// <inheritdoc />
    public long BaseOffset { get; set; }

    /// <inheritdoc />
    public IReadOnlyCollection<Record> Records { get; private set; } = new List<Record>();

    /// <summary>
    /// 
    /// </summary>
    protected RecordsBatch()
    {
        Length = RECORD_BATCH_OVERHEAD;
        ProducerEpoch = _NO_PRODUCER_EPOCH;
        ProducerId = _NO_PRODUCER_ID;
        BaseSequence = _NO_SEQUENCE;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    public RecordsBatch(ref BufferReader reader)
        : this()
    {
        Read(ref reader);
    }

    private void Read(ref BufferReader reader)
    {
        // не хватает данных для считываения заголовка батча, значит выходим из цикла
        if (reader.Remaining < RECORD_BATCH_OVERHEAD)
        {
            return;
        }

        BaseOffset = reader.ReadLong();
        Length = reader.ReadInt();
        PartitionLeaderEpoch = reader.ReadInt();
        Magic = reader.ReadByte();
        Crc = reader.ReadUInt();
        Attributes = reader.ReadShort();
        LastOffsetDelta = reader.ReadInt();
        BaseTimestamp = reader.ReadLong();
        MaxTimestamp = reader.ReadLong();
        ProducerId = reader.ReadLong();
        ProducerEpoch = reader.ReadShort();
        BaseSequence = reader.ReadInt();
        CountRecords = reader.ReadInt();

        var records = new List<Record>(CountRecords);

        for (var i = 0; i < CountRecords; i++)
        {
            if (reader.Remaining < _MAX_RECORD_OVERHEAD)
            {
                //Нет места для считывание даже записи с минимальным размером
                break;
            }
            var record = new Record(ref reader);
            SizeInBytes += record.Length;

            if (record.IsValid)
            {
                records.Add(record);
            }
        }

        Records = records;
    }

    /// <summary>
    /// An estimate of the upper bound on the record size in bytes
    /// </summary>
    internal static int EstimateSizeInBytesUpperBound(byte[]? serializedKey, byte[]? serializedValue, Headers headers)
    {
        var keySize = serializedKey?.Length ?? -1;
        var valueSize = serializedValue?.Length ?? -1;

        return _MAX_RECORD_OVERHEAD + RecordExtensions.SizeOf(keySize, valueSize, headers);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Records";
    }
}