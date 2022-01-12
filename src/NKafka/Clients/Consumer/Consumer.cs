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

using System.Collections.Concurrent;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Exceptions;
using NKafka.Serialization;

namespace NKafka.Clients.Consumer;

internal class Consumer<TKey, TValue>: Client<ConsumerConfig>, IConsumer<TKey, TValue>
{
    private readonly IAsyncSerializer<TKey>? _keySerializer;
    private readonly IAsyncSerializer<TValue>? _valueSerializer;

    private readonly ConcurrentDictionary<string, Channel<ConsumerRecord<TKey, TValue>>> _channelsByTopics;

    public Consumer(
        IKafkaCluster kafkaCluster,
        ConsumerConfig config,
        IAsyncSerializer<TKey>? keySerializer,
        IAsyncSerializer<TValue>? valueSerializer,
        ILogger logger)
        : base(kafkaCluster, config, logger)
    {
        _keySerializer = keySerializer;
        _valueSerializer = valueSerializer;
        _channelsByTopics = new ConcurrentDictionary<string, Channel<ConsumerRecord<TKey, TValue>>>();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public override void Dispose()
    {
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public override ValueTask DisposeAsync()
    {
        return default;
    }

    /// <summary>Notifies the provider that an observer is to receive notifications.</summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>A reference to an interface that allows observers to stop receiving notifications before the provider has finished sending them.</returns>
    public IDisposable Subscribe(IObserver<ConsumerRecord<TKey, TValue>> observer)
    {
        return this;
    }

    /// <summary>Notifies the observer that the provider has finished sending push-based notifications.</summary>
    public void OnCompleted()
    {
    }

    /// <summary>Notifies the observer that the provider has experienced an error condition.</summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    public void OnError(Exception error)
    {
    }

    /// <summary>Provides the observer with new data.</summary>
    /// <param name="value">The current notification information.</param>
    public void OnNext(ConsumerRecord<TKey, TValue> value)
    {
    }

    /// <inheritdoc/>
    public ChannelReader<ConsumerRecord<TKey, TValue>> Subscribe(string topicName)
    {
        if (_channelsByTopics.ContainsKey(topicName))
        {
            throw new ClusterKafkaException("Подписка на данный топик уже существует, повторная подписка не возможна");
        }

        var channel = Channel.CreateBounded<ConsumerRecord<TKey, TValue>>(
            new BoundedChannelOptions(50)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = true,
                SingleReader = false
            });

        var resultChannel = _channelsByTopics.GetOrAdd(topicName, channel);

        StartConsume(topicName);

        return resultChannel.Reader;
    }

    private void StartConsume(string topicName)
    {
    }

    public void Unsubscribe()
    {
        foreach (var channel in _channelsByTopics)
        {
            channel.Value.Writer.Complete();
        }

        _channelsByTopics.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topics"></param>
    public void Unsubscribe(IReadOnlyCollection<string> topics)
    {
        foreach (var topic in topics)
        {
            if (_channelsByTopics.TryRemove(topic, out var channel))
            {
                channel.Writer.Complete();
            }
        }
    }
}