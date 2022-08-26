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

namespace NKafka.Protocol;

public abstract class Message
{
    public ApiVersions Version { get; set; }

    /// <summary>
    /// Returns the number of bytes it would take to write out this message.
    /// </summary>
    public int MessageSize { get; set; }

    /// <summary>
    /// Returns the lowest supported API key of this message, inclusive.
    /// </summary>
    public ApiVersions LowestSupportedVersion { get; init; }

    /// <summary>
    /// Returns the highest supported API key of this message, inclusive.
    /// </summary>
    public ApiVersions HighestSupportedVersion { get; init; }

    /// <summary>
    /// Returns a list of tagged fields which this software can't understand.
    /// </summary>
    public List<TaggedField>? UnknownTaggedFields { get; set; }

    protected Message()
    {
    }

    protected Message(BufferReader reader, ApiVersions version)
    {
    }

    /// <summary>
    /// Writes out this message to the given stream.
    /// </summary>
    internal abstract void Write(BufferWriter writer, ApiVersions version);

    /// <summary>
    /// Reads this message from the given BufferReader. This will overwrite all relevant fields with information from the byte buffer.
    /// </summary>
    internal abstract void Read(BufferReader reader, ApiVersions version);
}