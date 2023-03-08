//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

using System.Text;

using NKafka.Clients.Producer.Internals;
using NKafka.Protocol;

namespace NKafka.Tests.Clients.Producer;

public class ProduceBathSerializationTests
{
    private byte[] _testSerialization =
    {
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x3c,
        0x00,
        0x00,
        0x00,
        0x00,
        0x02,
        0x70,
        0x07,
        0x01,
        0x6d,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x01,
        0x86,
        0xcf,
        0x2a,
        0xa0,
        0x85,
        0x00,
        0x00,
        0x01,
        0x86,
        0xcf,
        0x2a,
        0xa0,
        0x85,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0xff,
        0x00,
        0x00,
        0x00,
        0x01,
        0x14,
        0x00,
        0x00,
        0x00,
        0x01,
        0x08,
        0x74,
        0x65,
        0x73,
        0x74,
        0x00
    };

    [Fact]
    public void ProduceBatchSerializeTest()
    {
        var buffer = new byte[_testSerialization.Length + 28]; //21 is possible overhead
        var writer = new BufferWriter(new MemoryStream(buffer, 0, buffer.Length, true, true), 61); //61 is batch header size
        var producerBatch = new ProducerBatch(new TopicPartition("test", 0), writer, 1678512922757); //1678512922757 - test timestamp
        producerBatch.TryAppend(0, null, "test"u8.ToArray(), Headers.Empty, out _);
        producerBatch.Close();
        buffer[.._testSerialization.Length].Should().BeEquivalentTo(_testSerialization);
    }
}