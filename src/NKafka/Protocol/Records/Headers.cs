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

using System.Collections;

namespace NKafka.Protocol.Records;

/// <summary>
///     A collection of Kafka message headers.
/// </summary>
/// <remarks>
///     Message headers are supported by v0.11 brokers and above.
/// </remarks>
public class Headers: IEnumerable<IHeader>
{
    public static readonly Headers Empty = new();

    private readonly List<IHeader> _headers = new();

    /// <summary>
    ///     Gets the header at the specified index
    /// </summary>
    /// <param name="index">
    ///     The zero-based index of the element to get.
    /// </param>
    public IHeader this[int index] => _headers[index];

    /// <summary>
    ///     The number of headers in the collection.
    /// </summary>
    public int Count => _headers.Count;

    /// <summary>
    ///     Returns an enumerator that iterates through the headers collection.
    /// </summary>
    /// <returns>
    ///     An enumerator object that can be used to iterate through the headers collection.
    /// </returns>
    public IEnumerator<IHeader> GetEnumerator()
    {
        return new HeadersEnumerator(this);
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the headers collection.
    /// </summary>
    /// <returns>
    ///     An enumerator object that can be used to iterate through the headers collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new HeadersEnumerator(this);
    }

    /// <summary>
    ///     Append a new header to the collection.
    /// </summary>
    /// <param name="key">
    ///     The header key.
    /// </param>
    /// <param name="val">
    ///     The header value (possibly null). Note: A null
    ///     header value is distinct from an empty header
    ///     value (array of length 0).
    /// </param>
    public void Add(string key, byte[] val)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key), "Kafka message header key cannot be null");
        }

        _headers.Add(new Header(key, val));
    }

    /// <summary>
    ///     Append a new header to the collection.
    /// </summary>
    /// <param name="header">
    ///     The header to add to the collection.
    /// </param>
    public void Add(Header header)
    {
        _headers.Add(header);
    }

    /// <summary>
    ///     Get the value of the latest header with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The key to get the associated value of.
    /// </param>
    /// <returns>
    ///     The value of the latest element in the collection with the specified key.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     The key <paramref name="key" /> was not present in the collection.
    /// </exception>
    public byte[] GetLastBytes(string key)
    {
        if (TryGetLastBytes(key, out var result))
        {
            return result;
        }

        throw new KeyNotFoundException($"The key {key} was not present in the headers collection.");
    }

    /// <summary>
    ///     Try to get the value of the latest header with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The key to get the associated value of.
    /// </param>
    /// <param name="lastHeader">
    ///     The value of the latest element in the collection with the
    ///     specified key, if a header with that key was present in the
    ///     collection.
    /// </param>
    /// <returns>
    ///     true if the a value with the specified key was present in
    ///     the collection, false otherwise.
    /// </returns>
    public bool TryGetLastBytes(string key, out byte[] lastHeader)
    {
        for (var i = _headers.Count - 1; i >= 0; --i)
        {
            if (_headers[i].Key != key)
            {
                continue;
            }

            lastHeader = _headers[i].GetValueBytes();

            return true;
        }

        lastHeader = default!;

        return false;
    }

    /// <summary>
    ///     Removes all headers for the given key.
    /// </summary>
    /// <param name="key">
    ///     The key to remove all headers for
    /// </param>
    public void Remove(string key)
    {
        _headers.RemoveAll(a => a.Key == key);
    }

    internal class HeadersEnumerator: IEnumerator<IHeader>
    {
        private readonly Headers _headers;

        private int _location = -1;

        public HeadersEnumerator(Headers headers)
        {
            _headers = headers;
        }

        public object Current => ((IEnumerator<IHeader>)this).Current;

        IHeader IEnumerator<IHeader>.Current => _headers._headers[_location];

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _location += 1;

            if (_location >= _headers._headers.Count)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            _location = -1;
        }
    }
}