﻿using System;
using System.IO;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

internal class DescribeClusterContent : KafkaContent
{
    public DescribeClusterContent()
    {
        Length = 0x4;
        ApiKey = ApiKeys.DescribeCluster;
    }

    public bool IncludeClusterAuthorizedOperations { get; set; }

    public override void SerializeToStream(Stream stream)
    {
        stream.Write((IncludeClusterAuthorizedOperations ? 1 : 0).ToBigEndian());
    }
}