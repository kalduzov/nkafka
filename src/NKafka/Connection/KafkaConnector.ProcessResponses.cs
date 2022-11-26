using System.Diagnostics;

using NKafka.Exceptions;
using NKafka.Protocol;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Connection;

internal sealed partial class KafkaConnector
{
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
                if (_inFlightRequests.IsEmpty)
                {
                    return;
                }

                if (_stream != Stream.Null && !_stream.CanRead)
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

                Debug.WriteLine($"Get new message CorrelationId={requestId}, ResponseLength={requestLength}");

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
            if (_inFlightRequests.TryRemove(requestId, out var responseInfo))
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