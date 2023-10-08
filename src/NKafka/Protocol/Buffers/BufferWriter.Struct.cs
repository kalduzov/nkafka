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
// using System.Runtime.CompilerServices;
//
// using static System.Buffers.Binary.BinaryPrimitives;
//
// namespace NKafka.Protocol.Buffers;
//
// public ref partial struct BufferWriter<TBufferWriter>
// {
//     /// <summary>
//     /// Write int value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteInt(int value)
//     {
//         const int size = sizeof(int);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write uint value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteUInt(uint value)
//     {
//         const int size = sizeof(uint);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write byte value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteByte(byte value)
//     {
//         const int size = sizeof(byte);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write sbyte value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteSByte(sbyte value)
//     {
//         const int size = sizeof(sbyte);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write short value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteShort(short value)
//     {
//         const int size = sizeof(short);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write ushort value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteUShort(ushort value)
//     {
//         const int size = sizeof(ushort);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write long value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteLong(long value)
//     {
//         const int size = sizeof(long);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
//
//     /// <summary>
//     /// Write ulong value type to buffer 
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void WriteULong(ulong value)
//     {
//         const int size = sizeof(ulong);
//         ref var destPointer = ref GetSpanReference(size);
//
//         if (BitConverter.IsLittleEndian)
//         {
//             value = ReverseEndianness(value);
//         }
//         Unsafe.WriteUnaligned(ref destPointer, value);
//         Advance(size);
//     }
// }