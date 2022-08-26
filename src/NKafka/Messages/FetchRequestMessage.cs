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
//
// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class FetchRequestMessage: RequestMessage
{
    /// <summary>
    /// The clusterId if known. This is used to validate metadata fetches prior to broker registration.
    /// </summary>
    public string? ClusterId { get; set; } = "null";
    /// <summary>
    /// The broker ID of the follower, of -1 if this request is from a consumer.
    /// </summary>
    public int ReplicaId { get; set; } = 0;
    /// <summary>
    /// The maximum time in milliseconds to wait for the response.
    /// </summary>
    public int MaxWaitMs { get; set; } = 0;
    /// <summary>
    /// The minimum bytes to accumulate in the response.
    /// </summary>
    public int MinBytes { get; set; } = 0;
    /// <summary>
    /// The maximum bytes to fetch.  See KIP-74 for cases where this limit may not be honored.
    /// </summary>
    public int? MaxBytes { get; set; } = 0x7fffffff;
    /// <summary>
    /// This setting controls the visibility of transactional records. Using READ_UNCOMMITTED (isolation_level = 0) makes all records visible. With READ_COMMITTED (isolation_level = 1), non-transactional and COMMITTED transactional records are visible. To be more concrete, READ_COMMITTED returns all data from offsets smaller than the current LSO (last stable offset), and enables the inclusion of the list of aborted transactions in the result, which allows consumers to discard ABORTED transactional records
    /// </summary>
    public sbyte? IsolationLevel { get; set; } = 0;
    /// <summary>
    /// The fetch session ID.
    /// </summary>
    public int? SessionId { get; set; } = 0;
    /// <summary>
    /// The fetch session epoch, which is used for ordering requests in a session.
    /// </summary>
    public int? SessionEpoch { get; set; } = -1;
    /// <summary>
    /// The topics to fetch.
    /// </summary>
    public List<FetchTopicMessage> Topics { get; set; } = new();
    /// <summary>
    /// In an incremental fetch request, the partitions to remove.
    /// </summary>
    public List<ForgottenTopicMessage> ForgottenTopicsData { get; set; } = new();
    /// <summary>
    /// Rack ID of the consumer making this request
    /// </summary>
    public string? RackId { get; set; } = null!;



}
