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

namespace NKafka.Connection;

internal class RoundRobinNumberCounter: INumberCounter
{
    private readonly int _maxNumber;
    private int _lastNumber;

    public RoundRobinNumberCounter(int maxNumber)
    {
        _maxNumber = maxNumber;
        _lastNumber = -1;
    }

    public int GetNextNumber(int initIndex = -1)
    {
        int initial, computed;

        do
        {
            initial = _lastNumber;
            computed = initial + 1;
            // ReSharper disable once RedundantAssignment
#pragma warning disable IDE0059
            computed = computed >= _maxNumber ? computed = 0 : computed;
#pragma warning restore IDE0059
        } while (Interlocked.CompareExchange(ref _lastNumber, computed, initial) != initial);

        return computed;
    }
}