using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

namespace NKafka.Tests;

internal sealed class InMemoryLoggerFactory: ILoggerFactory
{
    private readonly ConcurrentQueue<LogEntry> _entries = new();

    public IReadOnlyCollection<LogEntry> Entries => _entries.ToArray();

    public void AddProvider(ILoggerProvider provider)
    {
    }

    public ILogger CreateLogger(string categoryName)
        => new InMemoryLogger(categoryName, _entries);

    public void Dispose()
    {
    }

    internal sealed record LogEntry(
        string CategoryName,
        LogLevel LogLevel,
        EventId EventId,
        string Message,
        Exception? Exception);

    private sealed class InMemoryLogger(string categoryName, ConcurrentQueue<LogEntry> entries): ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            entries.Enqueue(new LogEntry(categoryName, logLevel, eventId, formatter(state, exception), exception));
        }

        private sealed class NullScope: IDisposable
        {
            public static NullScope Instance { get; } = new();

            public void Dispose()
            {
            }
        }
    }
}
