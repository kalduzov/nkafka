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

public readonly struct KafkaRequestHeader
{
    public KafkaRequestHeader(ApiKeys apiKey, ApiVersions apiVersion, int correlationId, string clientId)
    {
        ApiKey = apiKey;
        ApiVersion = apiVersion;
        CorrelationId = correlationId;
        ClientId = clientId;

        Length = 0x2 + 0x2 + 0x4 + 0x2 + ClientId.Length;
    }

    public readonly ApiKeys ApiKey;

    public readonly ApiVersions ApiVersion;

    public readonly int CorrelationId;

    public readonly string ClientId;

    public readonly int Length;
}