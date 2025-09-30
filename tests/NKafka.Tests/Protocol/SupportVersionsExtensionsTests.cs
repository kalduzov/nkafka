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

using NKafka.Protocol;

using static NKafka.Protocol.SupportVersionsExtensions;

namespace NKafka.Tests.Protocol;

public sealed class SupportVersionsExtensionsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void AllApiKeysMustByAddedToDictionary(Version version)
    {
        var result = version.IsSupportKafkaVersion(out _);
        result.Should().BeTrue();
    }

    // [Fact]
    // public void ()
    // {
    //     var result = version.IsSupportKafkaVersion(out _);
    //     result.Should().BeTrue();
    // }

    public static IEnumerable<object[]> Data =>
    [
        [Version20],
        [Version21],
        [Version22],
        [Version23],
        [Version24],
        [Version25],
        [Version26],
        [Version27],
        [Version28],
        [Version30],
        [Version31],
        [Version32],
        [Version33],
        [Version34],
        [Version35],
        [Version36],
        [Version37],
        [Version38],
        [Version39],
        [Version40],
        [Version41]
    ];
}