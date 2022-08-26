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

using FluentAssertions;

using NKafka.MessageGenerator.Specifications;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class VersionsTests
{
    private readonly Versions _defaultVersion = new(0, 12);

    [Theory]
    [InlineData("1-12", 1, 12)]
    [InlineData("1+", 1, short.MaxValue)]
    [InlineData("4+", 4, short.MaxValue)]
    [InlineData("2", 2, 2)]
    [InlineData("2-5", 2, 5)]
    public void VersionTests_Successful(string inputVersion, short lowest, short highest)
    {
        var versions = Versions.Parse(inputVersion, _defaultVersion);
        versions.Highest.Should().Be(highest);
        versions.Lowest.Should().Be(lowest);
    }

    [Theory]
    [InlineData("1-4", "1-2", "3-4")]
    [InlineData("3+", "4+", "3")]
    [InlineData("4+", "3+", "none")]
    [InlineData("1-5", "2-4", null)]
    public void SubtractTests(string oneString, string twoString, string resultString)
    {
        var one = Versions.Parse(oneString, null!);
        var two = Versions.Parse(twoString, null!);
        var result = one! - two!;

        result.Should().Be(Versions.Parse(resultString, null!));
    }
}