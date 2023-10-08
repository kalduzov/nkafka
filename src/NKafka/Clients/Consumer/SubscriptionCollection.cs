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

using System.Collections;

namespace NKafka.Clients.Consumer;

/// <summary>
/// 
/// </summary>
public class SubscriptionCollection: IDictionary<string, Subscription>
{
    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, Subscription>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<string, Subscription> item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, Subscription> item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<string, Subscription>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<string, Subscription> item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public int Count { get; }

    /// <inheritdoc />
    public bool IsReadOnly { get; }

    /// <inheritdoc />
    public void Add(string key, Subscription value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Remove(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, out Subscription value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Subscription this[string key]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ICollection<string>? Keys { get; }

    /// <inheritdoc />
    public ICollection<Subscription>? Values { get; }
}