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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NKafka.Exceptions;

namespace NKafka.Protocol;

internal static partial class SupportVersionsExtensions
{
    internal static readonly Version NotSetVersion = new(0, 0);

    private static readonly SortedDictionary<Version, HashSet<ApiKeys>> _supportSetOfKafkaVersions = new()
    {
        [Version.Parse("1.0")] = new HashSet<ApiKeys>(),
        [Version.Parse("1.1")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.0")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.1")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.2")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.3")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.4")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.5")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.6")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.7")] = new HashSet<ApiKeys>(),
        [Version.Parse("2.8")] = new HashSet<ApiKeys>(),
        [Version.Parse("3.0")] = new HashSet<ApiKeys>(),
        [Version.Parse("3.1")] = new HashSet<ApiKeys>(),
        [Version.Parse("3.2")] = new HashSet<ApiKeys>(),
        [Version.Parse("3.3")] = new HashSet<ApiKeys>(),
        [Version.Parse("3.4")] = new HashSet<ApiKeys>()
    };

    public static Dictionary<ApiKeys, (short, short)> Default { get; set; } = new();

    public static bool IsSupportKafkaVersion(Version version, out (Version Min, Version Max) minMaxVersions)
    {
        minMaxVersions = (_supportSetOfKafkaVersions.First().Key, _supportSetOfKafkaVersions.Last().Key);

        return _supportSetOfKafkaVersions.ContainsKey(version);
    }

    public static ApiVersion GetSupportApiVersion(this ApiKeys apiKey, Dictionary<ApiKeys, (short low, short hi)> supportVersions)
    {
        if (apiKey == ApiKeys.ApiVersions)
        {
            return ApiVersion.Version0;
        }

        if (!supportVersions.TryGetValue(apiKey, out var versions))
        {
            throw new ProtocolKafkaException(ErrorCodes.UnsupportedVersion);
        }

        return (ApiVersion)versions.hi;
    }
}