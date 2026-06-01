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

using System.Net;

using Microsoft.Extensions.Logging;

using NKafka.Protocol;

namespace NKafka.Connection;

internal sealed partial class KafkaConnector;

internal static partial class KafkaConnectorLoggerExtensions
{
    [LoggerMessage(
        EventId = LogExtensions.KAFKA_CONNECTOR_EVENT_BASE_ID,
        Level = LogLevel.Information,
        Message = LogExtensions.LOGGER_PREFIX
                  + "Connection at address {EndPoint} to broker {NodeId} was dropped due to inactivity for {ConnectionsMaxIdleMs} ms.")]
    public static partial void ConnectionResetInformation(this ILogger<KafkaConnector> logger,
        EndPoint endpoint,
        int nodeId,
        int connectionsMaxIdleMs);

    [LoggerMessage(
        EventId = LogExtensions.KAFKA_CONNECTOR_EVENT_BASE_ID + 1,
        Level = LogLevel.Trace,
        Message = LogExtensions.LOGGER_PREFIX + "Send {Message} to {NodeId}")]
    public static partial void SendRequestTrace(this ILogger<KafkaConnector> logger, IRequestMessage message, int nodeId);

    [LoggerMessage(EventId = LogExtensions.KAFKA_CONNECTOR_EVENT_BASE_ID + 2,
        Level = LogLevel.Trace,
        Message = LogExtensions.LOGGER_PREFIX + "Got response {Message} from {NodeId}")]
    public static partial void GotResponseTrace(this ILogger<KafkaConnector> logger, IResponseMessage message, int nodeId);

    [LoggerMessage(
        EventId = LogExtensions.KAFKA_CONNECTOR_EVENT_BASE_ID + 3,
        Level = LogLevel.Debug,
        Message = LogExtensions.LOGGER_PREFIX + "Connector {NodeId} at {EndPoint} transitions from {PreviousState} to {NextState}")]
    public static partial void StateTransitionDebug(
        this ILogger<KafkaConnector> logger,
        int nodeId,
        EndPoint endPoint,
        KafkaConnector.State previousState,
        KafkaConnector.State nextState);

    [LoggerMessage(
        EventId = LogExtensions.KAFKA_CONNECTOR_EVENT_BASE_ID + 4,
        Level = LogLevel.Warning,
        Message = LogExtensions.LOGGER_PREFIX + "Connector {NodeId} at {EndPoint} entered fault handling: {Reason}")]
    public static partial void ConnectionFaultWarning(
        this ILogger<KafkaConnector> logger,
        int nodeId,
        EndPoint endPoint,
        string reason);

    [LoggerMessage(
        EventId = LogExtensions.KAFKA_CONNECTOR_EVENT_BASE_ID + 5,
        Level = LogLevel.Trace,
        Message = LogExtensions.LOGGER_PREFIX + "Connector {NodeId} invalidated negotiated API versions for the current session")]
    public static partial void SupportVersionsInvalidatedTrace(this ILogger<KafkaConnector> logger, int nodeId);

    [LoggerMessage(
        EventId = LogExtensions.KAFKA_CONNECTOR_EVENT_BASE_ID + 6,
        Level = LogLevel.Debug,
        Message = LogExtensions.LOGGER_PREFIX + "Connector {NodeId} starts SASL authentication using {Mechanism}")]
    public static partial void StartSaslAuthenticationDebug(this ILogger<KafkaConnector> logger, int nodeId, string mechanism);
}
