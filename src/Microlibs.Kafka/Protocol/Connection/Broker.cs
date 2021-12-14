using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly ConcurrentBag<Task> _bag = new();
    private readonly CancellationTokenSource _responseProcessingTokenSource = new();
    private readonly Task _responsesReaderTask;
    private readonly Socket _socket;
    private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _tckPool;
    private readonly Timer _bagCleanerTimer;
    private bool _disposed;
    private NetworkStream _networkStream;
    private volatile int _requestId;

    public IReadOnlyCollection<string> Topics { get; }

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

        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _tckPool = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(); //todo вынести в настройки
        _responsesReaderTask = Task.Factory.StartNew(ResponseReaderAction, TaskCreationOptions.LongRunning);

        _bagCleanerTimer = new Timer(
            _ =>
            {
                if (!_bag.TryTake(out var task))
                {
                    return;
                }

                if (task.Status != TaskStatus.RanToCompletion)
                {
                    _bag.Add(task);
                }
            },
            this,
            0,
            100);

        //todo конфигурировать через опции клиента
        var maxMessageSize = 84000;
        var maxRequests = 10;

        _arrayPool = ArrayPool<byte>.Create(maxMessageSize, maxRequests);
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
                var buffer = _arrayPool.Rent(responseLength);
                reader.Read(buffer, 0, responseLength);

                _bag.Add(ParseResponseAsync(buffer, responseLength, _responseProcessingTokenSource.Token));
            }
        }
        finally
        {
            reader?.Dispose();
        }
    }

    private async Task ParseResponseAsync(byte[] buffer, int responseLength, CancellationToken cancellationToken)
    {
        //сразу переключаемся на другой поток, что бы освободить работу для чтения ответов
        await Task.Yield();

        try
        {
            await using var stream = _streamManager.GetStream(buffer) as RecyclableMemoryStream;

            var memory = stream.GetMemory();
            var requestId = BinaryPrimitives.ReadInt32BigEndian(memory[..4].Span);

            if (_tckPool.TryGetValue(requestId, out var responseInfo))
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
        }
        finally
        {
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
        await ReEstablishConnectionAsync(token);

        //await CheckApiVersion(request, token);

        var requestId = Interlocked.Increment(ref _requestId);

        var request = new KafkaRequestMessage(new KafkaRequestHeader(content.ApiKey, content.Version, requestId, "test"), content);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var response = new TResponseMessage();
        var responseCsInfo = new ResponseTaskCompletionSource(request.Header.ApiKey, request.Header.ApiVersion, requestId, response);

        token.Register(
            state =>
            {
                var awaiter = state as ResponseTaskCompletionSource;
                awaiter.TrySetCanceled();
            },
            responseCsInfo,
            false);

        try
        {
            await using (var stream = _streamManager.GetStream())
            {
                request.ToByteStream(stream);
                var bufferForSend = stream.ToArray();
                await _networkStream.WriteAsync(bufferForSend, token);
            }

            _tckPool.TryAdd(requestId, responseCsInfo);
        }
        catch (Exception exc)
        {
            responseCsInfo.SetException(new ProtocolKafkaException(StatusCodes.UnknownServerError, "Не удалость отправить запрос", exc));
        }

        return (TResponseMessage)await responseCsInfo.Task;
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
            _responseProcessingTokenSource.Cancel();

            foreach (var value in _tckPool.Values)
            {
                value.SetException(new ObjectDisposedException(nameof(Broker), "Соединение с брокером было уничтожено"));
            }

            _tckPool.Clear();
            _responsesReaderTask.Dispose();
            _bag.Clear();

            _networkStream.Dispose();
            _socket.Dispose();
            _bagCleanerTimer.Dispose();
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