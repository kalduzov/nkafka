// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 *
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NKafka.Protocol;
using NKafka.Resources;

namespace NKafka.Messages;

public sealed partial class FindCoordinatorRequestMessage
{
    private const int _KEY_TYPE_GROUP = 0;

    /// <summary>
    /// Создает корректный запрос с учетом версии API
    /// </summary>
    /// <param name="version"></param>
    /// <param name="groupIds"></param>
    public static FindCoordinatorRequestMessage Build(ApiVersion version, string[] groupIds)
    {
        var findCoordinatorRequestMessage = new FindCoordinatorRequestMessage
        {
            KeyType = _KEY_TYPE_GROUP,
        };

        if (version <= ApiVersion.Version3) // начиная с 4 версии Api FindCoordinator может быть осуществлен сразу для нескольких групп
        {
            if (groupIds.Length != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(groupIds), ExceptionMessages.NoBatchedFindCoordinatorsExceptionMessage);
            }

            findCoordinatorRequestMessage.Key = groupIds[0];
        }
        else
        {
            findCoordinatorRequestMessage.CoordinatorKeys = groupIds.ToList();
        }

        return findCoordinatorRequestMessage;
    }
}