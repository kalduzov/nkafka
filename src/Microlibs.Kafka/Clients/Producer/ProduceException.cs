using System;

namespace Microlibs.Kafka.Clients.Producer;

public class ProduceException : Exception
{
    public ProduceException(Exception exc)
        : base("", exc)
    {
    }
}