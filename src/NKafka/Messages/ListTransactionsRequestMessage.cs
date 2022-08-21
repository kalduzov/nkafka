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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class ListTransactionsRequestMessage: RequestMessage
{
    /// <summary>
    /// The transaction states to filter by: if empty, all transactions are returned; if non-empty, then only transactions matching one of the filtered states will be returned
    /// </summary>
    public IReadOnlyCollection<string> StateFilters { get; set; }

    /// <summary>
    /// The producerIds to filter by: if empty, all transactions will be returned; if non-empty, only transactions which match one of the filtered producerIds will be returned
    /// </summary>
    public IReadOnlyCollection<long> ProducerIdFilters { get; set; }

    public ListTransactionsRequestMessage()
    {
        ApiKey = ApiKeys.ListTransactions;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
        if (Version >= ApiVersions.Version0)
        {
            if (StateFilters is null)
            {
                writer.WriteVarUInt(0);
                Size += 4;
            }
            else
            {
                writer.WriteVarUInt((uint)StateFilters.Count + 1);
                foreach (var val in StateFilters)
                {
                    writer.WriteVarInt(val);
                }
            }
        }
        else
        {
        }

        if (Version >= ApiVersions.Version0)
        {
            if (ProducerIdFilters is null)
            {
                writer.WriteVarUInt(0);
                Size += 4;
            }
            else
            {
                writer.WriteVarUInt((uint)ProducerIdFilters.Count + 1);
                foreach (var val in ProducerIdFilters)
                {
                    writer.WriteVarInt(val);
                }
            }
        }
        else
        {
        }

    }
}