using System;
using System.Diagnostics;

namespace Microlibs.Kafka.Common;

internal static class TimeUtils
{
    internal static long NanoTime()
    {
        var nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;

        return nano;
    }
}