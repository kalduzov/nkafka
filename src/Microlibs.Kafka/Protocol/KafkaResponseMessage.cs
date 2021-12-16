﻿using System;
using System.IO.Pipelines;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka;

public abstract class KafkaResponseMessage : IDisposable
{
    private const StatusCodes _DEFAULT_CODE = StatusCodes.None;
    protected short _code;
    private KafkaContent? _content;

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

    public abstract void DeserializeFromStream(PipeReader reader);

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