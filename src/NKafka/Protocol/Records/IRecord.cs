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
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace NKafka.Protocol.Records;

internal interface IRecord
{
    /// <summary>
    /// Full record length
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    ///
    /// </summary>
    public sbyte Attributes { get; set; }

    public long TimestampDelta { get; set; }

    /// <summary>
    /// The offset of this record in the log
    /// </summary>
    public long OffsetDelta { get; set; }

    public long KeyLength => Key?.LongLength ?? 0;

    /// <summary>
    /// The record's key
    /// </summary>
    public byte[]? Key { get; set; }

    /// <summary>
    /// The record's value
    /// </summary>
    public byte[]? Value { get; set; }

    /// <summary>
    /// Introduced in 0.11.0.0 for KIP-82, Kafka now supports application level record level headers.
    /// The Producer and Consumer APIS have been accordingly updated to write and read these headers.
    /// </summary>
    public Headers Headers { get; set; }
}