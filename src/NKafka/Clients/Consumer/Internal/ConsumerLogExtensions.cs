//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using Microsoft.Extensions.Logging;

namespace NKafka.Clients.Consumer.Internal;

internal static partial class ConsumerLogExtensions
{
    [LoggerMessage(EventId = LogExtensions.CONSUMER_EVENT_BASE_ID,
        Level = LogLevel.Trace,
        Message = LogExtensions.LOGGER_PREFIX + "Starting coordinator for group {GroupId}")]
    public static partial void StartingCoordinatorTrace(this ILogger logger, string groupId);

    [LoggerMessage(EventId = LogExtensions.CONSUMER_EVENT_BASE_ID + 1,
        Level = LogLevel.Trace,
        Message = LogExtensions.LOGGER_PREFIX + "The state of the coordinator has changed from {OldState} to {NewState}")]
    public static partial void CoordinatorChangeStateTrace(this ILogger logger, Coordinator.CoordinatorState oldState, Coordinator.CoordinatorState newState);

    [LoggerMessage(EventId = LogExtensions.CONSUMER_EVENT_BASE_ID + 2,
        Level = LogLevel.Trace,
        Message = LogExtensions.LOGGER_PREFIX + "Consumer '{ConsumerInstanceId}' init new subscription {Subscription}")]
    public static partial void ConsumerNewSubscriptionTrace(this ILogger logger, ulong consumerInstanceId, Subscription subscription);
}