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

using NKafka.Serialization;

namespace NKafka.Tests;

public class ListSerializerTests
{
    [Fact]
    public async Task IntListSerializer_Successful()
    {
        var ser = new ListSerializer<int>(new IntSerializer());
        var data = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };
        var result = await ser.SerializeAsync(data);

        //serialization strategy flat len + null indexes len + data element len + elements  
        var size = 0x4 + 0x4 + 0x4 + sizeof(int) * data.Count;

        result.Length.Should().Be(size);
    }

    [Fact]
    public async Task StringListSerializer_WithNulls_Successful()
    {
        var ser = new ListSerializer<string>(new StringSerializer());
        var data = new List<string>
        {
            "test1",
            null!,
            "test3",
            null!,
            "test5"
        };
        var result = await ser.SerializeAsync(data);

        result.Length.Should().Be(43);
    }
}