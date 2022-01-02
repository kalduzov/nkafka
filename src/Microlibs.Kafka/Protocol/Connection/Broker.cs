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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol.Extensions;
using Microsoft.IO;
using static System.Buffers.Binary.BinaryPrimitives;

namespace Microlibs.Kafka.Protocol.Connection;

/// <summary>
/// </summary>
internal sealed class Broker : IBroker, IEquatable<Broker>
{
    private readonly ArrayPool<byte> _arrayPool;
    private readonly RecyclableMemoryStreamManager _streamManager = new();
    private readonly ConcurrentDictionary<int, Task> _responsesTasks = new();
    private readonly CancellationTokenSource _responseProcessingTokenSource = new();
    private readonly Socket _socket;
    private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _tckPool;
    private readonly Timer _cleanerTimer;
    private bool _disposed;
    private NetworkStream _networkStream;
    private volatile int _requestId = -1;

    public IReadOnlyCollection<string> Topics { get; }

    private CommonConfig _config;
    private Task _receivedDataFromSocketTask;
    private readonly Task _processData;

    private readonly Memory<byte> _size = new(new byte[sizeof(int)]);

    /*
     * Когда _globalTimeWaiting.ElapsedMiliseconds превысит параметр ConnectionsMaxIdleMs из конфигурации,
     * соединение с брокером автоматически закроется 
     */
    private readonly Stopwatch _globalTimeWaiting = new();

    /// <summary>
    ///     Флаг указывающий, что данный брокер является контроллером
    /// </summary>
    public bool IsController { get; }

    /// <summary>
    ///     Идентикификатор брокера
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 
    /// </summary>
    public string? Rack { get; }

    /// <summary>
    ///     Точка подключения к брокеру
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    ///     Список партиций топиков связанных с этим брокером
    /// </summary>
    public IReadOnlyCollection<TopicPartition> TopicPartitions { get; }

    public Broker(EndPoint endpoint, int id = -1, string? rack = null)
    {
        Id = id;
        Rack = rack;
        EndPoint = endpoint;

        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _tckPool = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(); //todo вынести в настройки

        _processData = ResponseReaderAction();

        //todo конфигурировать через опции клиента
        var maxMessageSize = 84000;
        var maxRequests = 10;

        _arrayPool = ArrayPool<byte>.Create(maxMessageSize, maxRequests);
    }

    public async Task OpenAsync(CommonConfig commonConfig, CancellationToken token)
    {
        
        _config = commonConfig;

        // var request = new Api
        // await SendAsync<>()
    }

    /// <summary>
    /// Закрывает соединение с токеном
    /// </summary>
    public void Close()
    {
        if (_disposed)
        {
            return;
        }

        _responseProcessingTokenSource.Cancel();

        // Ждем когда остановится обработка потока данных из сокета
        Task.WaitAll(
            new[]
            {
                _receivedDataFromSocketTask,
                _processData
            },
            _config.CloseBrokerTimeoutMs);

        foreach (var value in _tckPool.Values) //Если еще остались необработанные задачи - отменяем их с ошибкой 
        {
            value.SetException(new ObjectDisposedException(nameof(Broker), "Соединение с брокером было уничтожено"));
        }

        _tckPool.Clear();
        _responsesTasks.Clear();
        _networkStream.Dispose();
        _socket.Dispose();
        _cleanerTimer.Dispose();

        _disposed = true;
    }

    /// <summary>
    /// Отправка сообщений брокеру по типу fire and forget
    /// </summary>
    public void Send<TRequestMessage>(TRequestMessage message)
        where TRequestMessage : RequestBody
    {
        CheckDisposed();
    }

    /// <summary>
    ///     Отправка сообщения брокеру и ожидание получения результата этого сообщения
    /// </summary>
    public Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
        where TResponseMessage : KafkaResponseMessage, new()
        where TRequestMessage : RequestBody
    {
        return InternalSendAsync<TResponseMessage, TRequestMessage>(message, token);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool Equals(Broker? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EndPoint.Equals(other.EndPoint);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_receivedDataFromSocketTask, _processData).ConfigureAwait(false);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is Broker other && Equals(other);
    }

    private async Task ResponseReaderAction()
    {
        await Task.Yield();

        try
        {
            var sw = new SpinWait();

            while (!_responseProcessingTokenSource.IsCancellationRequested)
            {
                if (!(_networkStream?.DataAvailable ?? false))
                {
                    sw.SpinOnce();

                    continue;
                }

                await _networkStream.ReadAsync(_size).ConfigureAwait(false);
                var requestLength = ReadInt32BigEndian(_size.Span);

                if (requestLength == 0) //Данных нет. выходим из цикла
                {
                    continue;
                }

                var bodyLen = requestLength - await _networkStream.ReadAsync(_size).ConfigureAwait(false);
                var requestId = ReadInt32BigEndian(_size.Span);

                Debug.WriteLine($"Get new message BrockerId {Id}, CorrelationId={requestId}, ResponseLength={requestLength}");

                var buffer = _arrayPool.Rent(bodyLen);

                await _networkStream.ReadAsync(buffer.AsMemory(0, bodyLen)).ConfigureAwait(false);

                _responsesTasks.TryAdd(
                    requestId,
                    ParseResponseAsync(buffer, requestId, requestLength - 4, _responseProcessingTokenSource.Token));
            }
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);
        }
    }

    private async Task ParseResponseAsync(byte[] buffer, int requestId, int bodyLen, CancellationToken cancellationToken)
    {
        //сразу переключаемся на другой поток, что бы освободить работу для чтения ответов
        await Task.Yield();

        try
        {
            if (_tckPool.TryRemove(requestId, out var responseInfo))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    responseInfo.SetCanceled();

                    return;
                }

                try
                {
                    var message = responseInfo.BuildResponseMessage(buffer.AsSpan(), bodyLen);
                    responseInfo.SetResult(message);
                }
                catch (ProtocolKafkaException exc)
                {
                    responseInfo.SetException(new ProtocolKafkaException(exc.InternalStatus, "Ошибка при чтении запроса", exc));
                }
                catch (Exception exc)
                {
                    responseInfo.SetException(
                        new ProtocolKafkaException(StatusCodes.UnknownServerError, "Неизвестная ошибка при чтении запроса", exc));
                }
                finally
                {
                    _tckPool.TryRemove(requestId, out responseInfo);
                }
            }
            else
            {
                Debug.WriteLine($"Не удалось получить данные по запросу {requestId}");
            }
        }
        finally
        {
            _responsesTasks.TryRemove(requestId, out _);

            _arrayPool.Return(buffer);
        }
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(Broker));
        }
    }

    private async Task<TResponseMessage> InternalSendAsync<TResponseMessage, TRequestMessage>(TRequestMessage content, CancellationToken token)
        where TResponseMessage : KafkaResponseMessage, new()
        where TRequestMessage : RequestBody
    {
        _globalTimeWaiting.Restart(); //Каждый новый запрос перезапускает таймер

        using var _ = KafkaDiagnosticsSource.InternalSendMessage(content.ApiKey, content.Version, _requestId, Id);

        var requestId = Interlocked.Increment(ref _requestId);
        var request = new KafkaRequestMessage(new KafkaRequestHeader(content.ApiKey, content.Version, requestId, _config.ClientId), content);
        await ThrowIfBrokerDontSupportApiVersion(request);

        await ReEstablishConnectionAsync(token);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var taskCompletionSource = new ResponseTaskCompletionSource(request.Header.ApiKey, request.Header.ApiVersion);

        token.Register(
            state =>
            {
                var awaiter = state as ResponseTaskCompletionSource;
                awaiter.TrySetCanceled();
            },
            taskCompletionSource,
            false);

        try
        {
            _tckPool.TryAdd(requestId, taskCompletionSource);

            await using var stream = _streamManager.GetStream();

            request.ToByteStream(stream);
            var bufferForSend = stream.ToArray();
            await _networkStream.WriteAsync(bufferForSend, token);
        }
        catch (Exception exc)
        {
            _tckPool.TryRemove(requestId, out var _); //Если не удалось отправить запрос, то удаляем сообщение 
            taskCompletionSource.SetException(new ProtocolKafkaException(StatusCodes.UnknownServerError, "Не удалость отправить запрос", exc));
        }

        return (TResponseMessage)await taskCompletionSource.Task;
    }

    private async Task ThrowIfBrokerDontSupportApiVersion(KafkaRequestMessage request)
    {
    }

    // private async Task CheckApiVersion(KafkaRequest request, CancellationToken token)
    // {
    //     if (request.Header.ApiKey == ApiKeys.ApiVersions)
    //     {
    //         return;
    //     }
    //
    //     if (!_versions.ContainsKey(request.Header.ApiKey))
    //     {
    //         var requestId = Interlocked.Increment(ref _requestId);
    //
    //         var apiRequest = _requestBuilder.Create(ApiKeys.ApiVersions, requestId);
    //         var result = await InternalSendAsync<ApiVersionMessage>(apiRequest, token);
    //     }
    // }

    private async Task ReEstablishConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!_socket.Connected && !cancellationToken.IsCancellationRequested)
            {
                await _socket.ConnectAsync(EndPoint);
                _networkStream = new NetworkStream(_socket);
                _globalTimeWaiting.Start();
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
        }
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Close();
        }
    }

    private class ResponseTaskCompletionSource : TaskCompletionSource<KafkaResponseMessage>
    {
        private readonly ApiKeys _apiKey;

        private readonly ApiVersions _version;

        public ResponseTaskCompletionSource(ApiKeys apiKey, ApiVersions version)
        {
            _apiKey = apiKey;
            _version = version;
        }

        internal KafkaResponseMessage BuildResponseMessage(ReadOnlySpan<byte> span, int bodyLen)
        {
            return DefaultResponseBuilder.Build(_apiKey, _version, bodyLen, span);
        }
    }
}