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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class BrokerRegistrationRequestMessage: RequestMessage
{
    /// <summary>
    /// The broker ID.
    /// </summary>
    public int BrokerId { get; set; } = 0;

    /// <summary>
    /// The cluster id of the broker process.
    /// </summary>
    public string ClusterId { get; set; } = "";

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
    public string Rack { get; set; } = "";

    public BrokerRegistrationRequestMessage()
    {
        ApiKey = ApiKeys.BrokerRegistration;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public BrokerRegistrationRequestMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.BrokerRegistration;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    internal override void Read(BufferReader reader, ApiVersions version)
    {
    }

    internal override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public sealed class ListenerMessage: Message
    {
        /// <summary>
        /// The name of the endpoint.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The hostname.
        /// </summary>
        public string Host { get; set; } = "";

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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public ListenerMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class ListenerCollection: HashSet<ListenerMessage>
    {
    }

    public sealed class FeatureMessage: Message
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        public string Name { get; set; } = "";

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
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public FeatureMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        internal override void Read(BufferReader reader, ApiVersions version)
        {
        }

        internal override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }

    public sealed class FeatureCollection: HashSet<FeatureMessage>
    {
    }
}
