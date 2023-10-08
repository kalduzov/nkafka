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

using System.Threading.Channels;

namespace NKafka.Clients.Consumer.Internal;

/// <summary>
/// Messages fetcher interface
/// </summary>
internal interface IFetcher<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    /// <summary>
    /// Наполняет данными канал по параметрам указанной подписки  
    /// </summary>
    /// <param name="subscription"></param>
    /// <param name="channelWriter"></param>
    /// <param name="token"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    Task StartAsync(Subscription subscription, ChannelWriter<ConsumerRecord<TKey, TValue>> channelWriter, CancellationToken token);

    Task StopAsync(CancellationToken cancellationToken);
}