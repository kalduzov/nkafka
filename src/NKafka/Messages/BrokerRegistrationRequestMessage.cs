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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class BrokerRegistrationRequestMessage: RequestMessage
{
    /// <summary>
    /// The broker ID.
    /// </summary>
    public int BrokerId { get; set; }

    /// <summary>
    /// The cluster id of the broker process.
    /// </summary>
    public string ClusterId { get; set; }

    /// <summary>
    /// The incarnation id of the broker process.
    /// </summary>
    public Guid IncarnationId { get; set; }

    /// <summary>
    /// The listeners of this broker
    /// </summary>
    public IReadOnlyCollection<ListenerMessage> Listeners { get; set; }

    /// <summary>
    /// The features on this broker
    /// </summary>
    public IReadOnlyCollection<FeatureMessage> Features { get; set; }

    /// <summary>
    /// The rack which this broker is in.
    /// </summary>
    public string Rack { get; set; }

    public BrokerRegistrationRequestMessage()
    {
        ApiKey = ApiKeys.BrokerRegistration;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version0;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class ListenerMessage: Message
    {
        /// <summary>
        /// The name of the endpoint.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The hostname.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The port.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The security protocol.
        /// </summary>
        public short SecurityProtocol { get; set; }

        public ListenerMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
    public class FeatureMessage: Message
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimum supported feature level.
        /// </summary>
        public short MinSupportedVersion { get; set; }

        /// <summary>
        /// The maximum supported feature level.
        /// </summary>
        public short MaxSupportedVersion { get; set; }

        public FeatureMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version0;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}