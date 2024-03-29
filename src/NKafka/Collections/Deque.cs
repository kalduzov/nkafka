﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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
using System.Runtime.CompilerServices;

namespace NKafka.Collections;

/// <summary>
/// The implementation of the double ended queue is based on the internal implementation of System.Collections.Generic.Deque&lt;T&gt;
/// </summary>
[DebuggerDisplay("Count = {Count}")]
internal sealed class Deque<T>: IEnumerable<T>, ICollection, IReadOnlyCollection<T>
    where T : class
{
    private const int _DEFAULT_CAPACITY = 8;

    private T[] _array;
    private int _head; // First valid element in the deque
    private int _tail; // First open slot in the dequeue, unless the dequeue is full
    private int _version;

    public bool IsEmpty => Count == 0;

    public bool IsFull => Count >= _array.Length;

    public Deque()
    {
        _array = new T[_DEFAULT_CAPACITY];
    }

    public Deque(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        var initialCapacity = CalculateInitElements(capacity);
        _array = new T[initialCapacity];
    }

    public Deque(ICollection<T> collection)
        : this(collection.Count)
    {
        foreach (var element in collection)
        {
            PushFront(element);
        }
    }

    public int Count { get; private set; }

    public bool IsSynchronized => false;

    object ICollection.SyncRoot => this;

    public void CopyTo(Array array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Rank != 1)
        {
            throw new ArgumentException(nameof(array));
        }

        if (array.GetLowerBound(0) != 0)
        {
            throw new ArgumentException(nameof(array));
        }

        var arrayLen = array.Length;

        if (index < 0 || index > arrayLen)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (arrayLen - index < Count)
        {
            throw new ArgumentException();
        }

        var numToCopy = Count;

        if (numToCopy == 0)
        {
            return;
        }

        try
        {
            var firstPart = _array.Length - _head < numToCopy ? _array.Length - _head : numToCopy;
            Array.Copy(_array, _head, array, index, firstPart);
            numToCopy -= firstPart;

            if (numToCopy > 0)
            {
                Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
            }
        }
        catch (ArrayTypeMismatchException)
        {
            throw new ArgumentException(nameof(array));
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        var pos = _head;
        var count = Count;

        while (count-- > 0)
        {
            yield return _array[pos];
            pos = (pos + 1) % _array.Length;
        }
    }

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public void Clear()
    {
        if (IsEmpty is not false)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                //Очищаем массив с сылочными типами
                if (_head < _tail)
                {
                    Array.Clear(_array, _head, Count);
                }
                else
                {
                    Array.Clear(_array, _head, _array.Length - _head);
                    Array.Clear(_array, 0, _tail);
                }
            }

            Count = 0;
        }

        _head = 0;
        _tail = 0;
        _version++;
    }

    public void PushBack(T item)
    {
        if (Count == _array.Length)
        {
            Grow();
        }

        _array[_tail] = item;

        _tail++;

        if (_tail == _array.Length)
        {
            _tail = 0;
        }

        Count++;
    }

    public void PushFront(T item)
    {
        if (Count == _array.Length)
        {
            Grow();
        }

        _head = (_head == 0 ? _array.Length : _head) - 1;
        _array[_head] = item;
        Count++;
    }

    public T PopFront()
    {
        Debug.Assert(!IsEmpty); // caller's responsibility to make sure there are elements remaining

        var item = _array[_head];
        _array[_head] = default!;

        _head++;

        if (_head == _array.Length)
        {
            _head = 0;
        }

        Count--;

        return item;
    }

    public T PopBack()
    {
        Debug.Assert(!IsEmpty);

        _tail--;

        if (_tail == -1)
        {
            _tail = _array.Length - 1;
        }

        var item = _array[_tail];
        _array[_tail] = default!;

        Count--;

        return item;
    }

    public T? PeekFront()
    {
        return IsEmpty ? default : _array[_head];
    }

    public T? PeekBack()
    {
        return IsEmpty ? default : _array[_tail];
    }

    private void Grow()
    {
        Debug.Assert(Count == _array.Length);
        Debug.Assert(_head == _tail);

        const int minimumGrow = 4;

        var capacity = _array.Length << 2;

        if (capacity < _array.Length + minimumGrow)
        {
            capacity = _array.Length + minimumGrow;
        }

        var newArray = new T[capacity];

        if (_head == 0)
        {
            Array.Copy(_array, newArray, Count);
        }
        else
        {
            Array.Copy(_array, _head, newArray, 0, _array.Length - _head);
            Array.Copy(_array, 0, newArray, _array.Length - _head, _tail);
        }

        _array = newArray;
        _head = 0;
        _tail = Count;
    }

    private static int CalculateInitElements(int capacity)
    {
        var result = _DEFAULT_CAPACITY;

        if (capacity < result)
        {
            return result;
        }

        result = capacity;
        result |= result >> 1;
        result |= result >> 2;
        result |= result >> 4;
        result |= result >> 8;
        result |= result >> 16;
        result++;

        if (result < 0)
        {
            result >>= 1;
        }

        return result;
    }

    // Implements an enumerator for a Queue.  The enumerator uses the
    // internal version number of the list to ensure that no modifications are
    // made to the list while an enumeration is in progress.
    internal struct Enumerator: IEnumerator<T>,
        IEnumerator
    {
        private readonly Deque<T> _q;
        private readonly int _version;
        private int _index; // -1 = not started, -2 = ended/disposed
        private T? _currentElement;

        internal Enumerator(Deque<T> q)
        {
            _q = q;
            _version = q._version;
            _index = -1;
            _currentElement = default;
        }

        public void Dispose()
        {
            _index = -2;
            _currentElement = default;
        }

        public bool MoveNext()
        {
            if (_version != _q._version)
            {
                throw new InvalidOperationException();
            }

            if (_index == -2)
            {
                return false;
            }

            _index++;

            if (_index == _q.Count)
            {
                // We've run past the last element
                _index = -2;
                _currentElement = default;

                return false;
            }

            // Cache some fields in locals to decrease code size
            var array = _q._array;
            var capacity = array.Length;

            // _index represents the 0-based index into the queue, however the queue
            // doesn't have to start from 0 and it may not even be stored contiguously in memory.

            var arrayIndex = _q._head + _index; // this is the actual index into the queue's backing array

            if (arrayIndex >= capacity)
            {
                // NOTE: Originally we were using the modulo operator here, however
                // on Intel processors it has a very high instruction latency which
                // was slowing down the loop quite a bit.
                // Replacing it with simple comparison/subtraction operations sped up
                // the average foreach loop by 2x.

                arrayIndex -= capacity; // wrap around if needed
            }

            _currentElement = array[arrayIndex];

            return true;
        }

        public T Current
        {
            get
            {
                if (_index < 0)
                {
                    ThrowEnumerationNotStartedOrEnded();
                }

                return _currentElement!;
            }
        }

        private void ThrowEnumerationNotStartedOrEnded()
        {
            Debug.Assert(_index == -1 || _index == -2);

            throw new InvalidOperationException();
        }

        object? IEnumerator.Current => Current;

        void IEnumerator.Reset()
        {
            if (_version != _q._version)
            {
                throw new InvalidOperationException();
            }

            _index = -1;
            _currentElement = default;
        }
    }
}