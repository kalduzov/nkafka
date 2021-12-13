using System;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka.Clients.Producer.Internals;

public sealed class RecordAccumulator
{
    public RecordAccumulator(
        ProducerConfig config,
        ILoggerFactory loggerFactory,
        int deliveryTimeoutMs,
        TimeSpan time,
        ApiVersions apiVersions,
        TransactionManager transactionManager,
        BufferPool bufferPool)
    {
    }

    public async Task<TaskCompletionSource<int>> AppendAsync(
        TopicPartition topicPartition,
        Timestamp messageTimestamp,
        byte[] keyByte,
        byte[] valueByte,
        Headers headers,
        CancellationToken token)
    {
        return new TaskCompletionSource<int>(TaskCreationOptions.None);
    }
}