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

namespace NKafka;

/// <summary>
/// Описывает состояние партиции 
/// </summary>
public readonly struct PartitionMetadata: IComparable<PartitionMetadata>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
    /// </summary>
    public PartitionMetadata(Partition partition,
        int leader,
        int leaderEpoch,
        IReadOnlyCollection<int> replicas,
        IReadOnlyCollection<int> isr,
        IReadOnlyCollection<int> offlineReplicas)
    {
        Partition = partition;
        Leader = leader;
        LeaderEpoch = leaderEpoch;
        Replicas = replicas;
        Isr = isr;
        OfflineReplicas = offlineReplicas;
    }

    /// <summary>
    ///  Партиция
    /// </summary>
    public Partition Partition { get; }

    /// <summary>
    /// Id ноды, которая является лидером для данной партиции
    /// </summary>
    public int Leader { get; }

    /// <summary>
    /// Номер эпохи лидера
    /// </summary>
    public int LeaderEpoch { get; }

    /// <summary>
    /// Список id нод, которые являются репликами
    /// </summary>
    public IReadOnlyCollection<int> Replicas { get; }

    /// <summary>
    /// Список id нод, который являются сейчас isr репликами.
    /// </summary>
    public IReadOnlyCollection<int> Isr { get; }

    /// <summary>
    /// Список id нод реплик, которые сейчас offline
    /// </summary>
    public IReadOnlyCollection<int> OfflineReplicas { get; }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Partition;
    }

    /// <inheritdoc/>
    public int CompareTo(PartitionMetadata other)
    {
        return Partition.CompareTo(other.Partition);
    }
}