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

namespace NKafka.Clients.Consumer;

/// <summary>
/// 
/// </summary>
public interface IConsumerEventListener
{
    /// <summary>
    /// Данные метод будет вызван в консьюмере, сразу перед тем, как консьюмер прекратит обрабатывать указанные разделы
    /// </summary>
    Task PartitionsRevokedEventHandlerAsync();

    /// <summary>
    /// Данные метод будет вызван в консьюмере, сразу после того, как консьюмеру будет связан с определенным разделом
    /// </summary>
    Task PartitionsAssignedEventHandlerAsync();

    /// <summary>
    /// Данные метод будет вызван в консьюмере, сразу после того, как произойдет автоматический коммит  
    /// </summary>
    Task OffsetsCommittedEventHandlerAsync();
}