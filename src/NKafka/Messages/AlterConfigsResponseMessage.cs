﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

public partial class AlterConfigsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The responses for each resource.
    /// </summary>
    public IReadOnlyCollection<AlterConfigsResourceResponseMessage> Responses { get; set; }

    public AlterConfigsResponseMessage()
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public AlterConfigsResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class AlterConfigsResourceResponseMessage: Message
    {
        /// <summary>
        /// The resource error code.
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// The resource error message, or null if there was no error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The resource type.
        /// </summary>
        public sbyte ResourceType { get; set; }

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName { get; set; }

        public AlterConfigsResourceResponseMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public AlterConfigsResourceResponseMessage(BufferReader reader, ApiVersions version)
            : base(reader, version)
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version2;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}