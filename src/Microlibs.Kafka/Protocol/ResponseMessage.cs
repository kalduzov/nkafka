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
using System;

namespace Microlibs.Kafka.Protocol;

public record KafkaResponseMessage : IDisposable
{
    private const StatusCodes _DEFAULT_CODE = StatusCodes.None;
    protected short _code;
    private RequestBody? _content;

    private bool _disposed;
    private ApiVersions _version;

    public KafkaResponseMessage()
        : this(_DEFAULT_CODE)
    {
    }

    public KafkaResponseMessage(StatusCodes code)
    {
        _code = (short)code;
        Version = ApiVersions.LastVersion;
    }

    public ApiVersions Version
    {
        get => _version;
        set
        {
            CheckDisposed();
            _version = value;
        }
    }

    public StatusCodes Code => (StatusCodes)_code;

    public bool IsSuccessStatusCode => Code == _DEFAULT_CODE;

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().ToString());
        }
    }

    #region IDisposable Members

    protected virtual void Dispose(bool disposing)
    {
        // The reason for this type to implement IDisposable is that it contains instances of types that implement
        // IDisposable (content).
        if (disposing && !_disposed)
        {
            _disposed = true;

            if (_content != null)
            {
                _content.Dispose();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}