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

using System.Net;

using NKafka.Protocol;

namespace NKafka.Connection;

/// <summary>
/// The interface for working with a physical connection to a broker
/// </summary>
internal interface IKafkaConnector: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Is it a dedicated connection or not
    /// </summary>
    bool IsDedicated { get; }

    /// <summary>
    /// Broker ID
    /// </summary>
    int NodeId { get; set; }

    /// <summary>
    /// Current connection state
    /// </summary>
    KafkaConnector.State ConnectorState { get; }

    /// <summary>
    /// The current number of running inflight requests
    /// </summary>
    int CurrentNumberInflightRequests { get; }

    /// <summary>
    /// Current connection address
    /// </summary>
    EndPoint Endpoint { get; }

    /// <summary>
    ///  
    /// </summary>
    Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)> SupportVersions { get; }

    /// <summary>
    /// Отправляет запрос в брокер
    /// </summary>
    internal Task<TResponseMessage> SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        bool isInternalRequest,
        CancellationToken token)
        where TResponseMessage : class, IResponseMessage
        where TRequestMessage : class, IRequestMessage;

    /// <summary>
    /// Открывает соединение с брокером по указанному адресу и порту
    /// </summary>
    ValueTask OpenAsync(CancellationToken token);
}