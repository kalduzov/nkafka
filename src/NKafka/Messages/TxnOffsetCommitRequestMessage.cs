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

public sealed class TxnOffsetCommitRequestMessage: RequestMessage
{
    /// <summary>
    /// The ID of the transaction.
    /// </summary>
    public string TransactionalId { get; set; } = null!;
    /// <summary>
    /// The ID of the group.
    /// </summary>
    public string GroupId { get; set; } = null!;
    /// <summary>
    /// The current producer ID in use by the transactional ID.
    /// </summary>
    public long ProducerId { get; set; } = 0;
    /// <summary>
    /// The current epoch associated with the producer ID.
    /// </summary>
    public short ProducerEpoch { get; set; } = 0;
    /// <summary>
    /// The generation of the consumer.
    /// </summary>
    public int GenerationId { get; set; } = -1;
    /// <summary>
    /// The member ID assigned by the group coordinator.
    /// </summary>
    public string MemberId { get; set; } = null!;
    /// <summary>
    /// The unique identifier of the consumer instance provided by end user.
    /// </summary>
    public string? GroupInstanceId { get; set; } = "null";
    /// <summary>
    /// Each topic that we want to commit offsets for.
    /// </summary>
    public List<TxnOffsetCommitRequestTopicMessage> Topics { get; set; } = new();



}
