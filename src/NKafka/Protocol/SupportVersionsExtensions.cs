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
    internal static readonly Version Version20 = new(2, 0);
    internal static readonly Version Version21 = new(2, 1);
    internal static readonly Version Version22 = new(2, 2);
    internal static readonly Version Version23 = new(2, 3);
    internal static readonly Version Version24 = new(2, 4);
    internal static readonly Version Version25 = new(2, 5);
    internal static readonly Version Version26 = new(2, 6);
    internal static readonly Version Version27 = new(2, 7);
    internal static readonly Version Version28 = new(2, 8);
    internal static readonly Version Version30 = new(3, 0);
    internal static readonly Version Version31 = new(3, 1);
    internal static readonly Version Version32 = new(3, 2);
    internal static readonly Version Version33 = new(3, 3);
    internal static readonly Version Version34 = new(3, 4);

    private static readonly SortedDictionary<Version, HashSet<ApiKeysVersion>> _supportSetOfKafkaVersions = new()
    {
        [Version20] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version0),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version2)
        },
        [Version21] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version10),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version0),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version2)
        },
        [Version22] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version10),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version2)
        },
        [Version23] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version3)
        },
        [Version24] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version4)
        },
        [Version25] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version26] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version27] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version12),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version28] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version12),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version6),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version11),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version30] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version12),
            new(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version11),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version31] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version32] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version33] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        },
        [Version34] = new HashSet<ApiKeysVersion>
        {
            new(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        }
    };

    public static Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)> Default { get; set; } = new();

    public static bool IsSupportKafkaVersion(Version version, out (Version Min, Version Max) minMaxVersions)
    {
        minMaxVersions = (_supportSetOfKafkaVersions.First().Key, _supportSetOfKafkaVersions.Last().Key);

        return _supportSetOfKafkaVersions.ContainsKey(version);
    }

    /// <summary>
    /// Возвращает эффективную версию для api
    /// </summary>
    /// <remarks>Эффективная версия - это максимальная версия Api, поддерживаемая всеми брокерами кластера.
    /// Каждый раз, когда меняется состав брокеров в кластере - происходит перерасчет эффективной версии.
    /// Через конфигурацию так же можно изменить общий набор версий для api указав необходимую версию</remarks>
    public static ApiVersion GetEffectiveApiVersion(this ApiKeys apiKey, Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)> supportVersions)
    {
        if (apiKey == ApiKeys.ApiVersions)
        {
            return ApiVersion.Version0;
        }

        if (!supportVersions.TryGetValue(apiKey, out var versions))
        {
            throw new ProtocolKafkaException(ErrorCodes.UnsupportedVersion);
        }

        return versions.MaxVersion;
    }

    internal readonly struct ApiKeysVersion: IEquatable<ApiKeysVersion>
    {
        public ApiKeysVersion(ApiKeys apiKey, ApiVersion minApiVersion, ApiVersion maxApiVersion)
        {
            ApiKey = apiKey;
            MinApiVersion = minApiVersion;
            MaxApiVersion = maxApiVersion;
        }

        public ApiKeys ApiKey { get; }

        public ApiVersion MinApiVersion { get; }

        public ApiVersion MaxApiVersion { get; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ApiKey;
                hashCode = (hashCode * 397) ^ (int)MinApiVersion;
                hashCode = (hashCode * 397) ^ (int)MaxApiVersion;

                return hashCode;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is ApiKeysVersion other && Equals(other);
        }

        public override string ToString()
        {
            return $"ApiKeysVersion(ApiKey={ApiKey}, MaxApiVersion={MaxApiVersion})";
        }

        public void Deconstruct(out ApiKeys apiKey, out ApiVersion minApiVersion, out ApiVersion maxApiVersion)
        {
            apiKey = ApiKey;
            maxApiVersion = MaxApiVersion;
            minApiVersion = MinApiVersion;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(ApiKeysVersion other)
        {
            return ApiKey == other.ApiKey
                   && MinApiVersion == other.MinApiVersion
                   && MaxApiVersion == other.MaxApiVersion;
        }
    }
}