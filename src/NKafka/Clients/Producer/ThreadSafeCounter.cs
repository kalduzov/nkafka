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

namespace NKafka.Clients.Producer;

/// <summary>
/// Represents a thread-safe counter that allows for getting and incrementing the value.
/// </summary>
internal class ThreadSafeCounter
{
    private volatile uint _value;

    /// <summary>
    /// Gets the value of the property.
    /// </summary>
    /// <remarks>
    /// This property represents the underlying value stored in the class.
    /// </remarks>
    /// <returns>
    /// The value of the property.
    /// </returns>
    public uint Value => _value;

    /// <summary>
    /// Represents a thread-safe counter.
    /// </summary>
    public ThreadSafeCounter(uint initValue = 0)
    {
        _value = initValue;
    }

    /// <summary>
    /// Retrieves the current value from a specified location, and increments it by one.
    /// </summary>
    /// <returns>
    /// The original value stored in the specified variable, incremented by one.
    /// </returns>
    public uint GetAndIncrement()
    {
        return Interlocked.Increment(ref _value) - 1; // всегда получаем предыдущее значение
    }
}