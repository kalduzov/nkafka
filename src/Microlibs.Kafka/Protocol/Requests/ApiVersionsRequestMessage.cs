// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

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

public class ApiVersionsRequestMessage : RequestBody
{
    public string ClientSoftwareName { get; }

    public string ClientSoftwareVersion { get; }

    public ApiVersionsRequestMessage(string clientSoftwareName, string clientSoftwareVersion)
    {
        ClientSoftwareName = clientSoftwareName;
        ClientSoftwareVersion = clientSoftwareVersion;
        Version = ApiVersions.Version3;
        ApiKey = ApiKeys.ApiVersions;
        Length = CalculateLen();
    }

    private int CalculateLen()
    {
        var len = 0;

        if (Version >= ApiVersions.Version3)
        {
            var clientSoftwareNameLen = Encoding.UTF8.GetByteCount(ClientSoftwareName);
            len += clientSoftwareNameLen.GetVarIntLen() + clientSoftwareNameLen;
            var clientSoftwareVersionLen = Encoding.UTF8.GetByteCount(ClientSoftwareVersion);
            len += clientSoftwareVersionLen.GetVarIntLen() + clientSoftwareVersionLen;
            len += 1; //empty tagged fields
        }

        return len;
    }

    public override void SerializeToStream(Stream stream)
    {
        if (Version >= ApiVersions.Version3)
        {
            
        }
    }
}