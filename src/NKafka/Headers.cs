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

using NKafka.Resources;

using NotImplemented = System.NotImplementedException;

namespace NKafka;

/// <summary>
///     A collection of Kafka message headers.
/// </summary>
public class Headers: IEnumerable<Header>
{
    private readonly List<Header> _headers;

    /// <summary>
    /// Represents an empty headers object.
    /// </summary>
    public static readonly Headers Empty = new EmptyHeaders();

    /// <summary>
    ///     Gets the header at the specified index
    /// </summary>
    /// <param name="index">
    ///     The zero-based index of the element to get.
    /// </param>
    public Header this[int index] => _headers[index];

    /// <summary>
    ///     The number of headers in the collection.
    /// </summary>
    public int Count => _headers.Count;

    /// <summary>
    /// Represents a collection of headers.
    /// </summary>
    /// <param name="headers">The initial collection of headers.</param>
    public Headers(IEnumerable<Header>? headers)
    {
        _headers = headers is null ? new() : new List<Header>(headers);
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the headers collection.
    /// </summary>
    /// <returns>
    ///     An enumerator object that can be used to iterate through the headers collection.
    /// </returns>
    public IEnumerator<Header> GetEnumerator()
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
    public virtual void Add(string key, byte[] val)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key), ExceptionMessages.Kafka_message_header_key_cannot_be_null);
        }

        var header = new Header(key, val);
        Add(header);
    }

    /// <summary>
    ///     Append a new header to the collection.
    /// </summary>
    /// <param name="header">
    ///     The header to add to the collection.
    /// </param>
    public virtual void Add(Header header)
    {
        if (header is null)
        {
            throw new ArgumentNullException(nameof(header));
        }

        _headers.Add(header);
    }

    /// <summary>
    ///     Removes all headers for the given key.
    /// </summary>
    /// <param name="key">
    ///     The key to remove all headers for
    /// </param>
    public virtual void Remove(string key)
    {
        _headers.RemoveAll(a => a.Key == key);
    }

    private class HeadersEnumerator: IEnumerator<Header>
    {
        private readonly Headers _headers;

        private int _location = -1;

        public HeadersEnumerator(Headers headers)
        {
            _headers = headers;
        }

        public object Current => ((IEnumerator<Header>)this).Current;

        Header IEnumerator<Header>.Current => _headers._headers[_location];

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

    private sealed class EmptyHeaders(): Headers(null)
    {
        public override void Add(string key, byte[] val)
        {
            throw new NotImplemented();
        }

        public override void Add(Header header)
        {
            throw new NotImplemented();
        }

        public override void Remove(string key)
        {
            throw new NotImplemented();
        }
    }
}