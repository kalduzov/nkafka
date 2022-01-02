// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Microlibs.Kafka.Protocol;

internal class DefaultRequestBuilder : IRequestBuilder
{
    // private readonly string _clientName;
    //
    // public DefaultRequestBuilder(string clientName)
    // {
    //     _clientName = clientName;
    // }
    //
    // public KafkaRequestMessage Create<TMessage>(ApiKeys apiKey, int correlationId, string topicName, TMessage message)
    // {
    //     return new KafkaRequestMessage();
    // }
    //
    // public KafkaRequestMessage Create(ApiKeys apiKey, int correlationId)
    // {
    //     return Create<KafkaContent>(apiKey, correlationId, null!);
    // }
    //
    // public KafkaRequestMessage Create<T>(ApiKeys apiKey, int correlationId, T message)
    //     where T : class
    // {
    //     return apiKey switch
    //     {
    //         ApiKeys.ApiVersions => CreteApiVersionsRequest(correlationId),
    //     };
    // }
    //
    // private KafkaRequestMessage CreteApiVersionsRequest(int correlationId)
    // {
    //     var header = new KafkaRequestHeader(ApiKeys.ApiVersions, 0, correlationId, _clientName);
    //     var message = KafkaContent.Empty;
    //
    //     return new KafkaRequestMessage(header, message);
    // }
    public KafkaRequestMessage Create(ApiKeys apiKey, ApiVersions version)
    {
        return null;
    }
}