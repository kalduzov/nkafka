//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

namespace NKafka;

/// <summary>
/// Contains all auto-generated extension methods for logging messages
/// </summary>
internal static partial class LogExtensions
{
    internal const int KAFKA_CONNECTOR_EVENT_BASE_ID = 1000;
    internal const int KAFKA_CONNECTOR_POOL_EVENT_BASE_ID = 2000;
    internal const int SASL_EVENT_BASE_ID = 3000;
    internal const int PRODUCER_EVENT_BASE_ID = 5000;
    internal const int CONSUMER_EVENT_BASE_ID = 6000;

    internal const string LOGGER_PREFIX = "[Kafka] ";

    [LoggerMessage(EventId = 1, Level = LogLevel.Trace, Message = LOGGER_PREFIX + "Creating new cluster")]
    public static partial void CreateClusterTrace(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = LOGGER_PREFIX + "OverrideDefaultAcks {Acks}")]
    public static partial void OverrideDefaultAcks(this ILogger logger, short acks);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Warning,
        Message = LOGGER_PREFIX
                  + "The metadata update interval is too short. Try to choose a larger value. Current value of MetadataMaxAge {MetadataMaxAge}ms")]
    public static partial void WarningMetadataMaxAge(this ILogger logger, int metadataMaxAge);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Trace,
        Message = LOGGER_PREFIX + "UpdateMetadata start. Count {Count}")]
    public static partial void UpdateMetadataStart(this ILogger logger, int count);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Trace,
        Message = LOGGER_PREFIX + "UpdateLastReadingOffset metadata end. Count {Count}, Time {Time}")]
    public static partial void UpdateMetadataEnd(this ILogger logger, int count, TimeSpan time);

    [LoggerMessage(
        EventId = 6,
        Level = LogLevel.Error,
        Message = LOGGER_PREFIX + "UpdateLastReadingOffset metadata error. Count {Count}")]
    public static partial void UpdateMetadataError(this ILogger logger, Exception exc, int count);

    [LoggerMessage(
        EventId = 8,
        Level = LogLevel.Trace,
        Message = LOGGER_PREFIX + "'{MemberName}' has been called")]
    public static partial void CallMethodTrace(this ILogger logger, [CallerMemberName] string memberName = "");
}