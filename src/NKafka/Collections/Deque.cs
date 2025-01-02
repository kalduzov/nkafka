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
using System.Diagnostics;

namespace NKafka.Collections;

/// <summary>
/// The implementation of the double ended queue is based on the internal implementation of System.Collections.Generic.Deque&lt;T&gt;
/// </summary>
/// <typeparam name="T">The type of elements in the Deque.</typeparam>
[DebuggerDisplay("Count = {Count}")]
internal class Deque<T>: ICollection
    where T : class
{
    private readonly T _defaultItem;
    
    private readonly LinkedList<T> _buffer;

    /// <summary>
    /// Creates a new instance of Deque.
    /// </summary>
    internal Deque(T defaultItem)
    {
        _defaultItem = defaultItem;
        _buffer = [];
    }

    public int Count => _buffer.Count;

    public bool IsSynchronized => false;

    object ICollection.SyncRoot => this;

    /// <summary>
    /// Copies the elements of the <paramref name="array"/> to a specified index in the current queue.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from the queue. The array must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="array"/> is multidimensional, or it has a non-zero lower bound.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of <paramref name="array"/>.</exception>
    /// <exception cref="ArgumentException">The number of elements in the queue is greater than the available space from the specified <paramref name="index"/> to the end of the destination array.</exception>
    /// <exception cref="ArgumentException">The type of the source or destination array is not compatible with the type of the items in the queue.</exception>
    public void CopyTo(Array array, int index)
    {
        var tArray = array as T[];
        _buffer.CopyTo(tArray!, index);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _buffer.GetEnumerator();
    }

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Clears the contents of the collection.
    /// </summary>
    public void Clear()
    {
        _buffer.Clear();
    }

    /// <summary>
    /// Inserts an item at the front of the collection.
    /// </summary>
    /// <param name="item">The item to be inserted.</param>
    public void AddFirst(T item)
    {
        _buffer.AddFirst(item);
    }

    /// <summary>
    /// Inserts an item to the back of the collection.
    /// </summary>
    /// <param name="item">The item to be inserted.</param>
    public void AddLast(T item)
    {
        _buffer.AddLast(item);
    }

    /// <summary>
    /// Removes and returns the element at the front of the array.
    /// </summary>
    /// <returns>The element that was removed from the front of the array.</returns>
    /// <remarks>
    /// The method assumes that the array is not empty. It is the caller's responsibility to ensure that there are elements remaining in the array before calling this method.
    /// The removed element is replaced with a default value of type T.
    /// If the head index reaches the end of the array, it wraps around to 0.
    /// The Count property is decremented by 1 after the element is removed.
    /// </remarks>
    public T RemoveFirst()
    {
        if (_buffer.First is null)
        {
            return _defaultItem;
        }

        var element = _buffer.First.Value;
        _buffer.RemoveFirst();

        return element;
    }

    /// <summary>
    /// Removes and returns the last element in the collection.
    /// </summary>
    /// <returns>The last element in the collection.</returns>
    /// <remarks>
    /// This method removes and returns the last element in the collection,
    /// decrementing the tail index and updating the count accordingly.
    /// If the tail index reaches -1, it wraps around to the end of the internal array.
    /// </remarks>
    public T RemoveLast()
    {
        if (_buffer.Last is null)
        {
            return _defaultItem;
        }

        var element = _buffer.Last.Value;
        _buffer.RemoveLast();

        return element;
    }

    /// <summary>
    /// Retrieves the front element of the underlying array without removing it.
    /// </summary>
    /// <returns>
    /// The front element of the array if it exists; otherwise, the default value of the type <typeparamref name="T"/>.
    /// </returns>
    public T PeekFirst()
    {
        return _buffer.First is null ? _defaultItem : _buffer.First.Value;

    }

    /// <summary>
    /// Returns the last element of the queue without removing it.
    /// </summary>
    /// <returns>The last element of the queue if the queue is not empty; otherwise, the default value of the type.</returns>
    public bool TryPeekLast(out T element)
    {
        if (_buffer.Last is not null)
        {
            element = _buffer.Last.Value;

            return true;
        }
        element = _defaultItem;

        return false;
    }
}