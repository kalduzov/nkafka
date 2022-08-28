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

using NKafka.Exceptions;
using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class BrokerHeartbeatRequestMessage: IRequestMessage, IEquatable<BrokerHeartbeatRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiKeys ApiKey => ApiKeys.BrokerHeartbeat;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The broker ID.
    /// </summary>
    public int BrokerId { get; set; } = 0;

    /// <summary>
    /// The broker epoch.
    /// </summary>
    public long BrokerEpoch { get; set; } = -1;

    /// <summary>
    /// The highest metadata offset which the broker has reached.
    /// </summary>
    public long CurrentMetadataOffset { get; set; } = 0;

    /// <summary>
    /// True if the broker wants to be fenced, false otherwise.
    /// </summary>
    public bool WantFence { get; set; } = false;

    /// <summary>
    /// True if the broker wants to be shut down, false otherwise.
    /// </summary>
    public bool WantShutDown { get; set; } = false;

    public BrokerHeartbeatRequestMessage()
    {
    }

    public BrokerHeartbeatRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        BrokerId = reader.ReadInt();
        BrokerEpoch = reader.ReadLong();
        CurrentMetadataOffset = reader.ReadLong();
        WantFence = reader.ReadByte() != 0;
        WantShutDown = reader.ReadByte() != 0;
        UnknownTaggedFields = null;
        var numTaggedFields = reader.ReadVarUInt();
        for (var t = 0; t < numTaggedFields; t++)
        {
            var tag = reader.ReadVarUInt();
            var size = reader.ReadVarUInt();
            switch (tag)
            {
                default:
                    UnknownTaggedFields = reader.ReadUnknownTaggedField(UnknownTaggedFields, tag, size);
                    break;
            }
        }
    }

    public void Write(BufferWriter writer, ApiVersions version)
    {
        var numTaggedFields = 0;
        writer.WriteInt(BrokerId);
        writer.WriteLong(BrokerEpoch);
        writer.WriteLong(CurrentMetadataOffset);
        writer.WriteBool(WantFence);
        writer.WriteBool(WantShutDown);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is BrokerHeartbeatRequestMessage other && Equals(other);
    }

    public bool Equals(BrokerHeartbeatRequestMessage? other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        hashCode = HashCode.Combine(hashCode, BrokerId, BrokerEpoch, CurrentMetadataOffset, WantFence, WantShutDown);
        return hashCode;
    }

    public override string ToString()
    {
        return "BrokerHeartbeatRequestMessage("
            + "BrokerId=" + BrokerId
            + ", BrokerEpoch=" + BrokerEpoch
            + ", CurrentMetadataOffset=" + CurrentMetadataOffset
            + ", WantFence=" + (WantFence ? "true" : "false")
            + ", WantShutDown=" + (WantShutDown ? "true" : "false")
            + ")";
    }
}
