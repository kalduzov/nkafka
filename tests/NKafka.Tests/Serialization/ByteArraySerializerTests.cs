//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
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

public class ByteArraySerializerTests
{
    [Theory]
    [InlineData("3ff0000000000000", "3ff0000000000000")]
    [InlineData("00", "00")]
    public async Task SerializeAsync_Successful(string value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        var serializer = new ByteArraySerializer();
        var result = await serializer.SerializeAsync(Convert.FromHexString(value));

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData("3ff0000000000000", "3ff0000000000000")]
    [InlineData("00", "00")]
    public void Serialize_Successful(string value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        var serializer = new ByteArraySerializer();
        var result = serializer.Serialize(Convert.FromHexString(value));

        serializer.PreferAsync.Should().BeFalse();
        result.Should().BeEquivalentTo(serializeValue);
    }
}