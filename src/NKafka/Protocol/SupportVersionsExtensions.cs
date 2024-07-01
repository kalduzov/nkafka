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
    internal static readonly Version Version35 = new(3, 5);
    internal static readonly Version Version36 = new(3, 6);
    internal static readonly Version Version37 = new(3, 7);
    internal static readonly Version Version38 = new(3, 8);

    private static readonly SortedDictionary<Version, HashSet<ApiKeysVersion>> _supportSetOfKafkaVersions = new()
    {
        [Version20] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version2)
        ],
        [Version21] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version10),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version2)
        ],
        [Version22] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version10),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version2)
        ],
        [Version23] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version3)
        ],
        [Version24] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version4)
        ],
        [Version25] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version26] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version11),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version27] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version28] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version6),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version11),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version30] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version11),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version31] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version32] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version33] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version34] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version35] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version36] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version37] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ],
        [Version38] =
        [
            new ApiKeysVersion(ApiKeys.AddOffsetsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.AddPartitionsToTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.ApiVersions, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.CreateTopics, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.EndTxn, ApiVersion.Version0, ApiVersion.Version3),
            new ApiKeysVersion(ApiKeys.Fetch, ApiVersion.Version0, ApiVersion.Version13),
            new ApiKeysVersion(ApiKeys.FetchSnapshot, ApiVersion.Version0, ApiVersion.Version0),
            new ApiKeysVersion(ApiKeys.FindCoordinator, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.Heartbeat, ApiVersion.Version0, ApiVersion.Version4),
            new ApiKeysVersion(ApiKeys.JoinGroup, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.LeaveGroup, ApiVersion.Version0, ApiVersion.Version5),
            new ApiKeysVersion(ApiKeys.ListOffsets, ApiVersion.Version0, ApiVersion.Version7),
            new ApiKeysVersion(ApiKeys.Metadata, ApiVersion.Version0, ApiVersion.Version12),
            new ApiKeysVersion(ApiKeys.OffsetCommit, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.OffsetFetch, ApiVersion.Version0, ApiVersion.Version8),
            new ApiKeysVersion(ApiKeys.Produce, ApiVersion.Version0, ApiVersion.Version9),
            new ApiKeysVersion(ApiKeys.SaslAuthenticate, ApiVersion.Version0, ApiVersion.Version2),
            new ApiKeysVersion(ApiKeys.SaslHandshake, ApiVersion.Version0, ApiVersion.Version1),
            new ApiKeysVersion(ApiKeys.SyncGroup, ApiVersion.Version0, ApiVersion.Version5)
        ]
    };

    public static Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)> Default { get; set; } = [];

    public static bool IsSupportKafkaVersion(this Version version, out (Version Min, Version Max) minMaxVersions)
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
    public static ApiVersion GetEffectiveApiVersion(this ApiKeys apiKey,
        Dictionary<ApiKeys, (ApiVersion MinVersion, ApiVersion MaxVersion)> supportVersions)
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

    internal readonly struct ApiKeysVersion(ApiKeys apiKey, ApiVersion minApiVersion, ApiVersion maxApiVersion): IEquatable<ApiKeysVersion>
    {
        public ApiKeys ApiKey { get; } = apiKey;

        public ApiVersion MinApiVersion { get; } = minApiVersion;

        public ApiVersion MaxApiVersion { get; } = maxApiVersion;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ApiKey;
                hashCode = hashCode * 397 ^ (int)MinApiVersion;
                hashCode = hashCode * 397 ^ (int)MaxApiVersion;

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