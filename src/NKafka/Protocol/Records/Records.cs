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

using NKafka.Protocol.Buffers;

namespace NKafka.Protocol.Records;

/// <summary>
/// https://kafka.apache.org/documentation/#record
/// </summary>
internal sealed class Records
{
    public ArrayBuffer Buffer { get; }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<RecordBatch> Batches { get; private set; } = [];

    /// <summary>
    /// The size of these records in bytes.
    /// </summary>
    public int SizeInBytes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Records(ArrayBuffer buffer)
    {
        Buffer = buffer;
    }

    private void Read(ref BufferReader reader, int length)
    {
        // var startOffset = reader.CurrentOffset;
        // var endOffset = startOffset + length;
        //
        // // исходно мы не знаем количество батчей, т.к. они формируются динамически - задать нормальное capacity невозможно
        // var allBathes = new List<IRecordsBatch>(32);
        //
        // while (reader.CurrentOffset < endOffset)
        // {
        //     try
        //     {
        //         var batch = new RecordBatch(ref reader);
        //         allBathes.Add(batch);
        //
        //         if (reader.Remaining >= batch.SizeInBytes)
        //         {
        //             continue;
        //         }
        //
        //         // Оставшиеся байты из буфера не позволяют считать
        //         // корректный батч записей - их мы просто пропускаем
        //         var last = endOffset - reader.CurrentOffset;
        //         reader.Advance(last);
        //
        //         break;
        //     }
        //     catch (Exception exc)
        //     {
        //         Debug.Write(exc);
        //
        //         break;
        //     }
        // }
        //
        // Batches = allBathes;
    }
}