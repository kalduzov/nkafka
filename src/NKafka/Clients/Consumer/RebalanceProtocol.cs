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

namespace NKafka.Clients.Consumer;

/// <summary>
/// The rebalance protocol defines partition assignment and revocation semantics.
/// </summary>
public enum RebalanceProtocol
{
    /// <summary>
    /// The <see cref="Eager"/> rebalance protocol requires a consumer to always revoke all its owned
    /// partitions before participating in a rebalance event. It therefore allows a complete reshuffling of the assignment.
    /// </summary>
    Eager = 0,

    /// <summary>
    /// <see cref="Cooperative"/> rebalance protocol allows a consumer to retain its currently owned
    /// partitions before participating in a rebalance event.
    /// </summary>
    Cooperative = 1,
}