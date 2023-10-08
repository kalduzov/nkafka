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

using NKafka.Exceptions;
using NKafka.Protocol;

namespace NKafka;

/// <summary>
/// Полная текущая информация о кластере
/// </summary>
internal sealed record ClusterMetadata
{
    /// <summary>
    /// Аггрегированный набор минимальных и максимальных версий API в кластере
    /// </summary>
    internal Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)> AggregationApiByVersion { get; } = new(17);

    /// <summary>
    /// Возвращает текущую версию Api ключа, которая поддерживается указанной нодой
    /// </summary>
    /// <param name="apiKey">Ключ API, для которого нужно получить информацию</param>
    /// <returns>Поддерживаемая версия API</returns>
    internal ApiVersion GetCurrentApiVersion(ApiKeys apiKey)
    {
        if (AggregationApiByVersion.TryGetValue(apiKey, out var version))
        {
            return version.MaxVersion;
        }

        throw new UnsupportedVersionException($"В данной конфигурации кластера, переданный {apiKey} не поддерживается");
    }
}