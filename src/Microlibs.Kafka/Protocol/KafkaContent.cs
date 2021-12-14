using System;
using System.IO;
using System.Threading.Tasks;
using Microlibs.Kafka.Protocol.RequestsMessages;

namespace Microlibs.Kafka.Protocol;

public abstract class KafkaContent : IDisposable
{
    internal static readonly KafkaContent Empty = new EmptyKafkaContent();
    
    private bool _canCalculateLength;

    private bool _disposed;

    protected KafkaContent()
    {
        _canCalculateLength = true;
    }

    public int Length { get; protected init; }

    public ApiKeys ApiKey { get; protected init; }
    
    public ApiVersions Version { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public abstract void SerializeToStream(Stream stream);
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;

            // if (_contentReadStream != null)
            // {
            //     Stream? s = _contentReadStream as Stream ??
            //                 (_contentReadStream is Task<Stream> t && t.Status == TaskStatus.RanToCompletion ? t.Result : null);
            //     s?.Dispose();
            //     _contentReadStream = null;
            // }
            //
            // if (IsBuffered)
            // {
            //     _bufferedContent!.Dispose();
            // }
        }
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().ToString());
        }
    }

    private static async Task<TResult> WaitAndReturnAsync<TState, TResult>(Task waitTask, TState state, Func<TState, TResult> returnFunc)
    {
        await waitTask.ConfigureAwait(false);

        return returnFunc(state);
    }
}