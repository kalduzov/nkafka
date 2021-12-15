using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Microlibs.Kafka.Protocol.Extensions;
using Microsoft.IO;

namespace Microlibs.Kafka.Protocol.Connection;

/// <summary>
/// </summary>
internal sealed class Broker : IBroker, IEquatable<Broker>
{
    private readonly RecyclableMemoryStreamManager _streamManager = new();
    private readonly ArrayPool<byte> _arrayPool;
    private readonly ConcurrentDictionary<int, Task> _responsesTasks = new();
    private readonly CancellationTokenSource _responseProcessingTokenSource = new();
    private readonly Task _responsesReaderTask;
    private readonly Socket _socket;
    private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _tckPool;
    private readonly Timer _cleanerTimer;
    private bool _disposed;
    private NetworkStream _networkStream;
    private volatile int _requestId;
    private readonly ManualResetEvent _manualResetEvent;

    public IReadOnlyCollection<string> Topics { get; }

    private CommonConfig _commonConfig;

    /// <summary>
    ///     Флаг указывающий, что данный брокер является контроллером
    /// </summary>
    public bool IsController { get; }

    /// <summary>
    ///     Идентикификатор брокера
    /// </summary>
    public int Id { get; }

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

        _manualResetEvent = new ManualResetEvent(false);
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _tckPool = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(); //todo вынести в настройки
        _responsesReaderTask = Task.Factory.StartNew(
            ResponseReaderAction,
            _responseProcessingTokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);

        _cleanerTimer = new Timer(_ => ClearingBugWithResponses(), null, 0, 1000);

        //todo конфигурировать через опции клиента
        var maxMessageSize = 84000;
        var maxRequests = 10;

        _arrayPool = ArrayPool<byte>.Create(maxMessageSize, maxRequests);
    }

    private void ClearingBugWithResponses()
    {
        var cts = new CancellationTokenSource(1000 / 2);

        while (!cts.IsCancellationRequested)
        {
            // if (!_responsesTasks.(out var task))
            // {
            //     return;
            // }
            //
            // if (task.Status != TaskStatus.RanToCompletion)
            // {
            //     _responsesTasks.Add(task);
            // }
        }

        if (_responsesTasks.IsEmpty)
        {
            _manualResetEvent.Reset();
        }
        else
        {
            _manualResetEvent.Set();
        }
    }

    public async Task OpenAsync(CommonConfig commonConfig, CancellationToken token)
    {
        _commonConfig = commonConfig;
        
        // var request = new Api
        // await SendAsync<>()
    }

    /// <summary>
    /// Отправка сообщений брокеру по типу fire and forget
    /// </summary>
    public void Send<TRequestMessage>(TRequestMessage message)
        where TRequestMessage : KafkaContent
    {
        CheckDisposed();
    }

    /// <summary>
    ///     Отправка сообщения брокеру и ожидание получения результата этого сообщения
    /// </summary>
    public Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
        where TResponseMessage : KafkaResponseMessage, new()
        where TRequestMessage : KafkaContent
    {
        return InternalSendAsync<TResponseMessage, TRequestMessage>(message, token);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    /// <footer>
    ///     <a href="https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1.Equals?view=netstandard-2.1">
    ///         `IEquatable.Equals`
    ///         on docs.microsoft.com
    ///     </a>
    /// </footer>
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
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is Broker other && Equals(other);
    }

    private async Task ResponseReaderAction()
    {
        BinaryReader? reader = null;

        try
        {
            while (!_responseProcessingTokenSource.IsCancellationRequested)
            {
                _manualResetEvent.WaitOne();

                if (_disposed)
                {
                    return;
                }

                if (!_socket.Connected)
                {
                    await Task.Delay(250);

                    continue;
                }

                if (reader is not null && _networkStream is not null)
                {
                    reader = new BinaryReader(_networkStream);
                }

                if (!(_networkStream?.DataAvailable ?? false))
                {
                    continue;
                }

                reader ??= new BinaryReader(_networkStream);

                var responseLength = reader.ReadInt32().Swap();
                var requestId = reader.ReadInt32().Swap();

                var buffer = _arrayPool.Rent(responseLength - 4);
                reader.Read(buffer, 0, responseLength - 4);

                _responsesTasks.TryAdd(requestId, ParseResponseAsync(buffer, requestId, _responseProcessingTokenSource.Token));
            }
        }
        finally
        {
            reader?.Dispose();
        }
    }

    private async Task ParseResponseAsync(byte[] buffer, int requestId, CancellationToken cancellationToken)
    {
        //сразу переключаемся на другой поток, что бы освободить работу для чтения ответов
        await Task.Yield();

        try
        {
            await using var stream = _streamManager.GetStream(buffer) as RecyclableMemoryStream;

            if (_tckPool.TryRemove(requestId, out var responseInfo))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    responseInfo.SetCanceled();

                    return;
                }

                try
                {
                    responseInfo.ResponseMessage.DeserializeFromStream(stream);
                    responseInfo.SetResult(responseInfo.ResponseMessage);
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
                Console.WriteLine($"Не удалось получить данные по запросу {requestId}");
            }
        }
        finally
        {
            _responsesTasks.TryRemove(requestId, out _);
            _arrayPool.Return(buffer, true);
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
        where TRequestMessage : KafkaContent
    {
        using var _ = KafkaDiagnosticsSource.InternalSendMessage(content.ApiKey, content.Version, _requestId, Id);

        await ReEstablishConnectionAsync(token);

        //await CheckApiVersion(request, token);

        var requestId = Interlocked.Increment(ref _requestId);

        var request = new KafkaRequestMessage(new KafkaRequestHeader(content.ApiKey, content.Version, requestId, "test"), content);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var responseMessage = new TResponseMessage();
        var taskCompletionSource = new ResponseTaskCompletionSource(request.Header.ApiKey, request.Header.ApiVersion, requestId, responseMessage);

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
            await using (var stream = _streamManager.GetStream())
            {
                request.ToByteStream(stream);
                var bufferForSend = stream.ToArray();
                await _networkStream.WriteAsync(bufferForSend, token);
                _manualResetEvent.Set();
            }

            _tckPool.TryAdd(requestId, taskCompletionSource);
        }
        catch (Exception exc)
        {
            taskCompletionSource.SetException(new ProtocolKafkaException(StatusCodes.UnknownServerError, "Не удалость отправить запрос", exc));
        }

        return (TResponseMessage)await taskCompletionSource.Task;
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
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
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
            _manualResetEvent.Dispose();
            _responseProcessingTokenSource.Cancel();

            foreach (var value in _tckPool.Values)
            {
                value.SetException(new ObjectDisposedException(nameof(Broker), "Соединение с брокером было уничтожено"));
            }

            _tckPool.Clear();
            _responsesReaderTask.Dispose();
            _responsesTasks.Clear();

            _networkStream.Dispose();
            _socket.Dispose();
            _cleanerTimer.Dispose();
        }

        _disposed = true;
    }

    private class ResponseTaskCompletionSource : TaskCompletionSource<KafkaResponseMessage>
    {
        public ResponseTaskCompletionSource(ApiKeys apiKey, ApiVersions version, int correlationId, KafkaResponseMessage responseMessage)
        {
            ApiKey = apiKey;
            CorrelationId = correlationId;
            ResponseMessage = responseMessage;
            responseMessage.Version = version;
        }

        private int CorrelationId { get; }

        public KafkaResponseMessage ResponseMessage { get; }

        private ApiKeys ApiKey { get; }
    }
}