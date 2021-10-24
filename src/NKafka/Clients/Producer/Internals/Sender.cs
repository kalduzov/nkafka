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

using Microsoft.Extensions.Logging;

using NKafka.Config;

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Класс, обрабатывющий 
/// </summary>
internal class Sender: ISender
{
    private readonly ProducerConfig _producerConfig;
    private readonly RecordAccumulator _recordAccumulator;
    private readonly IKafkaCluster _kafkaCluster;
    private readonly ILogger<Sender> _logger;

    public Sender(ProducerConfig producerConfig, RecordAccumulator recordAccumulator, IKafkaCluster kafkaCluster, ILoggerFactory loggerFactory)
    {
        _producerConfig = producerConfig;
        _recordAccumulator = recordAccumulator;
        _kafkaCluster = kafkaCluster;
        _logger = loggerFactory.CreateLogger<Sender>();

    }

    public async void RunAsync(object? cts)
    {
        var oldThreadName = Thread.CurrentThread.Name;
        Thread.CurrentThread.Name = "Kafka producer I/O thread";

        _logger.LogTrace("Starting producer I/O thread");

        try
        {
            if (cts is not CancellationTokenSource cancellationTokenSource)
            {
                throw new ArgumentException("На вход метода ожидался тип 'CancellationTokenSource'", nameof(cts));
            }
            var token = cancellationTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                //todo вызов этого цикла без паузы постоянно нагружает SOH
                await RunOnceAsync(token).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromMilliseconds(_producerConfig.RetryBackoffMs), token).ConfigureAwait(false);
            }

            //todo, после основного цикла нужно подчистить все ресурсы
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "");
        }
        finally
        {
            Thread.CurrentThread.Name = oldThreadName;
        }
    }

    public void Wakeup()
    {
    }

    private async Task RunOnceAsync(CancellationToken token)
    {
        await SendProducerDataAsync(token).ConfigureAwait(false);
    }

    private async Task SendProducerDataAsync(CancellationToken token)
    {
        var result = _recordAccumulator.GetReadyBatches(_kafkaCluster);

        // В батчах присутствуют партиции топиков для которых нет лидера в метаданных кластера
        // такое может происходить в нескольких случаях:
        // 1. Полное обновление метаданных отключено, а продюсер отправляет данные не учитывая реального положения дел
        // 2. Во время работы клиента появился новый топик и по нему еще не пришли обновления из брокеров, мы уже хотим отправить данные по нему.
        if (result.UnknownLeaderTopics.Count > 0)
        {
            _logger.LogDebug("Requesting metadata update due to unknown leader topics from the batched records: {Topics}",
                string.Join(',', result.UnknownLeaderTopics));
            await _kafkaCluster.RefreshMetadataAsync(token, result.UnknownLeaderTopics).ConfigureAwait(false);
        }

        //_recordAccumulator.
    }
}