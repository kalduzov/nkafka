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

using Xunit;

namespace NKafka.Tests;

public class CollectionExtensionsTests
{
    private static readonly int[] _data =
    {
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10
    };

    private readonly int[] _longData;

    private readonly IReadOnlyCollection<int> _data2 = _data;
    private readonly ICollection<int> _data3 = _data;
    private readonly List<int> _data4 = new(_data);

    public CollectionExtensionsTests()
    {
        _longData = new int[short.MaxValue];

        for (var i = 0; i < short.MaxValue; i++)
        {
            _longData[i] = i;
        }
    }

    [Fact]
    public void InternalShuffle_WithCopyList_Successful()
    {
        var newData = CollectionExtensions.InternalShuffle(_data.ToArray());
        newData.Should().NotContainInOrder(_data);
    }

    [Fact]
    public void InternalShuffleTest_WithoutCopyList_Successful()
    {
        var newData = CollectionExtensions.InternalShuffle(_data);
        newData.Should().ContainInOrder(_data);
    }

    [Fact]
    public void ShuffleIReadOnlyCollection_Successful()
    {
        var newData = _data2.Shuffle();
        newData.Should().NotContainInOrder(_data2);
    }

    [Fact]
    public void ShuffleICollection_Successful()
    {
        var newData = _data3.Shuffle();
        newData.Should().NotContainInOrder(_data3);
    }

    [Fact]
    public void ShuffleArray_Successful()
    {
        var newData = _data.Shuffle();
        newData.Should().NotContainInOrder(_data);
    }

    [Fact]
    public void ShuffleLongArray_Successful()
    {
        var newData = _longData.Shuffle();
        newData.Should().NotContainInOrder(_longData);
    }

    [Fact]
    public void ShuffleList_Successful()
    {
        var newData = _data4.Shuffle();
        newData.Should().NotContainInOrder(_data4);
    }
}