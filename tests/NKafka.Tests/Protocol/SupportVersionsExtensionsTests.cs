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

    [Theory]
    [MemberData(nameof(StageApiFallbackData))]
    public void StageApisShouldMatchFallbackMatrix(Version brokerVersion,
        ApiKeys apiKey,
        bool isPresent,
        ApiVersion minVersion,
        ApiVersion maxVersion)
    {
        var result = SupportVersionsExtensions.TryGetFallbackApiVersionRange(brokerVersion, apiKey, out var versions);

        result.Should().Be(isPresent);

        if (!isPresent)
        {
            return;
        }

        versions.MinVersion.Should().Be(minVersion);
        versions.MaxVersion.Should().Be(maxVersion);
    }

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
        [Version41],
        [Version42],
        [Version43]
    ];

    public static IEnumerable<object[]> StageApiFallbackData()
    {
        var versions = Data.Select(static item => (Version)item[0]).ToArray();
        var stageApis = new[]
        {
            ApiKeys.InitProducerId,
            ApiKeys.TxnOffsetCommit,
            ApiKeys.OffsetForLeaderEpoch,
            ApiKeys.DeleteTopics,
            ApiKeys.DescribeGroups,
            ApiKeys.ListGroups,
            ApiKeys.DeleteGroups,
            ApiKeys.DescribeCluster,
            ApiKeys.DescribeConfigs,
            ApiKeys.AlterConfigs,
            ApiKeys.IncrementalAlterConfigs,
            ApiKeys.DescribeAcls,
            ApiKeys.CreateAcls,
            ApiKeys.DeleteAcls,
            ApiKeys.ConsumerGroupHeartbeat,
            ApiKeys.ConsumerGroupDescribe
        };

        var expected = new Dictionary<(Version Version, ApiKeys ApiKey), (bool IsPresent, ApiVersion MinVersion, ApiVersion MaxVersion)>();

        foreach (var version in versions)
        {
            foreach (var apiKey in stageApis)
            {
                expected[(version, apiKey)] = (false, default, default);
            }
        }

        Add(ApiKeys.InitProducerId,
            (Version22, Version23, ApiVersion.Version0, ApiVersion.Version1),
            (Version24, Version24, ApiVersion.Version0, ApiVersion.Version2),
            (Version25, Version26, ApiVersion.Version0, ApiVersion.Version3),
            (Version27, Version37, ApiVersion.Version0, ApiVersion.Version4),
            (Version38, Version40, ApiVersion.Version0, ApiVersion.Version5),
            (Version41, Version43, ApiVersion.Version0, ApiVersion.Version6));

        Add(ApiKeys.TxnOffsetCommit,
            (Version22, Version24, ApiVersion.Version0, ApiVersion.Version2),
            (Version25, Version37, ApiVersion.Version0, ApiVersion.Version3),
            (Version38, Version39, ApiVersion.Version0, ApiVersion.Version4),
            (Version40, Version43, ApiVersion.Version0, ApiVersion.Version5));

        Add(ApiKeys.OffsetForLeaderEpoch,
            (Version22, Version22, ApiVersion.Version0, ApiVersion.Version2),
            (Version23, Version27, ApiVersion.Version0, ApiVersion.Version3),
            (Version28, Version39, ApiVersion.Version0, ApiVersion.Version4),
            (Version40, Version43, ApiVersion.Version2, ApiVersion.Version4));

        Add(ApiKeys.DeleteTopics,
            (Version22, Version23, ApiVersion.Version0, ApiVersion.Version3),
            (Version24, Version26, ApiVersion.Version0, ApiVersion.Version4),
            (Version27, Version27, ApiVersion.Version0, ApiVersion.Version5),
            (Version28, Version39, ApiVersion.Version0, ApiVersion.Version6),
            (Version40, Version43, ApiVersion.Version1, ApiVersion.Version6));

        Add(ApiKeys.DescribeGroups,
            (Version22, Version22, ApiVersion.Version0, ApiVersion.Version2),
            (Version23, Version23, ApiVersion.Version0, ApiVersion.Version3),
            (Version24, Version39, ApiVersion.Version0, ApiVersion.Version5),
            (Version40, Version43, ApiVersion.Version0, ApiVersion.Version6));

        Add(ApiKeys.ListGroups,
            (Version22, Version23, ApiVersion.Version0, ApiVersion.Version2),
            (Version24, Version25, ApiVersion.Version0, ApiVersion.Version3),
            (Version26, Version37, ApiVersion.Version0, ApiVersion.Version4),
            (Version38, Version43, ApiVersion.Version0, ApiVersion.Version5));

        Add(ApiKeys.DeleteGroups,
            (Version22, Version23, ApiVersion.Version0, ApiVersion.Version1),
            (Version24, Version43, ApiVersion.Version0, ApiVersion.Version2));

        Add(ApiKeys.DescribeCluster,
            (Version28, Version36, ApiVersion.Version0, ApiVersion.Version0),
            (Version37, Version39, ApiVersion.Version0, ApiVersion.Version1),
            (Version40, Version43, ApiVersion.Version0, ApiVersion.Version2));

        Add(ApiKeys.DescribeConfigs,
            (Version22, Version25, ApiVersion.Version0, ApiVersion.Version2),
            (Version26, Version27, ApiVersion.Version0, ApiVersion.Version3),
            (Version28, Version39, ApiVersion.Version0, ApiVersion.Version4),
            (Version40, Version43, ApiVersion.Version1, ApiVersion.Version4));

        Add(ApiKeys.AlterConfigs,
            (Version22, Version27, ApiVersion.Version0, ApiVersion.Version1),
            (Version28, Version43, ApiVersion.Version0, ApiVersion.Version2));

        Add(ApiKeys.IncrementalAlterConfigs,
            (Version23, Version23, ApiVersion.Version0, ApiVersion.Version0),
            (Version24, Version43, ApiVersion.Version0, ApiVersion.Version1));

        Add(ApiKeys.DescribeAcls,
            (Version22, Version24, ApiVersion.Version0, ApiVersion.Version1),
            (Version25, Version32, ApiVersion.Version0, ApiVersion.Version2),
            (Version33, Version39, ApiVersion.Version0, ApiVersion.Version3),
            (Version40, Version43, ApiVersion.Version1, ApiVersion.Version3));

        Add(ApiKeys.CreateAcls,
            (Version22, Version24, ApiVersion.Version0, ApiVersion.Version1),
            (Version25, Version32, ApiVersion.Version0, ApiVersion.Version2),
            (Version33, Version39, ApiVersion.Version0, ApiVersion.Version3),
            (Version40, Version43, ApiVersion.Version1, ApiVersion.Version3));

        Add(ApiKeys.DeleteAcls,
            (Version22, Version24, ApiVersion.Version0, ApiVersion.Version1),
            (Version25, Version32, ApiVersion.Version0, ApiVersion.Version2),
            (Version33, Version39, ApiVersion.Version0, ApiVersion.Version3),
            (Version40, Version43, ApiVersion.Version1, ApiVersion.Version3));

        Add(ApiKeys.ConsumerGroupHeartbeat,
            (Version35, Version39, ApiVersion.Version0, ApiVersion.Version0),
            (Version40, Version43, ApiVersion.Version0, ApiVersion.Version1));

        Add(ApiKeys.ConsumerGroupDescribe,
            (Version37, Version39, ApiVersion.Version0, ApiVersion.Version0),
            (Version40, Version43, ApiVersion.Version0, ApiVersion.Version1));

        foreach (var item in expected)
        {
            yield return
            [
                item.Key.Version,
                item.Key.ApiKey,
                item.Value.IsPresent,
                item.Value.MinVersion,
                item.Value.MaxVersion
            ];
        }

        yield break;

        void Add(ApiKeys apiKey, params (Version Start, Version End, ApiVersion MinVersion, ApiVersion MaxVersion)[] ranges)
        {
            foreach (var version in versions)
            {
                foreach (var range in ranges)
                {
                    if (version.CompareTo(range.Start) < 0 || version.CompareTo(range.End) > 0)
                    {
                        continue;
                    }

                    expected[(version, apiKey)] = (true, range.MinVersion, range.MaxVersion);
                    break;
                }
            }
        }
    }
}
