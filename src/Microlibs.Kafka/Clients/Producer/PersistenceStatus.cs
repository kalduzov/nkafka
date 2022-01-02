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
namespace Microlibs.Kafka.Clients.Producer;

public enum PersistenceStatus
{
    /// <summary>
    ///     Message was never transmitted to the broker, or failed with
    ///     an error indicating it was not written to the log.
    ///     Application retry risks ordering, but not duplication.
    /// </summary>
    NotPersisted = 0,

    /// <summary>
    ///     Message was transmitted to broker, but no acknowledgement was
    ///     received. Application retry risks ordering and duplication.
    /// </summary>
    PossiblyPersisted = 1,

    /// <summary>
    ///     Message was written to the log and acknowledged by the broker.
    ///     Note: acks='all' should be used for this to be fully trusted
    ///     in case of a broker failover.
    /// </summary>
    Persisted = 2
}