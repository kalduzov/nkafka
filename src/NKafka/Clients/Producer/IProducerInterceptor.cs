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
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace NKafka.Clients.Producer;

/// <summary>
///     A plugin interface that allows you to intercept (and possibly mutate) the records received by the producer before
///     they are published to the Kafka cluster.
/// </summary>
public interface IProducerInterceptor<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    Message<TKey, TValue> OnProduce(Message<TKey, TValue> record);

    void OnAcknowledgement(RecordMetadata metadata, Exception exception);
}