﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

public partial class DescribeTransactionsRequestMessage: RequestMessage
{
    /// <summary>
    /// Array of transactionalIds to include in describe results. If empty, then no results will be returned.
    /// </summary>
    public IReadOnlyCollection<string> TransactionalIds { get; set; }

    public DescribeTransactionsRequestMessage()
    {
        ApiKey = ApiKeys.DescribeTransactions;
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
            if (TransactionalIds is null)
            {
                writer.WriteVarUInt(0);
                Size += 4;
            }
            else
            {
                writer.WriteVarUInt((uint)TransactionalIds.Count + 1);
                foreach (var val in TransactionalIds)
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