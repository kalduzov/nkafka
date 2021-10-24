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

using NKafka.Exceptions;

namespace NKafka.Protocol;

public interface IResponseMessage: IMessage
{
    private const ErrorCodes _DEFAULT_CODE = ErrorCodes.None;

    /// <summary>
    /// Response error code
    /// </summary>
    public ErrorCodes Code => _DEFAULT_CODE;

    /// <summary>
    /// Indicates that the response message was successful
    /// </summary>
    public bool IsSuccessStatusCode => Code == _DEFAULT_CODE;

    /// <summary>
    /// The duration in milliseconds for which the request was throttled due to a quota violation, or zero if the request
    /// did not violate any quota
    /// </summary>
    public int ThrottleTimeMs { get; set; }

    /// <summary>
    /// Throws an exception if the given response contains an error
    /// </summary>
    public void ThrowIfError()
    {
        if (!IsSuccessStatusCode)
        {
            throw new ProtocolKafkaException(Code);
        }
    }

    /// <summary>
    /// Returns whether or not client should throttle upon receiving a response of the specified version with a non-zero
    /// throttle time. Client-side throttling is needed when communicating with a newer version of broker which, on
    /// quota violation, sends out responses before throttling.
    /// </summary>
    public bool ShouldClientThrottle(ApiVersion version)
    {
        return false;
    }
}