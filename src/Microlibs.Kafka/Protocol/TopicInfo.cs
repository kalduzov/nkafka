using System;
using System.Collections.Generic;

namespace Microlibs.Kafka.Protocol;

public record TopicInfo
{
    public TopicInfo(
        StatusCodes code,
        string? name,
        bool isInternal,
        int topicAuthorizedOperations,
        IReadOnlyCollection<PartitionInfo> partitions,
        Guid? topicId)
    {
        Code = code;
        Name = name;
        TopicId = topicId ?? Guid.Empty;
        IsInternal = isInternal;
        TopicAuthorizedOperations = topicAuthorizedOperations;
        Partitions = partitions;
    }

    public StatusCodes Code { get; init; }

    public string Name { get; init; }

    public Guid TopicId { get; init; }

    public bool IsInternal { get; init; }

    public int TopicAuthorizedOperations { get; init; }

    public IReadOnlyCollection<PartitionInfo> Partitions { get; init; }
}