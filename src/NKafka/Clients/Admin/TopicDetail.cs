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
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

namespace NKafka.Clients.Admin;

/// <summary>
/// A new topic to be created via <see cref="IAdminClient.CreateTopicsAsync"/> 
/// </summary>
/// <param name="Name">The name of the topic to be created</param>
/// <param name="Partitions">The number of partitions for the new topic or -1 if a replica assignment has been specified</param>
/// <param name="ReplicationFactor">The replication factor for the new topic or -1 if a replica assignment has been specified</param>
/// <param name="ReplicaAssignment">A map from partition id to replica ids (i.e. broker ids) or null if the number of partitions
/// and replication factor have been specified instead</param>
/// <param name="Configs">Set the configuration to use on the new topic</param>
public record TopicDetail(string Name,
    int Partitions,
    short ReplicationFactor,
    Dictionary<int, int> ReplicaAssignment,
    Dictionary<string, string> Configs);