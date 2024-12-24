namespace NKafka.Diagnostics;

internal sealed class LoggingScope(IDisposable?[] disposables): IDisposable
{
    public void Dispose()
    {
        for (var i = disposables.Length; i > 0; i--)
        {
            disposables[i]?.Dispose();
        }
    }
}