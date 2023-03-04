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

                // Каждое такое чтение - это заход в ядро.
                // Перевод на полное чтение например в pipe позволит вычитывать ответы с меньшим оверхедом.
                // Проблема в том, что нужно знать размер данных, которые нужно считать, а это можно узнать только чтением первых 4 байт из сети.
                // А потом еще надо прочитать это количество байт.
                // В идельном случае можно вообще не вычитывать весь буфер, а последовательным чтением сразу формировать нужный класс ответа 
                // Pipelines требуют свободного "потока", который будет сливать данные из сокета - его можно сделать один на весь пулл подключений aka NIO из java 
                var countReadBytes = await _stream.ReadAsync(intBuffer).ConfigureAwait(false);
                _totalBytesReceived = Interlocked.Add(ref _totalBytesReceived, countReadBytes);

                var responseLen = ReadInt32BigEndian(intBuffer.Span);

                if (responseLen == 0) //Данных нет идем дальше ждать
                {
                    if (countReadBytes == 4)
                    {
                        continue;
                    }

                    throw new ProtocolKafkaException(ErrorCodes.None, "Отправлен некорректный запрос к брокеру. Брокер вернул 0 байт.");
                }

                var responseIdLen = await _stream.ReadAsync(intBuffer).ConfigureAwait(false);
                _totalBytesReceived = Interlocked.Add(ref _totalBytesReceived, responseIdLen);

                var requestId = ReadInt32BigEndian(intBuffer.Span);

                Debug.WriteLine($"Get new message from NodeId = {NodeId} CorrelationId={requestId}, ResponseLength={responseLen}");

                var bodyLen = responseLen - responseIdLen;
                var buffer = _arrayPool.Rent(responseLen);

                // Возвращаем в буфер correlationId|requestId, он нужен для корректного считывания заголовка,
                // а без него версию заголовка не узнать
                buffer[0] = intBuffer.Span[0];
                buffer[1] = intBuffer.Span[1];
                buffer[2] = intBuffer.Span[2];
                buffer[3] = intBuffer.Span[3];

                var currentRead = 0;
                var leftRead = bodyLen;
                var startPosition = responseIdLen + currentRead;

                do
                {
                    currentRead = await _stream.ReadAsync(buffer.AsMemory(startPosition, leftRead))
                        .ConfigureAwait(false);
                    _totalBytesReceived = Interlocked.Add(ref _totalBytesReceived, currentRead);

                    leftRead -= currentRead;
                    startPosition += currentRead;
                } while (leftRead != 0);

                _responsesTasks.TryAdd(
                    requestId,
                    ParseResponseAsync(buffer, requestId, bodyLen, _responseProcessingTokenSource.Token));
            }
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);

            throw;
        }
    }

    private async Task ParseResponseAsync(byte[] buffer, int requestId, int bodyLen, CancellationToken token)
    {
        //сразу переключаемся на другой поток, что бы освободить работу для чтения ответов
        await Task.Yield();

        try
        {
            if (_inFlightRequests.TryRemove(requestId, out var responseInfo))
            {
                if (token.IsCancellationRequested)
                {
                    responseInfo.SetCanceled(token);

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

    private void UpdateResponseMetrics(int messageThrottleTimeMs, int bodyLen)
    {
    }
}