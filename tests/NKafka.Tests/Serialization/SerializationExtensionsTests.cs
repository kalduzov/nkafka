// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2025 Aleksey Kalduzov. All rights reserved
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

using NKafka.Serialization;

namespace NKafka.Tests.Serialization;

public class SerializationExtensionsTests
{
    [Fact]
    public void ToKafkaBytesInt()
    {
        var value = 1244543.ToKafkaBytes();
        value.Should().HaveCount(4);
        value.Should().Equal(0, 18, 253, 127);
    }

    [Fact]
    public void ToKafkaBytesNull()
    {
        int? val = null;
        var value = val.ToKafkaBytes();
        value.Should().HaveCount(0);
        value.Should().Equal();
    }

    [Fact]
    public void ToKafkaBytesString()
    {
        var value = "test".ToKafkaBytes();
        value.Should().HaveCount(4);
        value.Should().Equal("test"u8.ToArray());
    }
}