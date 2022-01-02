﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.IO;
using System.Text;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Protocol.Requests;

public class MetadataRequestMessage : RequestBody
{
    /// <summary>
    ///     The topics to fetch metadata for
    /// </summary>
    public string[] Topics { get; }

    /// <summary>
    ///     If this is true, the broker may auto-create topics that we requested which do not already exist, if it is
    ///     configured to do so
    /// </summary>
    public bool AllowAutoTopicCreation { get; set; }

    /// <summary>
    ///     Whether to include cluster authorized operations
    /// </summary>
    public bool IncludeClusterAuthorizedOperations { get; set; }

    /// <summary>
    ///     Whether to include topic authorized operations
    /// </summary>
    public bool IncludeTopicAuthorizedOperations { get; set; }

    public MetadataRequestMessage(ApiVersions version, params string[] topics)
    {
        Version = version;
        Topics = topics;
        ApiKey = ApiKeys.Metadata;
        Length = CalculateLen();
    }

    private int CalculateLen()
    {
        var len = Topics.GetArrayLength(); //4; //Длинна массива topics

        for (var index = Topics.Length - 1; index >= 0; index--)
        {
            var topic = Topics[index];
            len += 2 + Encoding.UTF8.GetByteCount(topic);
        }

        if (Version > ApiVersions.Version3)
        {
            len += 1;
        }

        if (Version > ApiVersions.Version7)
        {
            len += 2;
        }

        if (Version > ApiVersions.Version11)
        {
            len--;
        }

        return len;
    }

    public override void SerializeToStream(Stream stream)
    {
        if (Version == ApiVersions.Version0 || Topics.Length > 0)
        {
            stream.Write(Topics.Length.ToBigEndian());

            foreach (var topic in Topics)
            {
                stream.Write(topic.AsNullableString());
            }
        }
        else
        {
            stream.Write((-1).ToBigEndian());
        }

        if (Version > ApiVersions.Version3)
        {
            stream.WriteByte(AllowAutoTopicCreation.AsByte());
        }

        if (Version is > ApiVersions.Version7 and <= ApiVersions.Version11)
        {
            stream.WriteByte(IncludeClusterAuthorizedOperations.AsByte());
        }

        if (Version > ApiVersions.Version7)
        {
            stream.WriteByte(IncludeTopicAuthorizedOperations.AsByte());
        }
    }
}