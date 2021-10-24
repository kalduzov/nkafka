using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Protocol.Responses;

namespace Microlibs.Kafka.Protocol
{
    internal sealed class InternalConnection : IConnection
    {
        private readonly struct ResponseCSInfo
        {
            public ResponseCSInfo(ApiKeys apiKey, int correlationId, TaskCompletionSource<ResponseMessage> source)
            {
                ApiKey = apiKey;
                CorrelationId = correlationId;
                Source = source;
            }

            public readonly int CorrelationId;

            public readonly ApiKeys ApiKey;

            public readonly TaskCompletionSource<ResponseMessage> Source;
        }

        private readonly string _address;
        private readonly int _port;
        private readonly Socket _socket;
        private int _requestId;
        private NetworkStream _networkStream;
        private readonly Dictionary<ApiKeys, ApiVersion> _versions = new();
        private readonly ConcurrentDictionary<int, ResponseCSInfo> _tckPool;
        private readonly IRequestBuilder _requestBuilder;
        private readonly Task _responsesReaderTask;
        private bool _disposed = false;
        private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

        private readonly ConcurrentBag<Task> _bag = new();
        private readonly CancellationTokenSource _responseProcessingTokenSource = new();

        public InternalConnection(string address, int port, string clientId)
        {
            _address = address;
            _port = port;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _tckPool = new ConcurrentDictionary<int, ResponseCSInfo>(); //todo вынести в настройки
            _requestBuilder = new DefaultRequestBuilder(clientId);

            _responsesReaderTask = Task.Factory.StartNew(Action, TaskCreationOptions.LongRunning);
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

                    _bag.Add(ParseResponseAsync(buffer,responseLength, _responseProcessingTokenSource.Token));
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
                        responseInfo.Source.SetCanceled();

                        return;
                    }

                    try
                    {
                        var response = DefaultResponseBuilder.Create(responseInfo.ApiKey, binaryReader,responseLength);

                        responseInfo.Source.SetResult(response);
                    }
                    catch (KafkaException exc)
                    {
                        responseInfo.Source.SetException(new KafkaException(exc.InternalError, "Ошибка при чтении запроса", exc));
                    }
                    catch (Exception exc)
                    {
                        responseInfo.Source.SetException(
                            new KafkaException(ErrorCodes.UnknownServerError, "Неизвестная ошибка при чтении запроса", exc));
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

        public Task<TResponseMessage> SendAsync<TResponseMessage>(KafkaRequest request, CancellationToken token)
            where TResponseMessage : ResponseMessage
        {
            var task = InternalSendAsync<TResponseMessage>(request, token);

            return task;
        }

        private async Task<TResponseMessage> InternalSendAsync<TResponseMessage>(KafkaRequest request, CancellationToken token)
            where TResponseMessage : ResponseMessage
        {
            await TryConnectToBrokerAsync(token);
            await CheckApiVersion(request, token);

            var requestId = request.Header.CorrelationId == 0
                ? Interlocked.Increment(ref _requestId)
                : request.Header.CorrelationId;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            var responseCsInfo = new ResponseCSInfo(request.Header.ApiKey, requestId, new TaskCompletionSource<ResponseMessage>(cts));

            try
            {
                var buffer = request.ToByteStream();
                await _networkStream.WriteAsync(buffer, token);
                //await _networkStream.FlushAsync(token);

                _tckPool.TryAdd(requestId, responseCsInfo);
            }
            catch (Exception exc)
            {
                responseCsInfo.Source.SetException(new KafkaException(ErrorCodes.UnknownServerError, "Не удалость отправить запрос", exc));
            }

            return (TResponseMessage)await responseCsInfo.Source.Task;
        }

        private async Task CheckApiVersion(KafkaRequest request, CancellationToken token)
        {
            if (request.Header.ApiKey == ApiKeys.ApiVersions)
            {
                return;
            }

            if (!_versions.ContainsKey(request.Header.ApiKey))
            {
                var requestId = Interlocked.Increment(ref _requestId);

                var apiRequest = _requestBuilder.Create(ApiKeys.ApiVersions, requestId);
                var result = await InternalSendAsync<ApiVersionMessage>(apiRequest, token);
            }
        }

        private async Task TryConnectToBrokerAsync(CancellationToken cancellationToken)
        {
            if (!_socket.Connected && !cancellationToken.IsCancellationRequested)
            {
                await _socket.ConnectAsync(_address, _port);
                _networkStream = new NetworkStream(_socket);
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
                    value.Source.SetException(new ObjectDisposedException(nameof(InternalConnection), "Соединение с брокером было уничтожено"));
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
    }
}