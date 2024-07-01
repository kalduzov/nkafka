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

    public static IEnumerable<object[]> Data =>
    [
        [SupportVersionsExtensions.Version20],
        [SupportVersionsExtensions.Version21],
        [SupportVersionsExtensions.Version22],
        [SupportVersionsExtensions.Version23],
        [SupportVersionsExtensions.Version24],
        [SupportVersionsExtensions.Version25],
        [SupportVersionsExtensions.Version26],
        [SupportVersionsExtensions.Version27],
        [SupportVersionsExtensions.Version28],
        [SupportVersionsExtensions.Version30],
        [SupportVersionsExtensions.Version31],
        [SupportVersionsExtensions.Version32],
        [SupportVersionsExtensions.Version33],
        [SupportVersionsExtensions.Version34],
        [SupportVersionsExtensions.Version35],
        [SupportVersionsExtensions.Version36],
        [SupportVersionsExtensions.Version37],
        [SupportVersionsExtensions.Version38],
    ];
}