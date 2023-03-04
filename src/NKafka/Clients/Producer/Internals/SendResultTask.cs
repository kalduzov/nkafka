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

internal class SendResultTask: TaskCompletionSource<RecordMetadata>
{
    private readonly long _createTimestamp;
    private readonly int _recordsCount;
    private readonly TaskCompletionSource _result;
    private readonly int _serializedKeySize;
    private readonly int _serializedValueSize;

    public RecordMetadata Value { get; set; }

    public SendResultTask(
        TaskCompletionSource result,
        int recordsCount,
        long createTimestamp,
        int serializedKeySize,
        int serializedValueSize)
        : base(TaskCreationOptions.RunContinuationsAsynchronously)
    {
        _result = result;
        _recordsCount = recordsCount;
        _createTimestamp = createTimestamp;
        _serializedKeySize = serializedKeySize;
        _serializedValueSize = serializedValueSize;
    }
}