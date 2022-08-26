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

public sealed class FindCoordinatorResponseMessage: ResponseMessage
{
    /// <summary>
    /// The error code, or 0 if there was no error.
    /// </summary>
    public short ErrorCodeMessage { get; set; } = 0;

    /// <summary>
    /// The error message, or null if there was no error.
    /// </summary>
    public string ErrorMessageMessage { get; set; } = "";

    /// <summary>
    /// The node id.
    /// </summary>
    public int NodeIdMessage { get; set; } = 0;

    /// <summary>
    /// The host name.
    /// </summary>
    public string HostMessage { get; set; } = "";

    /// <summary>
    /// The port.
    /// </summary>
    public int PortMessage { get; set; } = 0;

    /// <summary>
    /// Each coordinator result in the response
    /// </summary>
    public List<CoordinatorMessage> CoordinatorsMessage { get; set; } = new ();

    public FindCoordinatorResponseMessage()
    {
        ApiKey = ApiKeys.FindCoordinator;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public FindCoordinatorResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        Read(reader, version);
        ApiKey = ApiKeys.FindCoordinator;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }


    public sealed class CoordinatorMessage: Message
    {
        /// <summary>
        /// The coordinator key.
        /// </summary>
        public string KeyMessage { get; set; } = "";

        /// <summary>
        /// The node id.
        /// </summary>
        public int NodeIdMessage { get; set; } = 0;

        /// <summary>
        /// The host name.
        /// </summary>
        public string HostMessage { get; set; } = "";

        /// <summary>
        /// The port.
        /// </summary>
        public int PortMessage { get; set; } = 0;

        /// <summary>
        /// The error code, or 0 if there was no error.
        /// </summary>
        public short ErrorCodeMessage { get; set; } = 0;

        /// <summary>
        /// The error message, or null if there was no error.
        /// </summary>
        public string ErrorMessageMessage { get; set; } = "";

        public CoordinatorMessage()
        {
            LowestSupportedVersion = ApiVersions.Version4;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public CoordinatorMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            Read(reader, version);
            LowestSupportedVersion = ApiVersions.Version4;
            HighestSupportedVersion = ApiVersions.Version32767;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }

    }
}
