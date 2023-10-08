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

using NKafka.Messages;
using NKafka.Protocol;

namespace NKafka.Tests.Messages;

public partial class FindCoordinatorRequestMessageTests
{
    [Theory]
    [InlineData(ApiVersion.Version0)]
    [InlineData(ApiVersion.Version1)]
    [InlineData(ApiVersion.Version2)]
    [InlineData(ApiVersion.Version3)]
    public void BuildForCoordinatorTestForVersionLessOrEquals3_ShouldSuccess(ApiVersion apiVersion)
    {
        var group = "test";
        var groups = new[]
        {
            group
        };
        var request = FindCoordinatorRequestMessage.Build(apiVersion, groups);

        request.Key.Should().Be(group);
        request.CoordinatorKeys.Should().BeEmpty();
    }

    [Theory]
    [InlineData(ApiVersion.Version0)]
    [InlineData(ApiVersion.Version1)]
    [InlineData(ApiVersion.Version2)]
    [InlineData(ApiVersion.Version3)]
    public void BuildForCoordinatorTestForVersionLessOrEquals3_ShouldThrowException(ApiVersion apiVersion)
    {
        const string group1 = "test1";
        const string group2 = "test2";
        var groups = new[]
        {
            group1,
            group2
        };
        var requestAction = () => FindCoordinatorRequestMessage.Build(apiVersion, groups);

        requestAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(ApiVersion.Version4)]
    public void BuildForCoordinatorTestForVersionGreatThen3_ShouldSuccess(ApiVersion apiVersion)
    {
        var group1 = "test1";
        var group2 = "test2";
        var groups = new[]
        {
            group1,
            group2,
        };
        var request = FindCoordinatorRequestMessage.Build(apiVersion, groups);

        request.Key.Should().BeEmpty();
        request.CoordinatorKeys.Should().HaveCount(2);
        request.CoordinatorKeys.Should()
            .Contain(new[]
            {
                group1,
                group2
            });
    }
}