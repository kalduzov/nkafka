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

namespace NKafka.Clients.Consumer.Internal;

/// <summary>
///                                                                                         
/// </summary>
internal interface ICoordinator: IAsyncDisposable
{
    /// <summary>
    /// 
    /// </summary>
    int GenerationId { get; }

    /// <summary>
    /// 
    /// </summary>
    string MemberId { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsLeader { get; }

    /// <summary>
    /// Создает новую сессию с координатором с указанной подпиской
    /// </summary>
    /// <param name="subscription">Информация о подписке для которой нужно начать новую сессию</param>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<bool> NewSessionAsync(Subscription subscription, CancellationToken token);

    /// <summary>
    /// Загружает закоммиченные офсеты для всех топиков и разделов данной группы. 
    /// </summary>
    /// <param name="subscription">Параметры текущей подписки</param>
    /// <param name="token">Токен отмены асинхронной операции</param>
    ValueTask FetchOffsetsAsync(Subscription subscription, CancellationToken token);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask StopSessionAsync(CancellationToken token);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offsetManager"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask CommitAsync(IOffsetManager offsetManager, CancellationToken token);
}