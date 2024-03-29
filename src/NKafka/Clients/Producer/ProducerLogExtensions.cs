﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.Extensions.Logging;

namespace NKafka.Clients.Producer;

/// <summary>
/// Contains all auto-generated extension methods for logging messages
/// </summary>
internal static partial class ProducerLogExtensions
{
    [LoggerMessage(EventId = LogExtensions.PRODUCER_EVENT_BASE_ID, Level = LogLevel.Trace, Message = LogExtensions.LOGGER_PREFIX + "Starting the Kafka producer {ProducerName}")]
    public static partial void StartProducerTrace(this ILogger logger, string producerName);

    [LoggerMessage(EventId = LogExtensions.PRODUCER_EVENT_BASE_ID + 1, Level = LogLevel.Debug, Message = LogExtensions.LOGGER_PREFIX + "Kafka producer {ProducerName} started")]
    public static partial void StartedProducer(this ILogger logger, string producerName);

    [LoggerMessage(EventId = LogExtensions.PRODUCER_EVENT_BASE_ID + 2, Level = LogLevel.Trace, Message = LogExtensions.LOGGER_PREFIX + "Produce new message to topic {TopicPartition}")]
    public static partial void ProduceMessageTrace(this ILogger logger, TopicPartition topicPartition);

    [LoggerMessage(EventId = LogExtensions.PRODUCER_EVENT_BASE_ID + 3, Level = LogLevel.Error, Message = LogExtensions.LOGGER_PREFIX + "Topic message producing error {TopicPartition}")]
    public static partial void ProduceMessageError(this ILogger logger, Exception exc, TopicPartition topicPartition);

    [LoggerMessage(EventId = LogExtensions.PRODUCER_EVENT_BASE_ID + 4, Level = LogLevel.Trace, Message = LogExtensions.LOGGER_PREFIX + "Add new batch for {TopicPartition}")]
    public static partial void AddNewBatchTrace(this ILogger logger, TopicPartition topicPartition);
}