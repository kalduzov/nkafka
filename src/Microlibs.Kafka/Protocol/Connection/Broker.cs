using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
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

    private CommonConfig _commonConfig;
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

    //Вычитывает из канала длинну следующего сообщения без учета длинны запроса и id запроса
    private static async Task<(int requestLength, int requestId)> ParseHeaderResponseAsync(PipeReader reader, CancellationToken token)
    {
        var requestLength = -1;
        var requestId = -1;

        while (!token.IsCancellationRequested)
        {
            var result = await reader.ReadAsync(token); //Читаем первый кусок данных
            var buffer = result.Buffer;

            if (buffer.IsEmpty)
            {
                break;
            }

            if (buffer.Length <= 8) //Если длинна буфера меньше 8 (размер 2х int'ов)
            {
                // То сообщаем что мы ничего не прочитали из канала и пробуем в цикле получить новый буфер с другой длинной.
                // Текущие данные вернуться в новом буфере
                reader.AdvanceTo(buffer.Start);

                continue;
            }

            // у буфер подходящей длины, теперь мы можем читать из него необходимые данные 
            buffer.FirstSpan
                .ReadInt32(out requestLength)
                .ReadInt32(out requestId);

            // сообщаем что мы прочитали из буфера 8 байт, теперь следующее чтение из канала вернет буфер без учета прочитанных байт
            reader.AdvanceTo(buffer.GetPosition(8));

            break;
        }

        return (requestLength - 4, requestId); // Возвращаем длинну оставшейся части сообщения и id запроса
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
        where TRequestMessage : KafkaContent
    {
        _globalTimeWaiting.Restart(); //Каждый новый запрос перезапускает таймер

        using var _ = KafkaDiagnosticsSource.InternalSendMessage(content.ApiKey, content.Version, _requestId, Id);

        await ReEstablishConnectionAsync(token);

        //await CheckApiVersion(request, token);

        var requestId = Interlocked.Increment(ref _requestId);

        var request = new KafkaRequestMessage(new KafkaRequestHeader(content.ApiKey, content.Version, requestId, "test"), content);

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
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _responseProcessingTokenSource.Cancel();

            // Ждем когда остановится обработка потока данных из сокета
            Task.WaitAll(_receivedDataFromSocketTask, _processData);

            foreach (var value in _tckPool.Values) //Если еще остались необработанные задачи - отменяем их с ошибкой 
            {
                value.SetException(new ObjectDisposedException(nameof(Broker), "Соединение с брокером было уничтожено"));
            }

            _tckPool.Clear();
            _responsesTasks.Clear();
            _networkStream.Dispose();
            _socket.Dispose();
            _cleanerTimer.Dispose();
        }

        _disposed = true;
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