using System;

namespace Microlibs.Kafka.Clients;

public abstract class CreateTopicOptions
{
    /// <summary>
    ///     If true, the request should be validated on the broker only
    ///     without creating the topic.
    /// 
    ///     Default: false
    /// </summary>
    public bool ValidateOnly { get; set; } = false;

    /// <summary>
    ///     The overall request timeout, including broker lookup, request 
    ///     transmission, operation time on broker, and response. If set
    ///     to null, the default request timeout for the AdminClient will
    ///     be used.
    /// 
    ///     Default: null
    /// </summary>
    public TimeSpan? RequestTimeout { get; set; }

    /// <summary>
    ///     The broker's operation timeout - the maximum time to wait for
    ///     CreateTopics before returning a result to the application.
    ///     If set to null, will return immediately upon triggering topic
    ///     creation.
    /// 
    ///     Default: null
    /// </summary>
    public TimeSpan? OperationTimeout { get; set; }
}