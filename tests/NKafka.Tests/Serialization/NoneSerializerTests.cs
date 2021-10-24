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

public class NoneSerializerTests
{
    [Fact]
    public async Task SerializeAsync_MustBe_Throw_Exception()
    {
        var serializer = NoneSerializer<int>.Instance;

        async Task SerializeMethod()
        {
            var _ = await serializer.SerializeAsync(int.MaxValue);
        }

        await FluentActions
            .Awaiting(SerializeMethod)
            .Should()
            .ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public void Serialize_MustBe_Throw_Exception()
    {
        var serializer = NoneSerializer<int>.Instance;

        void SerializeMethod()
        {
            var _ = serializer.Serialize(int.MaxValue);
        }

        FluentActions
            .Invoking(SerializeMethod)
            .Should()
            .Throw<NotImplementedException>();
    }

    [Fact]
    public void Get_PreferAsync_Value_MustBe_Throw_Exception()
    {
        var serializer = NoneSerializer<int>.Instance;

        void PreferGetMethod()
        {
            var _ = serializer.PreferAsync;
        }

        FluentActions
            .Invoking(PreferGetMethod)
            .Should()
            .Throw<NotImplementedException>();
    }
}