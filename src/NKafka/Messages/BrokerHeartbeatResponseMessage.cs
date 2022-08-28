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

public sealed class BrokerHeartbeatResponseMessage: IResponseMessage, IEquatable<BrokerHeartbeatResponseMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// Duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request did not violate any quota.
    /// </summary>
    public int ThrottleTimeMs { get; set; } = 0;

    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCode { get; set; } = 0;

    /// <inheritdoc />
    public ErrorCodes Code => (ErrorCodes)ErrorCode;

    /// <summary>
    /// True if the broker has approximately caught up with the latest metadata.
    /// </summary>
    public bool IsCaughtUp { get; set; } = false;

    /// <summary>
    /// True if the broker is fenced.
    /// </summary>
    public bool IsFenced { get; set; } = true;

    /// <summary>
    /// True if the broker should proceed with its shutdown.
    /// </summary>
    public bool ShouldShutDown { get; set; } = false;

    public BrokerHeartbeatResponseMessage()
    {
    }

    public BrokerHeartbeatResponseMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        ThrottleTimeMs = reader.ReadInt();
        ErrorCode = reader.ReadShort();
        IsCaughtUp = reader.ReadByte() != 0;
        IsFenced = reader.ReadByte() != 0;
        ShouldShutDown = reader.ReadByte() != 0;
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
        writer.WriteInt(ThrottleTimeMs);
        writer.WriteShort((short)ErrorCode);
        writer.WriteBool(IsCaughtUp);
        writer.WriteBool(IsFenced);
        writer.WriteBool(ShouldShutDown);
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is BrokerHeartbeatResponseMessage other && Equals(other);
    }

    public bool Equals(BrokerHeartbeatResponseMessage? other)
    {
        return true;
    }
}
