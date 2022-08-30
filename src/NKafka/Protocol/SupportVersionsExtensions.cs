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

// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using NKafka.Exceptions;

namespace NKafka.Protocol;

internal static class SupportVersionsExtensions
{
    public static ApiVersion GetHeaderVersion(this ApiKeys apiKey, ApiVersion version)
    {
        return apiKey switch
        {
            ApiKeys.AddOffsetsToTxn => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.AddPartitionsToTxn => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.AllocateProducerIds => ApiVersion.Version2,
            ApiKeys.AlterClientQuotas => version >= ApiVersion.Version1 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.AlterConfigs => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.AlterPartitionReassignments => ApiVersion.Version2,
            ApiKeys.AlterPartition => ApiVersion.Version2,
            ApiKeys.AlterReplicaLogDirs => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.AlterUserScramCredentials => ApiVersion.Version2,
            ApiKeys.ApiVersions => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.BeginQuorumEpoch => ApiVersion.Version1,
            ApiKeys.BrokerHeartbeat => ApiVersion.Version2,
            ApiKeys.BrokerRegistration => ApiVersion.Version2,
            ApiKeys.ControlledShutdown => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.CreateAcls => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.CreateDelegationToken => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.CreatePartitions => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.CreateTopics => version >= ApiVersion.Version5 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DeleteAcls => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DeleteGroups => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DeleteRecords => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DeleteTopics => version >= ApiVersion.Version4 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DescribeAcls => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DescribeClientQuotas => version >= ApiVersion.Version1 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DescribeCluster => ApiVersion.Version2,
            ApiKeys.DescribeConfigs => version >= ApiVersion.Version4 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DescribeDelegationToken => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DescribeGroups => version >= ApiVersion.Version5 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DescribeLogDirs => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.DescribeProducers => ApiVersion.Version2,
            ApiKeys.DescribeQuorum => ApiVersion.Version2,
            ApiKeys.DescribeTransactions => ApiVersion.Version2,
            ApiKeys.DescribeUserScramCredentials => ApiVersion.Version2,
            ApiKeys.ElectLeaders => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.EndQuorumEpoch => ApiVersion.Version1,
            ApiKeys.EndTxn => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.Envelope => ApiVersion.Version2,
            ApiKeys.ExpireDelegationToken => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.Fetch => version >= ApiVersion.Version12 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.FetchSnapshot => ApiVersion.Version2,
            ApiKeys.FindCoordinator => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.Heartbeat => version >= ApiVersion.Version4 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.IncrementalAlterConfigs => version >= ApiVersion.Version1 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.InitProducerId => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.JoinGroup => version >= ApiVersion.Version6 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.LeaderAndIsr => version >= ApiVersion.Version4 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.LeaveGroup => version >= ApiVersion.Version4 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.ListGroups => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.ListOffsets => version >= ApiVersion.Version6 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.ListPartitionReassignments => ApiVersion.Version2,
            ApiKeys.ListTransactions => ApiVersion.Version2,
            ApiKeys.Metadata => version >= ApiVersion.Version9 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.OffsetCommit => version >= ApiVersion.Version8 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.OffsetDelete => ApiVersion.Version1,
            ApiKeys.OffsetFetch => version >= ApiVersion.Version6 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.OffsetForLeaderEpoch => version >= ApiVersion.Version4 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.Produce => version >= ApiVersion.Version9 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.RenewDelegationToken => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.SaslAuthenticate => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.SaslHandshake => ApiVersion.Version1,
            ApiKeys.StopReplica => version >= ApiVersion.Version2 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.SyncGroup => version >= ApiVersion.Version4 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.TxnOffsetCommit => version >= ApiVersion.Version3 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.UnregisterBroker => ApiVersion.Version2,
            ApiKeys.UpdateFeatures => ApiVersion.Version2,
            ApiKeys.UpdateMetadata => version >= ApiVersion.Version6 ? ApiVersion.Version2 : ApiVersion.Version1,
            ApiKeys.Vote => ApiVersion.Version2,
            ApiKeys.WriteTxnMarkers => version >= ApiVersion.Version1 ? ApiVersion.Version2 : ApiVersion.Version1,
            _ => throw new UnsupportedVersionException($"Unsupported API key {apiKey}")
        };
    }
}