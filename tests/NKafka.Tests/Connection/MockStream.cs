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

using System.Collections.Concurrent;
using System.Diagnostics;

using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;

namespace NKafka.Tests.Connection;

internal class MockStream: Stream
{
    private readonly ConcurrentQueue<IRequestMessage> _requestMessages = new();
    private readonly ConcurrentQueue<byte[]> _sendQueue = new();
    private readonly ConcurrentDictionary<IRequestMessage, (int CorrelactionId, ApiVersion ApiVersion)> _correlationIds = new();
    private readonly Task _processTask;

#if NET9_0_OR_GREATER
    private readonly Lock _lockObject = new();
#else
    private readonly object _lockObject = new();
#endif

    private readonly CancellationTokenSource _tokenSource = new();

    public MockStream()
    {
        _processTask = ProcessRequests(_tokenSource.Token);
    }

    private async Task ProcessRequests(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!_requestMessages.TryDequeue(out var requestMessage))
            {
                await Task.Delay(25, token);

                continue;
            }

            GenerateMockResponse(requestMessage);
        }
    }

    private void GenerateMockResponse(IRequestMessage requestMessage)
    {
        try
        {
            var arrayBuffer = new ArrayBuffer(true, false, 10000);
            var writer = new BufferWriter(ref arrayBuffer);

            _correlationIds.TryRemove(requestMessage, out var requestData);

            var responseHeader = new ResponseHeader
            {
                CorrelationId = requestData.CorrelactionId,
            };

            responseHeader.Write(ref writer, requestMessage.ApiKey.GetResponseHeaderVersion(requestData.ApiVersion));

            switch (requestMessage.ApiKey)
            {
                case ApiKeys.ApiVersions:
                    {
                        var response = new ApiVersionsResponseMessage();
                        response.Write(ref writer, requestData.ApiVersion);

                        break;
                    }
            }

            lock (_lockObject)
            {
                //Т.к. из потока читается в 3 этапа, то для эмуляции нужно отправить 3 пакета байт
                var array = arrayBuffer.DangerousGetFirstBuffer();
                _sendQueue.Enqueue([]); //Тут нужно корректно написать длину данных
                _sendQueue.Enqueue(array[..4]);
                _sendQueue.Enqueue(array[4..]);
            }
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);
        }
    }

    public override void Flush()
    {
    }

    /// <summary>Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.</summary>
    /// <param name="buffer">The region of memory to write the data into.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
    /// <returns>A task that represents the asynchronous read operation. The value of its <see cref="P:System.Threading.Tasks.ValueTask`1.Result" /> property contains the total number of bytes read into the buffer. The result value can be less than the number of bytes allocated in the buffer if that many bytes are not currently available, or it can be 0 (zero) if the end of the stream has been reached.</returns>
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new())
    {
        byte[]? buf;

        // ReSharper disable once InconsistentlySynchronizedField
        while (!_sendQueue.TryDequeue(out buf) && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(25, cancellationToken);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return 0;
        }

        if (buf is null)
        {
            return 0;
        }

        buf.CopyTo(buffer);

        return buf.Length;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return 0;
    }

    public override void SetLength(long value)
    {
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        using var reader = new BufferReader(buffer);
        _ = reader.ReadInt();
        var apiKey = (ApiKeys)reader.ReadShort();
        var requestApiVersion = (ApiVersion)reader.ReadShort();
        var correlationId = reader.ReadInt();
        var request = RequestBuilder.Build(apiKey, requestApiVersion, buffer[4..count]);

        _requestMessages.Enqueue(request);
        _correlationIds.TryAdd(request, (correlationId, requestApiVersion));
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Stream" /> and optionally releases the managed resources.</summary>
    /// <param name="disposing">
    /// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _tokenSource.Cancel();
        _tokenSource.Dispose();

        if (_processTask.Status is TaskStatus.Canceled or TaskStatus.Faulted or TaskStatus.RanToCompletion)
        {
            _processTask.Dispose();
        }
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length { get; }

    public override long Position { get; set; }
}