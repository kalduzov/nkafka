//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

using NKafka.Exceptions;
using NKafka.Protocol;

namespace NKafka.Messages;

public sealed partial class JoinGroupRequestMessage
{
    /// <summary>
    /// Builder to create a JoinGroupRequestMessage object based on API version
    /// </summary>
    public static JoinGroupRequestMessage Build(ApiVersion currentApiVersion,
        string groupId,
        string? groupInstanceId,
        JoinGroupRequestProtocolCollection protocols,
        int rebalanceTimeoutMs,
        int sessionTimeoutMs,
        string memberId)
    {
        if (groupInstanceId is not null && currentApiVersion < ApiVersion.Version5)
        {
            throw new UnsupportedVersionException("Нельзя использовать groupInstanceId, т.к. текущая версия протокола это не поддерживает");
        }

        if (currentApiVersion == ApiVersion.Version0)
        {
            rebalanceTimeoutMs = sessionTimeoutMs;
        }

        var joinGroupRequestMessage = new JoinGroupRequestMessage
        {
            Protocols = protocols,
            GroupId = groupId,
            ProtocolType = "consumer",
            Reason = string.Empty,
            RebalanceTimeoutMs = rebalanceTimeoutMs,
            SessionTimeoutMs = sessionTimeoutMs,
            MemberId = memberId,
            GroupInstanceId = groupInstanceId
        };

        return joinGroupRequestMessage;
    }
}