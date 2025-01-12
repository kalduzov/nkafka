// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2024 Aleksey Kalduzov. All rights reserved
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

using System.Buffers;
using System.Runtime.CompilerServices;

namespace NKafka.Protocol.Buffers;

// This class fork from https://github.com/Cysharp/MemoryPack
internal struct BufferSegment(int size)
{
    private byte[] _buffer = ArrayPool<byte>.Shared.Rent(size);

    private int _written = 0;

    public readonly bool IsNull => _buffer == null;

    public int WrittenCount => _written;

    public Span<byte> WrittenBuffer => _buffer.AsSpan(0, _written);

    public Memory<byte> WrittenMemory => _buffer.AsMemory(0, _written);

    public Span<byte> FreeBuffer => _buffer.AsSpan(_written);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        _written += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        if (_buffer is not null)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
        }
        _buffer = null!;
        _written = 0;
    }
}