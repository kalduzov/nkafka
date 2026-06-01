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

using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Diagnostics;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;

namespace NKafka.Connection;

/// <summary>
/// 
/// </summary>
internal sealed partial class KafkaConnector: IKafkaConnector
{
    public static readonly IKafkaConnector Null = new NullConnector();

    private long _totalBytesSent;

    private readonly bool _apiVersionRequest;
    private readonly ArrayPool<byte> _arrayPool;
    private readonly string _clientId;

    // ReSharper disable once NotAccessedField.Local
#pragma warning disable IDE0052

    //Нужно убрать этот механизм в KafkaConnectorPool
    private readonly Timer _closeConnectionAfterTimeout;

#pragma warning restore IDE0052
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;

    /*
     * Когда _globalTimeWaiting.ElapsedMilliseconds превысит параметр ConnectionsMaxIdleMs из конфигурации,
     * соединение с брокером автоматически закроется
     */
    private readonly Stopwatch _globalTimeWaiting = new();

    private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _inFlightRequests;
    private readonly ILogger<KafkaConnector> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly int _maxInflightRequests;
    private readonly int _messageMaxBytes;
    private readonly int _requestTimeoutMs;

    private CancellationTokenSource _responseProcessingTokenSource = new();
    private int _responseProcessingSessionId;
    private readonly object _responseReaderSync = new();
    private readonly ConcurrentDictionary<int, Task> _responsesTasks = new();
    private readonly SaslSettings _saslSettings;
    private readonly SecurityProtocols _securityProtocol;
    private readonly ISocketFactory _socketFactory;

    private readonly ISocketProxy _socketProxy;
    private readonly SslSettings _sslSettings;

    // todo со временем нужно убрать обработку сообщений из коннектора в KafkaConnectorPool на отдельные потоки
    // так будет тратиться меньше ресурсов ThreadPool, а сама обработка будет максимально утилизироваться на выделенных потоках
    private Task _processData = Task.CompletedTask;
    private volatile int _requestId = -1;
    private Stream _stream = Stream.Null;

    /// <inheritdoc/>
    public Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)> SupportVersions { get; private set; } = [];

    /// <summary>
    /// Можно ли писать в текущий поток?
    /// </summary>
    private bool CanWrite => _stream != Stream.Null && _stream.CanWrite;

    /// <inheritdoc/>
    public bool IsDedicated { get; init; }

    /// <inheritdoc/>
    public int NodeId { get; set; } = Node.NoNode.Id;

    /// <inheritdoc/>
    public State ConnectorState { get; private set; } = State.Closed;

    /// <inheritdoc/>
    public int CurrentNumberInflightRequests => _inFlightRequests.Count;

    /// <inheritdoc/>
    public EndPoint Endpoint { get; }

    internal KafkaConnector(
        EndPoint endPoint,
        int maxInflightRequests,
        int messageMaxBytes,
        int closeConnectionTimeoutMs,
        int connectionsMaxIdleMs,
        int requestTimeoutMs,
        int receiveBufferBytes,
        SecurityProtocols securityProtocol,
        SaslSettings saslSettings,
        SslSettings sslSettings,
        string clientId,
        bool apiVersionRequest,
        ISocketFactory socketFactory,
        ILoggerFactory loggerFactory)
    {
        Endpoint = endPoint;
        _maxInflightRequests = maxInflightRequests;
        _messageMaxBytes = messageMaxBytes;
        _closeConnectionTimeoutMs = closeConnectionTimeoutMs;
        _connectionsMaxIdleMs = connectionsMaxIdleMs;
        _requestTimeoutMs = requestTimeoutMs;
        _securityProtocol = securityProtocol;
        _saslSettings = saslSettings;
        _sslSettings = sslSettings;
        _clientId = clientId;
        _apiVersionRequest = apiVersionRequest;
        _socketFactory = socketFactory;
        _loggerFactory = loggerFactory;

        _arrayPool = ArrayPool<byte>.Shared;
        _inFlightRequests = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(Environment.ProcessorCount, _maxInflightRequests);
        _logger = loggerFactory.CreateLogger<KafkaConnector>();

        _socketProxy = _socketFactory.CreateSocket(SocketType.Stream, ProtocolType.Tcp, receiveBufferBytes);

        _closeConnectionAfterTimeout = new Timer(_ =>
        {
            if (!_globalTimeWaiting.IsRunning || _globalTimeWaiting.ElapsedMilliseconds <= _connectionsMaxIdleMs)
            {
                return;
            }

            ResetConnection();
            _globalTimeWaiting.Reset();
        });
    }

    public async ValueTask OpenAsync(CancellationToken token)
    {
        await EnsureSessionEstablishedAsync(token);
    }

    async Task<TResponseMessage> IKafkaConnector.SendAsync<TRequestMessage, TResponseMessage>(
        TRequestMessage message,
        bool isInternalRequest,
        CancellationToken token)
    {
        _logger.SendRequestTrace(message, NodeId);

        if (_inFlightRequests.Count >= _maxInflightRequests)
        {
            throw new ProtocolKafkaException(
                ErrorCodes.None,
                $"Too many pending broker requests: '{_maxInflightRequests}' limit reached");
        }

        _globalTimeWaiting.Restart(); //Каждый новый запрос перезапускает таймер

        if (message is not ApiVersionsRequestMessage)
        {
            await EnsureSessionEstablishedAsync(token);
        }

        if (token.IsCancellationRequested)
        {
            return await Task.FromCanceled<TResponseMessage>(token);
        }

        var contentVersion = message.ApiKey.GetEffectiveApiVersion(SupportVersions);
        var headerVersion = message.ApiKey.GetRequestHeaderVersion(contentVersion);
        var requestId = Interlocked.Increment(ref _requestId);

        using var activity = KafkaDiagnosticsSource.InternalSendMessage(message.ApiKey, contentVersion, requestId, NodeId, Endpoint);

        var arrayBuffer = ArrayBufferPool.Rent(_messageMaxBytes);

        try
        {
            var requestHeader = new RequestHeader
            {
                ClientId = _clientId,
                RequestApiVersion = (short)contentVersion,
                CorrelationId = requestId,
                RequestApiKey = (short)message.ApiKey
            };

            var request = new SendMessage(
                requestHeader,
                message,
                contentVersion,
                headerVersion,
                arrayBuffer
            );

            if (!isInternalRequest)
            {
                ThrowExceptionIfRequestNotValid(request, activity);
            }

            var responseCompletionSource = new ResponseTaskCompletionSource(
                (ApiKeys)request.Header.RequestApiKey,
                (ApiVersion)request.Header.RequestApiVersion);

            using var requestLifetimeCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            requestLifetimeCts.CancelAfter(_requestTimeoutMs);

            // The registration only needs synchronous teardown because it just detaches
            // the callback from the current request lifetime and does not own async cleanup.
            using IDisposable cancellationRegistration = requestLifetimeCts.Token.Register(
                state =>
                {
                    var context = (RequestLifetimeContext)state!;
                    context.Connector.CompleteRequestFromLifetimeCancellation(
                        context.RequestId,
                        context.ResponseCompletionSource,
                        context.CallerToken);
                },
                new RequestLifetimeContext(this, requestId, responseCompletionSource, token),
                false);

            try
            {
                if (!_inFlightRequests.TryAdd(requestId, responseCompletionSource))
                {
                    throw new ConnectionKafkaException($"Request with correlation id {requestId} is already registered for NodeId={NodeId}.");
                }

                // The response may arrive as soon as the broker accepts the frame, so the
                // request must be visible in the inflight registry before any bytes are written.
                WakeupProcessingResponses(); //"пробуждаем" обработку ответов на запрос

                if (CanWrite)
                {
                    var bytesSent = await request.WriteToStream(_stream, true, _messageMaxBytes);
                    Debug.WriteLine("Send request {0}, Size={1}", request.RequestMessage.ApiKey, bytesSent);
                    _totalBytesSent = Interlocked.Add(ref _totalBytesSent, bytesSent);
                }
                else
                {
                    throw new ConnectionKafkaException($"Текущее соединение по адресу {Endpoint} к брокеру {NodeId} не может отправлять запросы");
                }
            }
            catch (Exception exc)
            {
                activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

                _inFlightRequests.TryRemove(requestId, out _); //Если не удалось отправить запрос, то удаляем сообщение

                if (!responseCompletionSource.Task.IsCompleted)
                {
                    CompleteRequestAsWriteFailed(
                        responseCompletionSource,
                        exc);
                }
            }

            return (TResponseMessage)await responseCompletionSource.Task;
        }
        finally
        {
            ArrayBufferPool.Return(arrayBuffer);
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous dispose operation.
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        await CloseConnectionCoreAsync(CreateClosedException("Connector is being disposed."));
        GC.SuppressFinalize(this);
    }

    private void ResetConnection()
    {
        _logger.ConnectionResetInformation(Endpoint, NodeId, _connectionsMaxIdleMs);
        CloseConnectionCore(CreateClosedException("Connection was reset."));
    }

    private void WakeupProcessingResponses()
    {
        if (ConnectorState is State.Closing or State.Closed or State.Faulted)
        {
            return;
        }

        lock (_responseReaderSync)
        {
            if (!_processData.IsCompleted)
            {
                return;
            }

            var sessionId = _responseProcessingSessionId;
            var responseProcessingToken = _responseProcessingTokenSource.Token;

            // A physical session must have exactly one active response reader so that
            // correlation ids are consumed in arrival order by a single stream owner.
            _processData = ResponseReaderTask(sessionId, responseProcessingToken);
        }
    }

    private async Task EnsureSessionEstablishedAsync(CancellationToken token)
    {
        try
        {
            if (!ShouldEstablishSession(token))
            {
                return;
            }

            await SetupConnectionSessionAsync(token);
        }
        catch (AuthenticationException exc)
        {
            HandleConnectionFault(new ConnectionKafkaException("Authentication failed during connector setup.", exc));
            Debug.WriteLine(exc.Message);

            throw;
        }
        catch (SocketException exc)
        {
            HandleConnectionFault(new ConnectionKafkaException("Socket failure during connector setup.", exc));
            Debug.WriteLine(exc.Message);

            throw;
        }
        catch (Exception exc)
        {
            HandleConnectionFault(new ConnectionKafkaException("Unexpected connector setup failure.", exc));
            Debug.WriteLine(exc.Message);

            throw;
        }
    }

    private bool ShouldEstablishSession(CancellationToken token)
    {
        return !_socketProxy.Connected && !token.IsCancellationRequested && ConnectorState == State.Closed;
    }

    private async Task SetupConnectionSessionAsync(CancellationToken token)
    {
        await EstablishTransportAsync(token);
        _globalTimeWaiting.Start();

        await NegotiateApiVersionsAsync(token);
        await AuthenticateSessionAsync(token);
        PublishOpenState();
    }

    private async Task EstablishTransportAsync(CancellationToken token)
    {
        SetState(State.Connecting);
        await _socketProxy.ConnectAsync(Endpoint, token);

        // The connector must always switch to the final transport stream here so that
        // every later setup step and every steady-state request uses the same session.
        var networkStream = _socketFactory.CreateNetworkStream(_socketProxy.Socket, true);

        if (_securityProtocol is SecurityProtocols.Ssl or SecurityProtocols.SaslSsl)
        {
            _stream = _socketFactory.CreateSslStream(networkStream);

            // SSL handshake completes the transport contract before any Kafka-level
            // negotiation starts because ApiVersions and SASL must run on the final wire format.
            await ((SslStream)_stream).AuthenticateAsClientAsync("", null, _sslSettings.Protocols, _sslSettings.CheckCertificateRevocation);
        }
        else
        {
            _stream = networkStream;
        }
    }

    private async Task NegotiateApiVersionsAsync(CancellationToken token)
    {
        SetState(State.Negotiating);

        if (SupportVersions.Count != 0 || !_apiVersionRequest)
        {
            return;
        }

        // The negotiated API map belongs to the current physical session because
        // every reconnect may land on a different capability set or require a new fallback range.
        var request = ApiVersionsRequestMessage.Build();
        var response = await ((IKafkaConnector)this).SendAsync<ApiVersionsRequestMessage, ApiVersionsResponseMessage>(
            request,
            true,
            token);

        ((IResponseMessage)response).ThrowIfError();
        PublishSupportVersions(response);
    }

    private void PublishSupportVersions(ApiVersionsResponseMessage response)
    {
        if (response is { Code: ErrorCodes.None, ApiKeys.Count: 0 })
        {
            SupportVersions = SupportVersionsExtensions.Default;

            return;
        }

        var supportVersions = new Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)>(response.ApiKeys.Count);

        foreach (var apiKey in response.ApiKeys)
        {
            supportVersions.Add((ApiKeys)apiKey.ApiKey, ((ApiVersion)apiKey.MinVersion, (ApiVersion)apiKey.MaxVersion));
        }

        SupportVersions = supportVersions;
    }

    private async Task AuthenticateSessionAsync(CancellationToken token)
    {
        if (_securityProtocol is not (SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl))
        {
            return;
        }

        SetState(State.Authenticating);

        // Authentication runs as a dedicated setup step because it depends on the
        // negotiated session context but must still complete before the connector becomes Open.
        await AuthenticateSaslSessionAsync(token);
    }

    private void PublishOpenState()
    {
        // Open is published only after the full setup pipeline succeeds so that callers
        // never observe a write-ready connector with incomplete protocol or auth state.
        SetState(State.Open);
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

    private void Dispose(bool disposing)

    {
        CloseConnectionCore(CreateClosedException("Connector is being disposed."), disposing);

        if (disposing)
        {
            _responseProcessingTokenSource.Dispose();
        }
    }

    private void SetState(State state)
    {
        if (ConnectorState != state)
        {
            _logger.StateTransitionDebug(NodeId, Endpoint, ConnectorState, state);
        }

        ConnectorState = state;
    }

    private void InvalidateSupportVersions()
    {
        if (SupportVersions.Count != 0)
        {
            _logger.SupportVersionsInvalidatedTrace(NodeId);
        }

        SupportVersions = [];
    }

    private void FailAllInflightRequests(Exception exception)
    {
        while (!_inFlightRequests.IsEmpty)
        {
            var pendingRequests = _inFlightRequests.Keys.ToArray();

            foreach (var requestId in pendingRequests)
            {
                if (_inFlightRequests.TryRemove(requestId, out var responseCompletionSource))
                {
                    responseCompletionSource.TrySetException(exception);
                }
            }
        }
    }

    private bool TryTakeInflightRequest(int requestId, [NotNullWhen(true)] out ResponseTaskCompletionSource? responseCompletionSource)
    {
        return _inFlightRequests.TryRemove(requestId, out responseCompletionSource);
    }

    private void CompleteRequestFromLifetimeCancellation(
        int requestId,
        ResponseTaskCompletionSource responseCompletionSource,
        CancellationToken callerToken)
    {
        if (!TryTakeInflightRequest(requestId, out _))
        {
            return;
        }

        if (callerToken.IsCancellationRequested)
        {
            CompleteRequestAsCanceled(responseCompletionSource, callerToken);

            return;
        }

        CompleteRequestAsTimedOut(responseCompletionSource);
    }

    private void CompleteRequestAsCanceled(
        ResponseTaskCompletionSource responseCompletionSource,
        CancellationToken cancellationToken)
    {
        responseCompletionSource.TrySetCanceled(cancellationToken);
    }

    private void CompleteRequestAsTimedOut(ResponseTaskCompletionSource responseCompletionSource)
    {
        responseCompletionSource.TrySetException(
            new ProtocolKafkaException(
                ErrorCodes.RequestTimedOut,
                $"Request to NodeId={NodeId} timed out after {_requestTimeoutMs} ms."));
    }

    private void CompleteRequestAsWriteFailed(
        ResponseTaskCompletionSource responseCompletionSource,
        Exception exception)
    {
        // A failed write means the broker cannot produce a correlated response for this request,
        // so the pending completion must leave the inflight registry immediately.
        responseCompletionSource.TrySetException(
            new ProtocolKafkaException(
                ErrorCodes.NetworkException,
                $"Failed to send request to NodeId={NodeId}.",
                exception));
    }

    private void CompleteRequestFromResponse(
        ResponseTaskCompletionSource responseCompletionSource,
        IResponseMessage responseMessage)
    {
        responseCompletionSource.TrySetResult(responseMessage);
    }

    private void CompleteRequestAsResponseFailure(
        ResponseTaskCompletionSource responseCompletionSource,
        Exception exception)
    {
        responseCompletionSource.TrySetException(exception);
    }

    private void CloseConnectionCore(Exception exception, bool disposeSocket = false)
    {
        if (ConnectorState == State.Closed)
        {
            return;
        }

        SetState(State.Closing);
        InvalidateSupportVersions();
        StopResponseProcessing();
        FailAllInflightRequests(exception);

        _stream.Dispose();
        _stream = Stream.Null;

        if (disposeSocket)
        {
            _socketProxy.Dispose();
        }
        else
        {
            _socketProxy.Close(_closeConnectionTimeoutMs);
        }

        ResetResponseProcessing();
        SetState(State.Closed);
    }

    private async ValueTask CloseConnectionCoreAsync(Exception exception)
    {
        if (ConnectorState == State.Closed)
        {
            return;
        }

        SetState(State.Closing);
        InvalidateSupportVersions();
        StopResponseProcessing();
        FailAllInflightRequests(exception);

        await _stream.DisposeAsync();
        _stream = Stream.Null;
        _socketProxy.Dispose();

        ResetResponseProcessing();
        SetState(State.Closed);
    }

    private ConnectionKafkaException CreateClosedException(string reason)
    {
        return new ConnectionKafkaException($"{reason} Endpoint={Endpoint}, NodeId={NodeId}");
    }

    private void HandleConnectionFault(ConnectionKafkaException exception)
    {
        if (ConnectorState is State.Closing or State.Closed)
        {
            return;
        }

        _logger.ConnectionFaultWarning(NodeId, Endpoint, exception.Message);
        SetState(State.Faulted);
        CloseConnectionCore(exception);
    }

    private void StopResponseProcessing()
    {
        if (!_responseProcessingTokenSource.IsCancellationRequested)
        {
            _responseProcessingTokenSource.Cancel();
        }
    }

    private void ResetResponseProcessing()
    {
        _responseProcessingTokenSource.Dispose();
        _responseProcessingTokenSource = new CancellationTokenSource();
        _processData = Task.CompletedTask;
        Interlocked.Increment(ref _responseProcessingSessionId);
    }

    /// <summary>
    /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~KafkaConnector()
    {
        Dispose(false);
    }

    internal enum State
    {
        Closed,
        Connecting,
        Negotiating,
        Authenticating,
        Open,
        Faulted,
        Closing,
    }

    private sealed class ResponseTaskCompletionSource(ApiKeys apiKey, ApiVersion version)
        : TaskCompletionSource<IResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously)
    {
        public ApiKeys ApiKey { get; } = apiKey;

        internal IResponseMessage BuildResponseMessage(byte[] span)
        {
            return ResponseBuilder.Build(ApiKey, version, span);
        }
    }

    private sealed record RequestLifetimeContext(
        KafkaConnector Connector,
        int RequestId,
        ResponseTaskCompletionSource ResponseCompletionSource,
        CancellationToken CallerToken);
}
