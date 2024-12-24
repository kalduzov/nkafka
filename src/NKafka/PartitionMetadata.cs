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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace NKafka;

/// <summary>
/// Describe partition 
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="T:System.Object" /> class.
/// </remarks>
/// <param name="leader">ID of the broker who is the leader of this partition</param>
/// <param name="partition">Partition number</param>
/// <param name="replicas">IDs of brokers where all replicas of the partition are located</param>
/// <param name="leaderEpoch">Number leader epoch</param>
/// <param name="isr">IDs of brokers where all ISR replicas of the partition are located</param>
/// <param name="offlineReplicas"></param>
public readonly struct PartitionMetadata(
    Partition partition,
    int leader,
    int leaderEpoch,
    IReadOnlyCollection<int> replicas,
    IReadOnlyCollection<int> isr,
    IReadOnlyCollection<int> offlineReplicas): IComparable<PartitionMetadata>, IEquatable<PartitionMetadata>
{
    /// <summary>
    /// Partition number
    /// </summary>
    public Partition Partition { get; } = partition;

    /// <summary>
    /// ID of the broker who is the leader of this partition
    /// </summary>
    public int Leader { get; } = leader;

    /// <summary>
    /// Number leader epoch
    /// </summary>
    public int LeaderEpoch { get; } = leaderEpoch;

    /// <summary>
    /// IDs of brokers where all replicas of the partition are located
    /// </summary>
    public IReadOnlyCollection<int> Replicas { get; } = replicas;

    /// <summary>
    /// IDs of brokers where all ISR replicas of the partition are located
    /// </summary>
    public IReadOnlyCollection<int> Isr { get; } = isr;

    /// <summary>
    /// IDs of brokers that have copies of the partitions, but they are currently offline
    /// </summary>
    public IReadOnlyCollection<int> OfflineReplicas { get; } = offlineReplicas;

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

    /// <inheritdoc />
    public bool Equals(PartitionMetadata other)
    {
        return Partition.Equals(other.Partition);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is PartitionMetadata other && Equals(other);
    }

    public static bool operator ==(PartitionMetadata left, PartitionMetadata right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PartitionMetadata left, PartitionMetadata right)
    {
        return !(left == right);
    }
}