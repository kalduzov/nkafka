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
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace NKafka.Protocol;

public abstract class ResponseMessage: Message
{
    private const ErrorCodes _DEFAULT_CODE = ErrorCodes.None;

    /// <summary>
    /// Response error code
    /// </summary>
    public ErrorCodes Code { get; } = _DEFAULT_CODE;

    /// <summary>
    /// Indicates that the response message was successful
    /// </summary>
    public bool IsSuccessStatusCode => Code == _DEFAULT_CODE;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request
    /// did not violate any quota
    /// </summary>
    public int ThrottleTimeMs { get; init; } = 0;

    protected ResponseMessage()
    {
    }

    protected ResponseMessage(BufferReader reader, ApiVersions version)
        : base(reader, version)
    {
    }
}