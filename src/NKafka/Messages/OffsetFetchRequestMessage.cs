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

using NKafka.Protocol;

namespace NKafka.Messages;

public sealed partial class OffsetFetchRequestMessage
{
    /// <summary>
    /// 
    /// </summary>
    public static OffsetFetchRequestMessage Build(ApiVersion currentApiVersion,
        string groupId,
        bool requireStable,
        IReadOnlyDictionary<string, Partition[]> topicPartitions)
    {
        if (requireStable && currentApiVersion < ApiVersion.Version7)
        {
            requireStable = false;
        }

        var request = new OffsetFetchRequestMessage
        {
            RequireStable = requireStable,
        };

        if (currentApiVersion >= ApiVersion.Version8)
        {
            var groupMessage = new OffsetFetchRequestGroupMessage
            {
                groupId = groupId,
            };

            foreach (var topicPartition in topicPartitions)
            {
                groupMessage.Topics.Add(new OffsetFetchRequestTopicsMessage
                {
                    Name = topicPartition.Key,
                    PartitionIndexes = topicPartition.Value.Select(x => x.Value).ToList()
                });
            }
            request.Groups.Add(groupMessage);
        }
        else
        {
            request.GroupId = groupId;

            foreach (var topicPartition in topicPartitions)
            {
                request.Topics.Add(new OffsetFetchRequestTopicMessage
                {
                    Name = topicPartition.Key,
                    PartitionIndexes = topicPartition.Value.Select(x => x.Value).ToList()
                });
            }
        }

        return request;
    }
}