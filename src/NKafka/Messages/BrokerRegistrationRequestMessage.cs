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

public sealed class BrokerRegistrationRequestMessage: IRequestMessage, IEquatable<BrokerRegistrationRequestMessage>
{
    public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

    public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

    public ApiKeys ApiKey => ApiKeys.BrokerRegistration;

    public ApiVersions Version {get; set;}

    public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

    /// <summary>
    /// The broker ID.
    /// </summary>
    public int BrokerId { get; set; } = 0;

    /// <summary>
    /// The cluster id of the broker process.
    /// </summary>
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>
    /// The incarnation id of the broker process.
    /// </summary>
    public Guid IncarnationId { get; set; } = Guid.Empty;

    /// <summary>
    /// The listeners of this broker
    /// </summary>
    public ListenerCollection Listeners { get; set; } = new ();

    /// <summary>
    /// The features on this broker
    /// </summary>
    public FeatureCollection Features { get; set; } = new ();

    /// <summary>
    /// The rack which this broker is in.
    /// </summary>
    public string Rack { get; set; } = string.Empty;

    public BrokerRegistrationRequestMessage()
    {
    }

    public BrokerRegistrationRequestMessage(BufferReader reader, ApiVersions version)
        : this()
    {
        Read(reader, version);
    }

    public void Read(BufferReader reader, ApiVersions version)
    {
        BrokerId = reader.ReadInt();
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                throw new Exception("non-nullable field ClusterId was serialized as null");
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field ClusterId had invalid length {length}");
            }
            else
            {
                ClusterId = reader.ReadString(length);
            }
        }
        IncarnationId = reader.ReadGuid();
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Listeners was serialized as null");
            }
            else
            {
                ListenerCollection newCollection = new(arrayLength);
                for (var i = 0; i< arrayLength; i++)
                {
                    newCollection.Add(new ListenerMessage(reader, version));
                }
                Listeners = newCollection;
            }
        }
        {
            int arrayLength;
            arrayLength = reader.ReadVarUInt() - 1;
            if (arrayLength < 0)
            {
                throw new Exception("non-nullable field Features was serialized as null");
            }
            else
            {
                FeatureCollection newCollection = new(arrayLength);
                for (var i = 0; i< arrayLength; i++)
                {
                    newCollection.Add(new FeatureMessage(reader, version));
                }
                Features = newCollection;
            }
        }
        {
            int length;
            length = reader.ReadVarUInt() - 1;
            if (length < 0)
            {
                Rack = null;
            }
            else if (length > 0x7fff)
            {
                throw new Exception($"string field Rack had invalid length {length}");
            }
            else
            {
                Rack = reader.ReadString(length);
            }
        }
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
        {
            var stringBytes = Encoding.UTF8.GetBytes(ClusterId);
            writer.WriteVarUInt(stringBytes.Length + 1);
            writer.WriteBytes(stringBytes);
        }
        writer.WriteGuid(IncarnationId);
        writer.WriteVarUInt(Listeners.Count + 1);
        foreach (var element in Listeners)
        {
            element.Write(writer, version);
        }
        writer.WriteVarUInt(Features.Count + 1);
        foreach (var element in Features)
        {
            element.Write(writer, version);
        }
        if (Rack is null)
        {
            writer.WriteVarUInt(0);
        }
        else
        {
            var stringBytes = Encoding.UTF8.GetBytes(Rack);
            writer.WriteVarUInt(stringBytes.Length + 1);
            writer.WriteBytes(stringBytes);
        }
        var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
        numTaggedFields += rawWriter.FieldsCount;
        writer.WriteVarUInt(numTaggedFields);
        rawWriter.WriteRawTags(writer, int.MaxValue);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is BrokerRegistrationRequestMessage other && Equals(other);
    }

    public bool Equals(BrokerRegistrationRequestMessage? other)
    {
        return true;
    }

    public sealed class ListenerMessage: IMessage, IEquatable<ListenerMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The name of the endpoint.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The hostname.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The port.
        /// </summary>
        public ushort Port { get; set; } = 0;

        /// <summary>
        /// The security protocol.
        /// </summary>
        public short SecurityProtocol { get; set; } = 0;

        public ListenerMessage()
        {
        }

        public ListenerMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of ListenerMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field Host was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Host had invalid length {length}");
                }
                else
                {
                    Host = reader.ReadString(length);
                }
            }
            Port = reader.ReadUShort();
            SecurityProtocol = reader.ReadShort();
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            {
                var stringBytes = Encoding.UTF8.GetBytes(Host);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteUShort(Port);
            writer.WriteShort(SecurityProtocol);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is ListenerMessage other && Equals(other);
        }

        public bool Equals(ListenerMessage? other)
        {
            return true;
        }
    }

    public sealed class ListenerCollection: HashSet<ListenerMessage>
    {
        public ListenerCollection()
        {
        }

        public ListenerCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public sealed class FeatureMessage: IMessage, IEquatable<FeatureMessage>
    {
        public ApiVersions LowestSupportedVersion => ApiVersions.Version0;

        public ApiVersions HighestSupportedVersion => ApiVersions.Version0;

        public ApiVersions Version {get; set;}

        public List<TaggedField>? UnknownTaggedFields { get; set; } = null;

        /// <summary>
        /// The feature name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The minimum supported feature level.
        /// </summary>
        public short MinSupportedVersion { get; set; } = 0;

        /// <summary>
        /// The maximum supported feature level.
        /// </summary>
        public short MaxSupportedVersion { get; set; } = 0;

        public FeatureMessage()
        {
        }

        public FeatureMessage(BufferReader reader, ApiVersions version)
            : this()
        {
            Read(reader, version);
        }

        public void Read(BufferReader reader, ApiVersions version)
        {
            if (version > ApiVersions.Version0)
            {
                throw new UnsupportedVersionException($"Can't read version {version} of FeatureMessage");
            }
            {
                int length;
                length = reader.ReadVarUInt() - 1;
                if (length < 0)
                {
                    throw new Exception("non-nullable field Name was serialized as null");
                }
                else if (length > 0x7fff)
                {
                    throw new Exception($"string field Name had invalid length {length}");
                }
                else
                {
                    Name = reader.ReadString(length);
                }
            }
            MinSupportedVersion = reader.ReadShort();
            MaxSupportedVersion = reader.ReadShort();
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
            {
                var stringBytes = Encoding.UTF8.GetBytes(Name);
                writer.WriteVarUInt(stringBytes.Length + 1);
                writer.WriteBytes(stringBytes);
            }
            writer.WriteShort(MinSupportedVersion);
            writer.WriteShort(MaxSupportedVersion);
            var rawWriter = RawTaggedFieldWriter.ForFields(UnknownTaggedFields);
            numTaggedFields += rawWriter.FieldsCount;
            writer.WriteVarUInt(numTaggedFields);
            rawWriter.WriteRawTags(writer, int.MaxValue);
        }


        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is FeatureMessage other && Equals(other);
        }

        public bool Equals(FeatureMessage? other)
        {
            return true;
        }
    }

    public sealed class FeatureCollection: HashSet<FeatureMessage>
    {
        public FeatureCollection()
        {
        }

        public FeatureCollection(int capacity)
            : base(capacity)
        {
        }
    }
}
