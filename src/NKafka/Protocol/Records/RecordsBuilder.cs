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

using NKafka.Compressions;
using NKafka.Config;

namespace NKafka.Protocol.Records;

internal class RecordsBuilder
{
    private readonly BufferWriter _buffer;
    private readonly CompressionType _compressionType;
    private readonly TimestampType _timestampType;
    private readonly long _baseOffset;
    private readonly long _logAppendTime;
    private readonly long _producerId;
    private readonly short _producerEpoch;
    private readonly int _baseSequence;
    private readonly bool _isTransactional;
    private readonly bool _isControlBatch;
    private readonly int _partitionLeaderEpoch;
    private readonly long _writeLimit;
    private int _numRecords;
    private int _uncompressedRecordsSizeInBytes;
    private readonly int _actualCompressionRatio;
    private long _maxTimestamp;
    private readonly int _batchHeaderSizeInBytes;
    private readonly long _initialPosition;
    private readonly BufferWriter? _appendBuffer;
    private Records? _builtRecords = null;
    private long? _lastOffset = null;
    private long? _baseTimestamp = null;
    private long _offsetOfMaxTimestamp;

    private long NextSequentialOffset => (long)(_lastOffset is null ? _baseOffset : _lastOffset + 1);

    public RecordsBuilder(BufferWriter buffer,
        CompressionType compressionType,
        TimestampType timestampType,
        long baseOffset,
        long logAppendTime,
        long producerId,
        short producerEpoch,
        int baseSequence,
        bool isTransactional,
        bool isControlBatch,
        int partitionLeaderEpoch,
        long writeLimit)
    {
        _buffer = buffer;
        _compressionType = compressionType;
        _timestampType = timestampType;
        _baseOffset = baseOffset;
        _logAppendTime = logAppendTime;
        _producerId = producerId;
        _producerEpoch = producerEpoch;
        _baseSequence = baseSequence;
        _isTransactional = isTransactional;
        _isControlBatch = isControlBatch;
        _partitionLeaderEpoch = partitionLeaderEpoch;
        _writeLimit = writeLimit;
        _numRecords = 0;
        _uncompressedRecordsSizeInBytes = 0;
        _actualCompressionRatio = 1;
        _maxTimestamp = Records.NO_TIMESTAMP;
        _batchHeaderSizeInBytes = Records.RECORD_BATCH_OVERHEAD;
        _initialPosition = buffer.Position;
        buffer.Position = _initialPosition + _batchHeaderSizeInBytes;
        _appendBuffer = compressionType.Wrap(_buffer);
    }

    /// <summary>
    /// Определяет есть ли свободное место для добавления новой записи
    /// </summary>
    /// <returns>true, если место еще есть</returns>
    public bool HasRoomFor(long timestamp, byte[]? key, byte[]? value, Headers headers)
    {
        if (IsFull)
        {
            return false;
        }

        if (_numRecords == 0)
        {
            return true;
        }

        return false;
    }

    public bool IsFull { get; set; }

    public void Append(long timestamp, byte[]? key, byte[]? value, Headers headers)
    {
        AppendWithOffset(NextSequentialOffset, timestamp, key, value, headers);
    }

    private void AppendWithOffset(long offset,
        long timestamp,
        byte[]? key,
        byte[]? value,
        Headers headers)
    {
        AppendWithOffset(offset, false, timestamp, key, value, headers);
    }

    private void AppendWithOffset(long offset,
        bool isControlRecord,
        long timestamp,
        byte[]? key,
        byte[]? value,
        Headers headers)
    {
        try
        {
            if (isControlRecord != _isControlBatch)
            {
                throw new ArgumentException("Control records can only be appended to control batches");
            }

            if (_lastOffset is not null && offset <= _lastOffset)
            {
                throw new ArgumentException($"Illegal offset {offset} following previous offset {_lastOffset} (Offsets must increase monotonically).");
            }

            if (timestamp < 0 && timestamp != Records.NO_TIMESTAMP)
            {
                throw new ArgumentException($"Invalid negative timestamp {timestamp}");
            }

            if (_baseTimestamp is null)
            {
                _baseTimestamp = timestamp;
            }
            AppendRecord(offset, timestamp, key, value, headers);
        }
        catch
        {
        }
    }

    private void AppendRecord(long offset,
        long timestamp,
        byte[]? key,
        byte[]? value,
        Headers headers)
    {
        if (_appendBuffer is null)
        {
            throw new ArgumentException("Tried to append a record, but RecordsBuilder is closed for record appends");
        }

        var offsetDelta = (int)(offset - _baseOffset);
        var timestampDelta = timestamp - _baseTimestamp!.Value;
        var sizeInBytes = Record.WriteTo(_appendBuffer, offsetDelta, timestampDelta, key, value, headers);
        RecordWritten(offset, timestamp, sizeInBytes);
    }

    private void RecordWritten(long offset, long timestamp, int size)
    {
        if (_numRecords == int.MaxValue)
        {
            throw new ArgumentException($"Maximum number of records per batch exceeded, max records: {int.MaxValue}");
        }

        if (offset - _baseOffset > int.MaxValue)
        {
            throw new ArgumentException($"Maximum offset delta exceeded, base offset: {_baseOffset}, last offset: {offset}");
        }
        _numRecords++;
        _uncompressedRecordsSizeInBytes += size;
        _lastOffset = offset;

        if (timestamp <= _maxTimestamp)
        {
            return;
        }
        _maxTimestamp = timestamp;
        _offsetOfMaxTimestamp = offset;
    }

    public void CloseForRecordAppends()
    {
    }
}