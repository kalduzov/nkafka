//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

namespace NKafka.Clients.Consumer.Internal;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
internal sealed class ConsumerChannel<TKey, TValue>: Channel<ConsumerRecord<TKey, TValue>>
    where TKey : notnull
    where TValue : notnull
{
    private readonly Subscription _subscription;
    private readonly ILogger<ConsumerChannel<TKey, TValue>> _logger;

    public ConsumerChannel(int capacity, bool dropOldRecordsFromChannel, Subscription subscription, ILogger<ConsumerChannel<TKey, TValue>> logger)
    {
        _subscription = subscription;
        _logger = logger;
        var innerChannel = Channel.CreateBounded<ConsumerRecord<TKey, TValue>>(
            new BoundedChannelOptions(capacity)
            {
                FullMode = dropOldRecordsFromChannel ? BoundedChannelFullMode.DropOldest : BoundedChannelFullMode.Wait,
                SingleWriter = false,
                SingleReader = false
            },
            RecordsDroppedFromChannel);
        Reader = new ConsumerChannelReader(innerChannel.Reader, subscription);
        Writer = new ConsumerChannelWriter(innerChannel.Writer, subscription);
    }

    private void RecordsDroppedFromChannel(ConsumerRecord<TKey, TValue> record)
    {
        // todo
        // возможно имеет смысл для сценариев, когда потребитель не успевает обрабатывать сообщения,
        // однако ему не сильно важно, если такие сообщения теряются. В таком случае мы должны тоже
        // сохранять последний оффсет для сохранения

        // В случае чтения данных из канала - конечный потребитель сам обрабатывает исключения обновления оффсета, если они возникают.
        // В данном случае исключения нужно обрабатывать в этом методе 

        _logger.LogTrace("Сообщение {Record} было выброшено из канала без обработки", record.ToString());

        try
        {
            _subscription.OffsetManager.UpdateLastReadingOffset(record.TopicPartitionOffset);
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Для сообщение {Record} не удалось обновить смещение в OffsetStore", record.ToString());
        }

    }

    private sealed class ConsumerChannelWriter: ChannelWriter<ConsumerRecord<TKey, TValue>>
    {
        private readonly ChannelWriter<ConsumerRecord<TKey, TValue>> _innerChannelWriter;
        private readonly Subscription _subscription;

        public ConsumerChannelWriter(ChannelWriter<ConsumerRecord<TKey, TValue>> innerChannelWriter, Subscription subscription)
        {
            _innerChannelWriter = innerChannelWriter;
            _subscription = subscription;
        }

        public override bool TryWrite(ConsumerRecord<TKey, TValue> item)
        {
            var wasMessageWritten = _innerChannelWriter.TryWrite(item);

            if (wasMessageWritten) // yes
            {
                _subscription.OffsetManager.UpdateLastWroteOffset(item.TopicPartitionOffset);
            }

            return wasMessageWritten;
        }

        public override ValueTask<bool> WaitToWriteAsync(CancellationToken token = default)
        {
            return _innerChannelWriter.WaitToWriteAsync(token);
        }
    }

    private sealed class ConsumerChannelReader: ChannelReader<ConsumerRecord<TKey, TValue>>
    {
        private readonly ChannelReader<ConsumerRecord<TKey, TValue>> _innerChannelReader;
        private readonly Subscription _subscription;

        public ConsumerChannelReader(ChannelReader<ConsumerRecord<TKey, TValue>> innerChannelReader, Subscription subscription)
        {
            _innerChannelReader = innerChannelReader;
            _subscription = subscription;
        }

        public override bool TryRead([MaybeNullWhen(false)] out ConsumerRecord<TKey, TValue> item)
        {
            if (!_innerChannelReader.TryRead(out item))
            {
                return false;
            }
            _subscription.OffsetManager.UpdateLastReadingOffset(item.TopicPartitionOffset);

            return true;
        }

        public override ValueTask<bool> WaitToReadAsync(CancellationToken token = default)
        {
            return _innerChannelReader.WaitToReadAsync(token);
        }
    }
}