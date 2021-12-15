using System;

namespace Microlibs.Kafka;

public struct Timestamp : IEquatable<Timestamp>
{
    private const long _RD_KAFKA_NO_TIMESTAMP = 0;

    /// <summary>
    ///     A read-only field representing an unspecified timestamp.
    /// </summary>
    public static Timestamp Default => new(_RD_KAFKA_NO_TIMESTAMP, TimestampType.NotAvailable);

    /// <summary>
    ///     Unix epoch as a UTC DateTime. Unix time is defined as
    ///     the number of seconds past this UTC time, excluding
    ///     leap seconds.
    /// </summary>
    public static readonly DateTime UnixTimeEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private const long _UNIX_TIME_EPOCH_MILLISECONDS = 62135596800000; // = UnixTimeEpoch.TotalMiliseconds

    /// <summary>
    ///     Initializes a new instance of the Timestamp structure.
    /// </summary>
    /// <param name="unixTimestampMs">
    ///     The unix millisecond timestamp.
    /// </param>
    /// <param name="type">
    ///     The type of the timestamp.
    /// </param>
    public Timestamp(long unixTimestampMs, TimestampType type)
    {
        Type = type;
        UnixTimestampMs = unixTimestampMs;
    }

    /// <summary>
    ///     Initializes a new instance of the Timestamp structure.
    ///     Note: <paramref name="dateTime" /> is first converted to UTC
    ///     if it is not already.
    /// </summary>
    /// <param name="dateTime">
    ///     The DateTime value corresponding to the timestamp.
    /// </param>
    /// <param name="type">
    ///     The type of the timestamp.
    /// </param>
    public Timestamp(DateTime dateTime, TimestampType type)
    {
        Type = type;
        UnixTimestampMs = DateTimeToUnixTimestampMs(dateTime);
    }

    /// <summary>
    ///     Initializes a new instance of the Timestamp structure.
    ///     Note: <paramref name="dateTime" /> is first converted
    ///     to UTC if it is not already and TimestampType is set
    ///     to CreateTime.
    /// </summary>
    /// <param name="dateTime">
    ///     The DateTime value corresponding to the timestamp.
    /// </param>
    public Timestamp(DateTime dateTime)
        : this(dateTime, TimestampType.CreateTime)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Timestamp structure.
    ///     Note: TimestampType is set to CreateTime.
    /// </summary>
    /// <param name="dateTimeOffset">
    ///     The DateTimeOffset value corresponding to the timestamp.
    /// </param>
    public Timestamp(DateTimeOffset dateTimeOffset)
        : this(dateTimeOffset.UtcDateTime, TimestampType.CreateTime)
    {
    }

    /// <summary>
    ///     Gets the timestamp type.
    /// </summary>
    public TimestampType Type { get; }

    /// <summary>
    ///     Get the Unix millisecond timestamp.
    /// </summary>
    public long UnixTimestampMs { get; }

    /// <summary>
    ///     Gets the UTC DateTime corresponding to the <see cref="UnixTimestampMs" />.
    /// </summary>
    public DateTime UtcDateTime => UnixTimestampMsToDateTime(UnixTimestampMs);

    /// <summary>
    ///     Determines whether two Timestamps have the same value.
    /// </summary>
    /// <param name="obj">
    ///     Determines whether this instance and a specified object,
    ///     which must also be a Timestamp object, have the same value.
    /// </param>
    /// <returns>
    ///     true if obj is a Timestamp and its value is the same as
    ///     this instance; otherwise, false. If obj is null, the method
    ///     returns false.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is Timestamp ts)
        {
            return Equals(ts);
        }

        return false;
    }

    /// <summary>
    ///     Determines whether two Timestamps have the same value.
    /// </summary>
    /// <param name="other">
    ///     The timestamp to test.
    /// </param>
    /// <returns>
    ///     true if other has the same value. false otherwise.
    /// </returns>
    public bool Equals(Timestamp other)
    {
        return other.Type == Type && other.UnixTimestampMs == UnixTimestampMs;
    }

    /// <summary>
    ///     Returns the hashcode for this Timestamp.
    /// </summary>
    /// <returns>
    ///     A 32-bit signed integer hash code.
    /// </returns>
    public override int GetHashCode()
    {
        return Type.GetHashCode() * 251 + UnixTimestampMs.GetHashCode();

        // x by prime number is quick and gives decent distribution.
    }

    /// <summary>
    ///     Determines whether two specified Timestamps have the same value.
    /// </summary>
    /// <param name="a">
    ///     The first Timestamp to compare.
    /// </param>
    /// <param name="b">
    ///     The second Timestamp to compare
    /// </param>
    /// <returns>
    ///     true if the value of a is the same as the value of b; otherwise, false.
    /// </returns>
    public static bool operator ==(Timestamp a, Timestamp b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Determines whether two specified Timestamps have different values.
    /// </summary>
    /// <param name="a">
    ///     The first Timestamp to compare.
    /// </param>
    /// <param name="b">
    ///     The second Timestamp to compare
    /// </param>
    /// <returns>
    ///     true if the value of a is different from the value of b; otherwise, false.
    /// </returns>
    public static bool operator !=(Timestamp a, Timestamp b)
    {
        return !(a == b);
    }

    /// <summary>
    ///     Convert a DateTime instance to a milliseconds unix timestamp.
    ///     Note: <paramref name="dateTime" /> is first converted to UTC
    ///     if it is not already.
    /// </summary>
    /// <param name="dateTime">
    ///     The DateTime value to convert.
    /// </param>
    /// <returns>
    ///     The milliseconds unix timestamp corresponding to <paramref name="dateTime" />
    ///     rounded down to the previous millisecond.
    /// </returns>
    public static long DateTimeToUnixTimestampMs(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond - _UNIX_TIME_EPOCH_MILLISECONDS;
    }

    /// <summary>
    ///     Convert a milliseconds unix timestamp to a DateTime value.
    /// </summary>
    /// <param name="unixMillisecondsTimestamp">
    ///     The milliseconds unix timestamp to convert.
    /// </param>
    /// <returns>
    ///     The DateTime value associated with <paramref name="unixMillisecondsTimestamp" /> with Utc Kind.
    /// </returns>
    public static DateTime UnixTimestampMsToDateTime(long unixMillisecondsTimestamp)
    {
        return UnixTimeEpoch + TimeSpan.FromMilliseconds(unixMillisecondsTimestamp);
    }
}