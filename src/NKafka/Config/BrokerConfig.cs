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

namespace NKafka.Config;

/// <summary>
/// 
/// </summary>
public class BrokerConfig
{
    /// <summary>
    /// Версия брокера кафки в кластере
    /// </summary>
    /// <remarks>Клиент поддерживает минимально версию 1.0. Максимальная версия 3.4.
    /// Если задано данное свойство, то клиент не обращается к серверу за списком версий,
    /// а использует предустановленный набор для конкретной версии. Это помогает уменьшить время инициализации соединения
    /// с брокером за счет отстуствия одного запроса за списком поддерживаемого брокером API</remarks>
    public Version BrokerVersion { get; set; } = Version.Parse("0.0");

    /// <summary>
    /// Ssl configuration
    /// </summary>
    public SslSettings Ssl { get; set; } = SslSettings.None;

    /// <summary>
    /// Sasl configuration
    /// </summary>
    public SaslSettings Sasl { get; set; } = SaslSettings.None;

    internal void Validate()
    {
    }
}