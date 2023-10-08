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
// using NKafka.Protocol.Buffers;
//
// namespace NKafka.Tests.Protocol.Buffers;
//
// public class BufferWriterTests
// {
//     [Fact]
//     public void WriteStringTests()
//     {
//         var buf = new byte[7];
//         var fa = new FixedArrayBufferWriter(buf);
//         var bw = new BufferWriter<FixedArrayBufferWriter>(ref fa);
//         bw.WriteString("range");
//         bw.Flush();
//
//         buf.SequenceEqual(new byte[]
//             {
//                 0x00,
//                 0x05,
//                 0x72,
//                 0x61,
//                 0x6e,
//                 0x67,
//                 0x65
//             })
//             .Should()
//             .BeTrue();
//     }
//
//     [Fact]
//     public void WriteZeroByteTest()
//     {
//         var buf = new byte[1];
//         var fa = new FixedArrayBufferWriter(buf);
//         var bw = new BufferWriter<FixedArrayBufferWriter>(ref fa);
//         bw.WriteZeroBytes<byte>();
//         bw.Flush();
//         buf[0].Should().Be(0);
//     }
//
//     [Fact]
//     public void WriteByteTest()
//     {
//         var buf = new byte[1];
//         var fa = new FixedArrayBufferWriter(buf);
//         var bw = new BufferWriter<FixedArrayBufferWriter>(ref fa);
//         bw.WriteByte(1);
//         bw.Flush();
//         buf[0].Should().Be(1);
//     }
//
//     [Fact]
//     public void WriteSByteTest()
//     {
//         var buf = new byte[1];
//         var fa = new FixedArrayBufferWriter(buf);
//         var bw = new BufferWriter<FixedArrayBufferWriter>(ref fa);
//         bw.WriteSByte(-1);
//         bw.Flush();
//         ((sbyte)buf[0]).Should().Be(-1);
//     }
//
//     [Fact]
//     public void WriteShortTest()
//     {
//         var buf = new byte[2];
//         var fa = new FixedArrayBufferWriter(buf);
//         var bw = new BufferWriter<FixedArrayBufferWriter>(ref fa);
//         bw.WriteShort(short.MaxValue);
//         bw.Flush();
//         buf.SequenceEqual(new byte[]
//             {
//                 0x7f,
//                 0xff
//             })
//             .Should()
//             .BeTrue();
//     }
//
//     [Fact]
//     public void WriteUShortTest()
//     {
//         var buf = new byte[2];
//         var fa = new FixedArrayBufferWriter(buf);
//         var bw = new BufferWriter<FixedArrayBufferWriter>(ref fa);
//         bw.WriteUShort(45000);
//         bw.Flush();
//         buf.SequenceEqual(new byte[]
//             {
//                 0xaf,
//                 0xc8
//             })
//             .Should()
//             .BeTrue();
//     }
//
//     [Fact]
//     public void WriteIntTest()
//     {
//         var buf = new byte[4];
//         var fa = new FixedArrayBufferWriter(buf);
//         var bw = new BufferWriter<FixedArrayBufferWriter>(ref fa);
//         bw.WriteInt(300000);
//         bw.Flush();
//         buf.SequenceEqual(new byte[]
//             {
//                 0x00,
//                 0x04,
//                 0x93,
//                 0xe0
//             })
//             .Should()
//             .BeTrue();
//     }
// }