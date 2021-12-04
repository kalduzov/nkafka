using System;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka
{
    public class KafkaResponseMessage : IDisposable
    {
        private const StatusCodes _DEFAULT_CODE = StatusCodes.None;

        private bool _disposed;
        private KafkaContent? _content;
        private StatusCodes _code;
        private ApiVersions _version;

        public ApiVersions Version
        {
            get => _version;
            set
            {
                CheckDisposed();
                _version = value;
            }
        }

        public StatusCodes Code
        {
            get => _code;
            set
            {
                CheckDisposed();
                _code = value;
            }
        }

        public KafkaContent Content
        {
            get { return _content ??= KafkaContent.Empty; }
            set
            {
                CheckDisposed();
                _content = value;
            }
        }

        public bool IsSuccessStatusCode => _code == _DEFAULT_CODE;

        public KafkaResponseMessage()
            : this(_DEFAULT_CODE)
        {
        }

        public KafkaResponseMessage(StatusCodes code)
        {
            _code = code;
            Version = ApiVersions.LastVersion;
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

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
        }
    }
}