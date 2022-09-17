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

using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Diagnostics;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Connection;

/// <summary>
/// </summary>
internal sealed class Broker: IBroker, IEquatable<Broker>
{
    private readonly ArrayPool<byte> _arrayPool;
    private readonly ConcurrentDictionary<int, Task> _responsesTasks = new();
    private readonly CancellationTokenSource _responseProcessingTokenSource = new();
    private readonly Socket _socket;
    private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _tckPool;
    private bool _disposed;
    private Stream _stream;
    private volatile int _requestId = -1;

    private readonly CommonConfig _config;

    private Task _processData;

    /*
     * Когда _globalTimeWaiting.ElapsedMiliseconds превысит параметр ConnectionsMaxIdleMs из конфигурации,
     * соединение с брокером автоматически закроется 
     */
    private readonly Stopwatch _globalTimeWaiting = new();
    private volatile IReadOnlyDictionary<string, TopicPartition> _topicPartitions;
    private volatile IReadOnlySet<string> _topics;
    private readonly Timer _closeConnectionAfterTimeout;

    private readonly SupportVersions _supportVersions = new(); //Какие версии поддерживает данный брокер

    /// <summary>
    ///     Флаг указывающий, что данный брокер является контроллером
    /// </summary>
    public bool IsController { get; private set; }

    /// <summary>
    ///     Идентикификатор брокера
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 
    /// </summary>
    public string? Rack { get; private set; }

    /// <summary>
    ///     Точка подключения к брокеру
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    ///     Список партиций топиков связанных с этим брокером
    /// </summary>
    public IReadOnlyDictionary<string, TopicPartition> TopicPartitions => _topicPartitions;

    /// <summary>
    /// The current number of running inflight requests
    /// </summary>
    public int CurrentNumberInflightRequests => _tckPool.Count;

    /// <summary>
    /// Топики
    /// </summary>
    public IReadOnlySet<string> Topics => _topics;

    public Broker(
        CommonConfig clusterConfig,
        ILoggerFactory loggerFactory,
        EndPoint endpoint,
        int id = -1,
        string? rack = null,
        bool isController = false)
    {
        _config = clusterConfig;
        Id = id;
        Rack = rack;
        EndPoint = endpoint;
        IsController = isController;

        var maxInflightRequests = _config.MaxInflightRequests;

        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        //todo потом поиграться с параметрами структур
        _tckPool = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(Environment.ProcessorCount, maxInflightRequests);
        _arrayPool = ArrayPool<byte>.Create(_config.MessageMaxBytes, maxInflightRequests);
        _topics = new HashSet<string>(0);
        _topicPartitions = new Dictionary<string, TopicPartition>(0);

        _processData = Task.CompletedTask; //init value

        _closeConnectionAfterTimeout = new Timer(
            _ =>
            {
                if (!_globalTimeWaiting.IsRunning || _globalTimeWaiting.ElapsedMilliseconds <= _config.ConnectionsMaxIdleMs)
                {
                    return;
                }

                ResetConnection();
                _globalTimeWaiting.Reset();
            });
    }

    private void ResetConnection()
    {
        _socket.Close(_config.CloseConnectionTimeoutMs);
    }

    public async Task OpenAsync(CancellationToken token)
    {
        if (_config.ApiVersionRequest)
        {
            var request = new ApiVersionsRequestMessage();
            var response = await InternalSendAsync<ApiVersionsResponseMessage, ApiVersionsRequestMessage>(request, true, token)
                .ConfigureAwait(false);
            UpdateApiVersions(response);
        }
    }

    private void UpdateApiVersions(ApiVersionsResponseMessage response)
    {
        if (response.Code != ErrorCodes.None)
        {
            return;
        }

        foreach (var key in response.ApiKeys)
        {
            _supportVersions.AddOrUpdate((ApiKeys)key.ApiKey, (ApiVersion)key.MinVersion, (ApiVersion)key.MaxVersion);
        }
    }

    /// <summary>
    /// Закрывает соединение с брокером
    /// </summary>
    public async Task CloseAsync(CancellationToken token)
    {
        _responseProcessingTokenSource.Cancel();

        // Ждем когда остановится обработка потока данных из сокета
        await _processData.WaitAsync(TimeSpan.FromMilliseconds(_config.CloseConnectionTimeoutMs), token);

        foreach (var value in _tckPool.Values) //Если еще остались необработанные задачи - отменяем их с ошибкой 
        {
            value.SetException(new ObjectDisposedException(nameof(Broker), "Broker connection was closed"));
        }

        _tckPool.Clear();
        _responsesTasks.Clear();
        await _stream.DisposeAsync();
        _socket.Dispose();
    }

    /// <summary>
    /// Отправка сообщений брокеру по типу fire and forget
    /// </summary>
    void IBroker.Send<TRequestMessage>(TRequestMessage message)
    {
        CheckDisposed();
    }

    /// <summary>
    ///     Отправка сообщения брокеру и ожидание получения результата этого сообщения
    /// </summary>
    Task<TResponseMessage> IBroker.SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
    {
        //return _kafkaConnector.SendAsync<TResponseMessage, TRequestMessage>(message, false, token);
        return InternalSendAsync<TResponseMessage, TRequestMessage>(message, false, token);
    }

    /// <summary>
    /// Обновляет информацию о брокере
    /// </summary>
    void IBroker.UpdateInfo(EndPoint endpoint, string? rack, bool isController)
    {
        if (EndPoint != endpoint)
        {
            EndPoint = endpoint;
        }

        if (Rack != rack)
        {
            Rack = rack;
        }

        IsController = isController;
    }

    void IBroker.UpdateTopicsAndPartitions(string messageTopicName, Partition partition)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Broker()
    {
        Dispose(false);
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

    public async ValueTask DisposeAsync()
    {
        if (!_responseProcessingTokenSource.IsCancellationRequested)
        {
            _responseProcessingTokenSource.Cancel();
        }

        await _processData.ConfigureAwait(false);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is Broker other && Equals(other));
    }

    private async Task ResponseReaderTask()
    {
        /*
         * Задача на чтение запускается при постановке нового запроса в очередь ожидания 
         * Задача не завершается, пока в очереди запросов есть хотя бы один не обработанный запрос
         */
        await Task.Yield();

        try
        {
            var sw = new SpinWait();

            while (!_responseProcessingTokenSource.IsCancellationRequested)
            {
                if (_tckPool.IsEmpty)
                {
                    return;
                }

                if (!(_stream?.CanRead ?? false))
                {
                    sw.SpinOnce();

                    continue;
                }

                Memory<byte> size = new(new byte[sizeof(int)]);
                var countReadBytes = await _stream.ReadAsync(size).ConfigureAwait(false);

                var requestLength = ReadInt32BigEndian(size.Span);

                if (requestLength == 0) //Данных нет идем дальше ждать
                {
                    if (countReadBytes == 4)
                    {
                        continue;
                    }

                    throw new ProtocolKafkaException(ErrorCodes.None, "Отправлен некорректный запрос к брокеру. Брокер вернул 0 байт.");
                }

                var bodyLen = requestLength - await _stream.ReadAsync(size).ConfigureAwait(false);
                var requestId = ReadInt32BigEndian(size.Span);

                Debug.WriteLine($"Get new message BrokerId {Id}, CorrelationId={requestId}, ResponseLength={requestLength}");

                var buffer = _arrayPool.Rent(bodyLen);

                await _stream.ReadAsync(buffer.AsMemory(0, bodyLen)).ConfigureAwait(false);

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
                    responseInfo.SetCanceled(cancellationToken);

                    return;
                }

                try
                {
                    var message = responseInfo.BuildResponseMessage(buffer);
                    UpdateResponseMetrics(message.ThrottleTimeMs, bodyLen);

                    if (message.IsSuccessStatusCode)
                    {
                        responseInfo.SetResult(message);
                    }
                    else
                    {
                        throw new ProtocolKafkaException(message.Code, "");
                    }
                }
                catch (ProtocolKafkaException exc)
                {
                    responseInfo.SetException(exc);
                }
                catch (Exception exc)
                {
                    responseInfo.SetException(
                        new ProtocolKafkaException(ErrorCodes.UnknownServerError, "Неизвестная ошибка при чтении запроса", exc));
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

    private void UpdateResponseMetrics(int messageThrottleTimeMs, int bodyLen)
    {
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(Broker));
        }
    }

    private async Task<TResponseMessage> InternalSendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage content,
        bool isInternalRequest,
        CancellationToken token)
        where TResponseMessage : IResponseMessage
        where TRequestMessage : IRequestMessage
    {
        if (_tckPool.Count >= _config.MaxInflightRequests)
        {
            throw new ProtocolKafkaException(
                ErrorCodes.None,
                $"Количество ожидающих ответов запросов к брокеру превысило ограничение в '{_config.MaxInflightRequests}' единиц");
        }

        _globalTimeWaiting.Restart(); //Каждый новый запрос перезапускает таймер

        var contentVersion = ApiVersion.Version0; //content.ApiKey.GetApiVersion(_supportVersions);
        var headerVersion = content.ApiKey.GetHeaderVersion(contentVersion);
        var requestId = Interlocked.Increment(ref _requestId);

        using var activity = KafkaDiagnosticsSource.InternalSendMessage(content.ApiKey, contentVersion, requestId, Id, EndPoint);

        await ReEstablishConnectionAsync(token);

        var request = new SendMessage(
            new RequestHeader
            {
                ClientId = _config.ClientId,
                RequestApiVersion = (short)headerVersion, //header minimum version
                CorrelationId = requestId,
                RequestApiKey = (short)content.ApiKey
            },
            content,
            contentVersion);

        if (!isInternalRequest)
        {
            ThrowExceptionIfRequestNotValid(request, activity);
        }

        if (token.IsCancellationRequested)
        {
            return await Task.FromCanceled<TResponseMessage>(token);
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var taskCompletionSource = new ResponseTaskCompletionSource(
            (ApiKeys)request.Header.RequestApiKey,
            (ApiVersion)request.Header.RequestApiVersion);

        token.Register(
            state =>
            {
                if (state is not ResponseTaskCompletionSource awaiter)
                {
                    return;
                }

                awaiter.TrySetCanceled();
                _tckPool.TryRemove(requestId, out _);
            },
            taskCompletionSource,
            false);

        try
        {
            _tckPool.TryAdd(requestId, taskCompletionSource);
            WakeupProcessingResponses(); //"пробуждаем" обработку ответов на запрос

            if (_stream.CanWrite)
            {
                request.Write(_stream);
            }
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            _tckPool.TryRemove(requestId, out _); //Если не удалось отправить запрос, то удаляем сообщение

            if (!taskCompletionSource.Task.IsCanceled || !taskCompletionSource.Task.IsCompleted)
            {
                taskCompletionSource.SetException(new ProtocolKafkaException(ErrorCodes.UnknownServerError, "Не удалось отправить запрос", exc));
            }
        }

        return (TResponseMessage)await taskCompletionSource.Task;
    }

    private void WakeupProcessingResponses()
    {
        if (_processData.Status == TaskStatus.RanToCompletion)
        {
            _processData = ResponseReaderTask();
        }
    }

    private void ThrowExceptionIfRequestNotValid(SendMessage message, Activity? activity)
    {
        // if (message.RequestLength >= _config.MessageMaxBytes)
        // {
        //     var logMessage = $"Размер запроса превышает допустимый предел указанный в конфигурации {_config.MessageMaxBytes}";
        //     activity?.SetStatus(ActivityStatusCode.Error);
        //     activity?.AddTag("error.message", logMessage);
        //
        //     throw new ProtocolKafkaException(ErrorCodes.MessageTooLarge, logMessage);
        // }
    }

    private void ThrowIfBrokerDontSupportApiVersion(SendMessage message)
    {
    }

    private async Task ReEstablishConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!_socket.Connected && !cancellationToken.IsCancellationRequested)
            {
                await _socket.ConnectAsync(EndPoint, cancellationToken);
                Stream stream = new NetworkStream(_socket, true);

                if (_config.SecurityProtocol is SecurityProtocols.Ssl or SecurityProtocols.SaslSsl)
                {
                    stream = new SslStream(stream, false, (_, _, _, _) => true); //skip server cert validation
                }

                _stream = stream;
                _globalTimeWaiting.Start();

                if (_config.SecurityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
                {
                    await AuthenticateProcessAsync(cancellationToken);
                }
            }
        }
        catch (SocketException exc)
        {
            Debug.WriteLine(exc.Message);

            throw;
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);
        }
    }

    private async Task AuthenticateProcessAsync(CancellationToken token)
    {
        var saslHandshakeRequest = new SaslHandshakeRequestMessage();
        var response = await InternalSendAsync<SaslHandshakeResponseMessage, SaslHandshakeRequestMessage>(saslHandshakeRequest, true, token);

        if (response.Code == ErrorCodes.None)
        {
            var authenticateRequest = new SaslAuthenticateRequestMessage
            {
            };
            var r = await InternalSendAsync<SaslAuthenticateResponseMessage, SaslAuthenticateRequestMessage>(authenticateRequest, true, token);
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
        }

        CloseAsync(CancellationToken.None);

        _disposed = true;
    }

    private class ResponseTaskCompletionSource: TaskCompletionSource<IResponseMessage>
    {
        private readonly ApiKeys _apiKey;

        private readonly ApiVersion _version;

        public ResponseTaskCompletionSource(ApiKeys apiKey, ApiVersion version)
            : base(TaskCreationOptions.RunContinuationsAsynchronously)
        {
            _apiKey = apiKey;
            _version = version;
        }

        internal IResponseMessage BuildResponseMessage(byte[] span)
        {
            return ResponseBuilder.Build(_apiKey, _version, span);
        }
    }
}