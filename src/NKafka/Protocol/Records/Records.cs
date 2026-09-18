// This is an independent project of an individual developer. Dear PVS-Studio, please check.

using NKafka.Protocol.Buffers;

namespace NKafka.Protocol.Records;

/// <summary>
/// A serialized sequence of Kafka record batches.
/// </summary>
internal sealed class Records
{
    public ArrayBuffer Buffer { get; }

    public IEnumerable<RecordBatch> Batches { get; private set; } = [];

    public int SizeInBytes { get; }

    public Records(ArrayBuffer buffer, int sizeInBytes)
    {
        Buffer = buffer;
        SizeInBytes = sizeInBytes;
    }

    internal Records(ArrayBuffer buffer, int sizeInBytes, bool parse)
        : this(buffer, sizeInBytes)
    {
        if (parse)
        {
            Read();
        }
    }

    private void Read()
    {
        var bytes = new byte[SizeInBytes];
        Buffer.CopyWrittenTo(bytes);
        var reader = new BufferReader(bytes);
        var batches = new List<RecordBatch>();

        while (reader.Remaining >= RecordBatch.RECORD_BATCH_OVERHEAD)
        {
            var offset = reader.CurrentOffset;
            var batch = new RecordBatch(ref reader);

            if (reader.CurrentOffset <= offset)
            {
                break;
            }

            batches.Add(batch);
        }

        Batches = batches;
    }
}
