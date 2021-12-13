using System;

namespace Microlibs.Kafka;

public interface IConsumer<TKey, TValue> : IConsumer
{
}

public interface IConsumer : IDisposable, IAsyncDisposable
{
}