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

namespace NKafka.Clients.Producer.Internals;

/// <summary>
/// Metadata about a record just appended to the record accumulator
/// </summary>
/// <param name="BatchIsFull">Указывает, что результат добавления записи аккумулятор привел к полной загрузке пачки записей</param>
/// <param name="NewBatchCreated">Указывает, что в процессе добавления записи была создана новая пачка</param>
/// <param name="AppendedBytes">Размер добавленных записей в пачку</param>
/// <param name="SendResult"></param>
internal record RecordAppendResult(
    RecordMetadataTask? SendResult,
    bool BatchIsFull,
    bool NewBatchCreated,
    int AppendedBytes);