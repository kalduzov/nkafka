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

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Provides a manager interface for sending messages to a kafka cluster
/// </summary>
internal interface IMessagesSender
{
    /// <summary>
    /// The process of sending batches with messages starts.
    /// </summary>
    Task StartAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Stops the process of sending messages until the Wakeup method is called 
    /// </summary>
    void Sleep();

    /// <summary>
    /// Completely stops all sending batches with messages
    /// </summary>
    /// <param name="timeout"></param>
    void Stop(TimeSpan timeout);

    /// <summary>
    /// Wakes up the manager for further sending batches with messages
    /// </summary>
    void Wakeup();
}