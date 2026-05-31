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

using System.Diagnostics;

using NKafka.Exceptions;
using NKafka.Protocol;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Connection;

internal sealed partial class KafkaConnector
{
    private long _totalBytesReceived;

    private async Task ResponseReaderTask()
    {
        /*
         * Задача на чтение запускается при постановке нового запроса в очередь ожидания
         * Задача не завершается, пока в очереди запросов есть хотя бы один не обработанный запрос
         *
         *
         * в случае если данные для запроса так и не придут, скорее всего было потеряно соединение
         * с брокером и тогда нужно будет удалить все запросы и сбросить соединение
         */
        await Task.Yield();

        try
        {
            var sw = new SpinWait();

            Memory<byte> intBuffer = new(new byte[sizeof(int)]);

            while (!_responseProcessingTokenSource.IsCancellationRequested)
            {
                if (_inFlightRequests.IsEmpty)
                {
                    return;
                }

                if (_stream != Stream.Null && !_stream.CanRead)
                {
                    sw.SpinOnce();

                    continue;
                }

                var countReadBytes = await _stream.ReadAsync(intBuffer);
                _totalBytesReceived = Interlocked.Add(ref _totalBytesReceived, countReadBytes);

                var responseLen = ReadInt32BigEndian(intBuffer.Span);

                if (responseLen == 0) //Данных нет, идем дальше ждать
                {
                    if (countReadBytes == 4)
                    {
                        continue;
                    }

                    throw new ProtocolKafkaException(ErrorCodes.None, "Отправлен некорректный запрос к брокеру. Брокер вернул 0 байт.");
                }

                var buffer = _arrayPool.Rent(responseLen);

                var leftRead = responseLen;
                var startPosition = 0;

                do
                {
                    var memorySlice = buffer.AsMemory(startPosition, leftRead);
                    var currentRead = await _stream.ReadAsync(memorySlice, _responseProcessingTokenSource.Token);
                    _totalBytesReceived = Interlocked.Add(ref _totalBytesReceived, currentRead);

                    leftRead -= currentRead;
                    startPosition += currentRead;
                } while (leftRead != 0);

                var requestId = ReadInt32BigEndian(buffer.AsSpan(0, 4));
                _responsesTasks.TryAdd(
                    requestId,
                    ParseResponseAsync(buffer, requestId, responseLen, _responseProcessingTokenSource.Token));
            }
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);
            if (exc is not OperationCanceledException || ConnectorState is not State.Closing and not State.Closed)
            {
                HandleConnectionFault(new ConnectionKafkaException("Response processing failed.", exc));
            }
        }
    }

    private async Task ParseResponseAsync(byte[] buffer, int requestId, int bodyLen, CancellationToken token)
    {
        //сразу переключаемся на другой поток, что бы освободить предыдущую таску чтения ответов
        await Task.Yield();

        try
        {
            if (_inFlightRequests.TryRemove(requestId, out var responseInfo))
            {
                Debug.WriteLine(
                    $"Get new response for {responseInfo.ApiKey} from NodeId = {NodeId} CorrelationId={requestId}, ResponseLength={bodyLen}");

                if (token.IsCancellationRequested)
                {
                    responseInfo.SetCanceled(token);

                    return;
                }

                try
                {
                    var message = responseInfo.BuildResponseMessage(buffer);
                    _logger.GotResponseTrace(message, NodeId);
                    UpdateResponseMetrics(message.ThrottleTimeMs, bodyLen + 4);
                    responseInfo.SetResult(message);
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
                    _inFlightRequests.TryRemove(requestId, out responseInfo);
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

    private void UpdateResponseMetrics(int messageThrottleTimeMs, int contentLen)
    {
    }
}
