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

using System.Runtime.CompilerServices;

namespace NKafka.Protocol;

internal static class ErrorCodesExtensions
{
    private static readonly HashSet<ErrorCodes> _retriableCodes = new()
    {
        ErrorCodes.CorruptMessage,
        ErrorCodes.UnknownTopicOrPartition,
        ErrorCodes.LeaderNotAvailable,
        ErrorCodes.NotLeaderOrFollower,
        ErrorCodes.RequestTimedOut,
        ErrorCodes.ReplicaNotAvailable,
        ErrorCodes.NetworkException,
        ErrorCodes.CoordinatorLoadInProgress,
        ErrorCodes.CoordinatorNotAvailable,
        ErrorCodes.NotCoordinator,
        ErrorCodes.NotEnoughReplicas,
        ErrorCodes.NotEnoughReplicasAfterAppend,
        ErrorCodes.NotController,
        ErrorCodes.ConcurrentTransactions,
        ErrorCodes.KafkaStorageError,
        ErrorCodes.FetchSessionIdNotFound,
        ErrorCodes.InvalidFetchSessionEpoch,
        ErrorCodes.ListenerNotFound,
        ErrorCodes.FencedLeaderEpoch,
        ErrorCodes.UnknownLeaderEpoch,
        ErrorCodes.OffsetNotAvailable,
        ErrorCodes.PreferredLeaderNotAvailable,
        ErrorCodes.EligibleLeadersNotAvailable,
        ErrorCodes.ElectionNotNeeded,
        ErrorCodes.UnstableOffsetCommit,
        ErrorCodes.ThrottlingQuotaExceeded,
        ErrorCodes.UnknownTopicId,
        ErrorCodes.InconsistentTopicId,
        ErrorCodes.FetchSessionTopicIdError
    };

    private static readonly HashSet<ErrorCodes> _processingRequiredClientCodes = new()
    {
        ErrorCodes.MemberIdRequired,
    };

    /// <summary>
    /// A retriable error is a transient error that if retried may succeed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRetriableCode(this ErrorCodes code)
    {
        return _retriableCodes.Contains(code);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSuccessCode(this ErrorCodes code)
    {
        return code == ErrorCodes.None;
    }

    /// <summary>
    /// Обработка данной ошибки возложена на клиенские механизмы библиотеки  
    /// </summary>
    /// Если при получении ответа от брокера данный код ошибки требует переотправки запроса в измененном виде,
    /// то такая ошибка должна обрабатываться кодом логики библиотеки 
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsProcessingRequiredClient(this ErrorCodes code)
    {
        return _processingRequiredClientCodes.Contains(code);
    }
}