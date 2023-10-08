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
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Net;

using Microsoft.Extensions.Logging;

namespace NKafka.Connection;

internal partial class KafkaConnectorPool
{
}

internal static partial class KafkaConnectorPoolLoggerExtensions
{
    [LoggerMessage(
        EventId = LogExtensions.KAFKA_CONNECTOR_POOL_EVENT_BASE_ID,
        Level = LogLevel.Trace,
        Message = LogExtensions.LOGGER_PREFIX + "New kafka connector on {EndPoint} created")]
    public static partial void CreateConnectorTrace(this ILogger<KafkaConnectorPool> logger, EndPoint endPoint);

    [LoggerMessage(EventId = LogExtensions.KAFKA_CONNECTOR_POOL_EVENT_BASE_ID + 1,
        Level = LogLevel.Warning,
        Message = LogExtensions.LOGGER_PREFIX + "Список bootstrap брокеров содержит повторы. Адрес {EndPoint} уже присутствует в списке. Текущий адрес будет проигнорирован.")]
    public static partial void IgnoreBootstrapEndpointWarning(this ILogger<KafkaConnectorPool> logger, EndPoint endPoint);
}