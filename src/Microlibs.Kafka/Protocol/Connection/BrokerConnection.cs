using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka.Protocol.Connection
{
    internal sealed class BrokerConnection : IKafkaBrokerConnection, IEquatable<BrokerConnection>
    {
        private readonly BrokerEndpoint _brokerEndpoint;

        private class ResponseTaskCompletionSource : TaskCompletionSource<ResponseMessage>
        {
            public ResponseTaskCompletionSource(ApiKeys apiKey, int correlationId)
            {
                ApiKey = apiKey;
                CorrelationId = correlationId;
            }

            public int CorrelationId { get; }

            public ApiKeys ApiKey { get; }
        }

        private readonly Socket _socket;
        private int _requestId;
        private NetworkStream _networkStream;
        private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _tckPool;
        private readonly Task _responsesReaderTask;
        private bool _disposed = false;
        private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

        private readonly ConcurrentBag<Task> _bag = new();
        private readonly CancellationTokenSource _responseProcessingTokenSource = new();

        public BrokerConnection(BrokerEndpoint brokerEndpoint)
        {
            _brokerEndpoint = brokerEndpoint;

            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _tckPool = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(); //todo вынести в настройки

            _responsesReaderTask = Task.Factory.StartNew(Action, TaskCreationOptions.LongRunning);
        }

        public override int GetHashCode()
        {
            return _brokerEndpoint.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is BrokerConnection other && Equals(other);
        }

        private async Task Action()
        {
            BinaryReader? reader = null;

            try
            {
                while (!_responseProcessingTokenSource.IsCancellationRequested)
                {
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
            await Task.Yield();

            try
            {
                await using var memoryStream = new MemoryStream(buffer);
                using var binaryReader = new BinaryReader(memoryStream);

                var requestId = binaryReader.ReadInt32().Swap();

                if (_tckPool.TryGetValue(requestId, out var responseInfo))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        responseInfo.SetCanceled();

                        return;
                    }

                    try
                    {
                        var response = DefaultResponseBuilder.Create(responseInfo.ApiKey, binaryReader, responseLength);

                        responseInfo.SetResult(response);
                    }
                    catch (KafkaException exc)
                    {
                        responseInfo.SetException(new KafkaException(exc.InternalError, "Ошибка при чтении запроса", exc));
                    }
                    catch (Exception exc)
                    {
                        responseInfo.SetException(new KafkaException(ErrorCodes.UnknownServerError, "Неизвестная ошибка при чтении запроса", exc));
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

        public void Send(KafkaRequest request)
        {
        }

        // public Task<TResponseMessage> SendAsync<TResponseMessage>(KafkaRequest request, CancellationToken token)
        //     where TResponseMessage : ResponseMessage
        // {
        //     return InternalSendAsync<TResponseMessage>(request, token);
        // }

        public Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
            where TResponseMessage : ResponseMessage
            where TRequestMessage : RequestMessage
        {
            return InternalSendAsync<TResponseMessage, TRequestMessage>(message, token);
        }

        private async Task<TResponseMessage> InternalSendAsync<TResponseMessage, TRequestMessage>(TRequestMessage message, CancellationToken token)
            where TResponseMessage : ResponseMessage
            where TRequestMessage : RequestMessage
        {
            await ReEstablishConnectionAsync(token);

            //await CheckApiVersion(request, token);

            var requestId = Interlocked.Increment(ref _requestId);

            var request = new KafkaRequest(new RequestHeader(message.ApiKey, 0, requestId, "test"), message);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            var responseCsInfo = new ResponseTaskCompletionSource(request.Header.ApiKey, requestId);

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
                var buffer = request.ToByteStream();
                await _networkStream.WriteAsync(buffer, token);

                _tckPool.TryAdd(requestId, responseCsInfo);
            }
            catch (Exception exc)
            {
                responseCsInfo.SetException(new KafkaException(ErrorCodes.UnknownServerError, "Не удалость отправить запрос", exc));
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
                    await _socket.ConnectAsync(_brokerEndpoint.Host, _brokerEndpoint.Port);
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
                    value.SetException(new ObjectDisposedException(nameof(BrokerConnection), "Соединение с брокером было уничтожено"));
                }

                _tckPool.Clear();
                _responsesReaderTask.Dispose();
                _bag.Clear();

                _networkStream.Dispose();
                _socket.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1.Equals?view=netstandard-2.1">`IEquatable.Equals` on docs.microsoft.com</a></footer>
        public bool Equals(BrokerConnection? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _brokerEndpoint.Equals(other._brokerEndpoint);
        }
    }
}