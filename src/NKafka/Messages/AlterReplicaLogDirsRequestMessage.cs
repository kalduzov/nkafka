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

public partial class AlterReplicaLogDirsRequestMessage: RequestMessage
{
    /// <summary>
    /// The alterations to make for each directory.
    /// </summary>
    public IReadOnlyCollection<AlterReplicaLogDirMessage> Dirs { get; set; }

    public AlterReplicaLogDirsRequestMessage()
    {
        ApiKey = ApiKeys.AlterReplicaLogDirs;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version2;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class AlterReplicaLogDirMessage: Message
    {
        /// <summary>
        /// The absolute directory path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The topics to add to the directory.
        /// </summary>
        public IReadOnlyCollection<AlterReplicaLogDirTopicMessage> Topics { get; set; }

        public AlterReplicaLogDirMessage()
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
    public class AlterReplicaLogDirTopicMessage: Message
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The partition indexes.
        /// </summary>
        public IReadOnlyCollection<int> Partitions { get; set; }

        public AlterReplicaLogDirTopicMessage()
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