// //  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// // 
// //  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// // 
// //  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
// // 
// //  Author: Aleksey Kalduzov
// //  Email: alexei.kalduzov@gmail.com
// // 
// //  Licensed under the Apache License, Version 2.0 (the "License");
// //  you may not use this file except in compliance with the License.
// //  You may obtain a copy of the License at
// // 
// //      http://www.apache.org/licenses/LICENSE-2.0
// // 
// //  Unless required by applicable law or agreed to in writing, software
// //  distributed under the License is distributed on an "AS IS" BASIS,
// //  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //  See the License for the specific language governing permissions and
// //  limitations under the License.
//
// using System.Buffers;
// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
// using System.Text;
// using System.Text.Unicode;
//
// using NKafka.Exceptions;
//
// using static System.Buffers.Binary.BinaryPrimitives;
//
// namespace NKafka.Protocol.Buffers;
//
// /// <summary>
// /// 
// /// </summary>
// /// <typeparam name="TBufferWriter"></typeparam>
// [StructLayout(LayoutKind.Auto)]
// public ref partial struct BufferWriter<TBufferWriter>
//     where TBufferWriter : IBufferWriter<byte>
// {
//     private readonly bool _useCompact;
// #if NET7_0_OR_GREATER
//     private ref TBufferWriter _bufferWriter;
//     private ref byte _bufferReference;
// #else
//     private TBufferWriter _bufferWriter;
//     private Span<byte> _bufferReference;
// #endif
//     private int _bufferLength;
//     private int _advancedCount;
//     private int _writtenCount;
//
//     /// <summary>
//     /// 
//     /// </summary>
//     public int WrittenCount => _writtenCount;
//
//     /// <summary>
//     /// 
//     /// </summary>
//     public int BufferLength => _bufferLength;
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="writer"></param>
//     /// <param name="useCompact"></param>
//     public BufferWriter(ref TBufferWriter writer, bool useCompact = false)
//     {
//         _useCompact = useCompact;
// #if NET7_0_OR_GREATER
//         _bufferWriter = ref writer;
//         _bufferReference = ref Unsafe.NullRef<byte>();
// #else
//         _bufferWriter = writer;
//         _bufferReference = default;
// #endif
//         _bufferLength = 0;
//         _writtenCount = 0;
//         _advancedCount = 0;
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="sizeHint"></param>
//     /// <returns></returns>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public ref byte GetSpanReference(int sizeHint)
//     {
//         if (_bufferLength < sizeHint)
//         {
//             RequestNewBuffer(sizeHint);
//         }
// #if NET7_0_OR_GREATER
//         return ref _bufferReference;
// #else
//         return ref MemoryMarshal.GetReference(_bufferReference);
// #endif
//     }
//
//     [MethodImpl(MethodImplOptions.NoInlining)]
//     private void RequestNewBuffer(int sizeHint)
//     {
//         if (_advancedCount != 0)
//         {
//             _bufferWriter.Advance(_advancedCount);
//             _advancedCount = 0;
//         }
//         var span = _bufferWriter.GetSpan(sizeHint);
// #if NET7_0_OR_GREATER
//         _bufferReference = ref MemoryMarshal.GetReference(span);
// #else
//         _bufferReference = span;
// #endif
//         _bufferLength = span.Length;
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="count"></param>
//     /// <exception cref="KafkaException"></exception>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Advance(int count)
//     {
//         if (count == 0)
//             return;
//
//         var rest = _bufferLength - count;
//
//         if (rest < 0)
//         {
//             throw new KafkaException("Cannot advance past the end of the buffer.");
//         }
//
//         _bufferLength = rest;
// #if NET7_0_OR_GREATER
//         _bufferReference = ref Unsafe.Add(ref _bufferReference, count);
// #else
//         _bufferReference = _bufferReference.Slice(count);
// #endif
//         _advancedCount += count;
//         _writtenCount += count;
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Flush()
//     {
//         if (_advancedCount != 0)
//         {
//             _bufferWriter.Advance(_advancedCount);
//             _advancedCount = 0;
//         }
// #if NET7_0_OR_GREATER
//         _bufferReference = ref Unsafe.NullRef<byte>();
// #else
//         _bufferReference = default;
// #endif
//         _bufferLength = 0;
//         _writtenCount = 0;
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="value"></param>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteString(string value)
//     {
//         if (value.Length == 0)
//         {
//             WriteZeroBytes<short>();
//         }
//
//         var source = value.AsSpan();
//
//         var maxByteCount = Encoding.UTF8.GetByteCount(source);
//         ref var destPointer = ref GetSpanReference(maxByteCount + 2); // header
//         var dest = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref destPointer, 2), maxByteCount);
//
// #if NET7_0_OR_GREATER
//         var status = Utf8.FromUtf16(source, dest, out _, out var bytesWritten, replaceInvalidSequences: false);
//
//         if (status != OperationStatus.Done)
//         {
//             throw new KafkaException("Cannot advance past the end of the buffer.");
//         }
// #else
//         var bytesWritten = Encoding.UTF8.GetBytes(value, dest);
// #endif
//         var lenValue = ReverseEndianness((short)bytesWritten);
//         Unsafe.WriteUnaligned(ref destPointer, lenValue);
//         Advance(bytesWritten + 2);
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <typeparam name="T"></typeparam>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteZeroBytes<T>()
//         where T : struct
//     {
//         var size = Unsafe.SizeOf<T>();
//         T zero = default;
//         ref var destPointer = ref GetSpanReference(size);
//         Unsafe.WriteUnaligned(ref destPointer, zero);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write double value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteDouble(double value)
//     {
//         const int size = sizeof(double);
//         ref var destPointer = ref GetSpanReference(size);
//
//         var val = BitConverter.DoubleToInt64Bits(value);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             val = ReverseEndianness(val);
//         }
//
//         Unsafe.WriteUnaligned(ref destPointer, val);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write float value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteFloat(float value)
//     {
//         const int size = sizeof(double);
//         ref var destPointer = ref GetSpanReference(size);
//         var val = BitConverter.SingleToUInt32Bits(value);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             val = ReverseEndianness(val);
//         }
//
//         Unsafe.WriteUnaligned(ref destPointer, val);
//         Advance(size);
//     }
// }