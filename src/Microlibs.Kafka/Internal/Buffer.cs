using System;

namespace Microlibs.Kafka.Internal;

public abstract class Buffer<T>
{
    private int _limit;
    private int _mark = -1;
    private int _position;

    public Buffer(int mark, int position, int limit, int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), $"capacity < 0: ({capacity} < 0)");
        }

        Capacity = capacity;

        Limit = limit;
        Position = position;

        if (mark < 0)
        {
            return;
        }

        if (mark > position)
        {
            throw new ArgumentOutOfRangeException(nameof(mark), $"mark > position: ({mark} > {position})");
        }

        _mark = mark;
    }

    /// <summary>
    ///     Get or set this buffer's limit.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int Limit
    {
        get => _limit;
        set
        {
            if (value > Capacity || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Limit));
            }

            _limit = value;

            if (_position > value)
            {
                _position = value;
            }

            if (_mark > value)
            {
                _mark = -1;
            }
        }
    }

    /// <summary>
    ///     Get or set this buffer's position.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int Position
    {
        get => _position;
        set
        {
            if (value > _limit || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Position));
            }

            if (_mark > value)
            {
                _mark = -1;
            }

            _position = value;
        }
    }

    /// <summary>
    ///     Get this buffer's capacity.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// </summary>
    public int Remaining
    {
        get
        {
            var rem = _limit - _position;

            return rem > 0 ? rem : 0;
        }
    }

    /// <summary>
    /// </summary>
    public bool HasRemaining => _position < _limit;

    public abstract bool IsReadOnly { get; }

    public Buffer<T> Mark()
    {
        _mark = _position;

        return this;
    }

    public Buffer<T> Reset()
    {
        var m = _mark;

        if (m < 0)
        {
            throw new InvalidOperationException();
        }

        Position = m;

        return this;
    }

    public Buffer<T> Clear()
    {
        Position = 0;
        Limit = Capacity;
        _mark = -1;

        return this;
    }

    public Buffer<T> Flip()
    {
        Limit = Position;
        Position = 0;
        _mark = -1;

        return this;
    }

    public Buffer<T> Rewind()
    {
        Position = 0;
        _mark = -1;

        return this;
    }

    public abstract Buffer<T> Slice();

    public abstract Buffer<T> Slice(int index, int lenght);

    public abstract Buffer<T> Copy();
}