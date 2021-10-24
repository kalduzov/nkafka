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
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

using NKafka.Connection.Sasl.Messages;

namespace NKafka.Connection.Sasl;

internal class ScramSaslClient: ISaslClient
{
    private readonly ScramFormatter _formatter;
    private readonly ILogger<ScramSaslClient> _logger;
    private readonly ScramMechanism _mechanism;
    private readonly ISaslAuthStore _saslAuthStore;
    private ClientFinalMessage? _clientFinalMessage;
    private ClientFirstMessage? _clientFirstMessage;
    private string? _clientNonce;
    private State _currentState;
    private byte[]? _saltedPassword;
    private ServerFirstMessage? _serverFirstMessage;

    private State CurrentState
    {
        get => _currentState;
        set
        {
            _logger.SaslChangeClientState(_mechanism, value);
            _currentState = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mechanism"></param>
    /// <param name="saslAuthStore"></param>
    /// <param name="logger"></param>
    public ScramSaslClient(ScramMechanism mechanism, ISaslAuthStore saslAuthStore, ILogger<ScramSaslClient> logger)
    {
        _mechanism = mechanism;
        _saslAuthStore = saslAuthStore;
        _logger = logger;
        _formatter = new ScramFormatter(mechanism);
        CurrentState = State.SendClientFirstMessage;
    }

    /// <summary>
    /// Returns the IANA-registered mechanism name of this SASL client.
    /// (e.g. "CRAM-MD5", "GSSAPI").
    /// </summary>
    /// <returns>A non-null string representing the IANA-registered mechanism name.</returns>
    public string MechanismName => _mechanism.GetName();

    /// <summary>
    /// Determines whether this mechanism has an optional initial response.
    /// If true, caller should call <see cref="ISaslClient.EvaluateChallenge"/> with an
    /// empty array to get the initial response.
    /// </summary>
    /// <returns>true if this mechanism has an initial response.</returns>
    public bool HasInitialResponse => true;

    /// <summary>
    /// Determines whether the authentication exchange has completed.
    /// This method may be called at any time, but typically, it
    /// will not be called until the caller has received indication
    /// from the server
    /// (in a protocol-specific manner) that the exchange has completed.
    /// </summary>
    /// <returns>true if the authentication exchange has completed; false otherwise.</returns>
    public bool IsComplete => CurrentState == State.Complete;

    /// <summary>
    /// Evaluates the challenge data and generates a response.
    /// If a challenge is received from the server during the authentication
    /// process, this method is called to prepare an appropriate next
    /// response to submit to the server.
    /// </summary>
    /// <param name="challenge">The non-null challenge sent from the server.
    /// The challenge array may have zero length.
    /// </param>
    /// <returns></returns>
    /// <exception cref="SaslException">If an error occurred while processing
    /// the challenge or generating a response.
    /// </exception>
    public Span<byte> EvaluateChallenge(Span<byte> challenge)
    {
        try
        {
            switch (CurrentState)
            {
                case State.SendClientFirstMessage:
                    {
                        if (!(challenge is { }))
                        {
                            throw new SaslException("Expected empty challenge");
                        }

                        _clientNonce = _formatter.SecureRandomString;
                        var saslName = ScramFormatter.SaslName(_saslAuthStore.GetUserName());
                        var extensions = _saslAuthStore.GetExtensions();
                        _clientFirstMessage = new ClientFirstMessage(saslName, _clientNonce, extensions);

                        CurrentState = State.ReceiveServerFirstMessage;

                        return _clientFirstMessage.ToBytes();
                    }
                case State.ReceiveServerFirstMessage:
                    {
                        _serverFirstMessage = new ServerFirstMessage(challenge);

                        if (!_serverFirstMessage.Nonce.StartsWith(_clientNonce!))
                        {
                            throw new SaslException("Invalid server nonce: does not start with client nonce");
                        }

                        if (_serverFirstMessage.Iterations < _mechanism.MinIterations())
                        {
                            throw new SaslException(
                                $"Requested iterations {_serverFirstMessage.Iterations} is less than the minimum {_mechanism.MinIterations()} for {_mechanism}");
                        }

                        var passwordBytes = _saslAuthStore.GetPasswordAsBytes();
                        var hashAlgorithmName = _mechanism.GetHashAlgorithmName();

                        try
                        {
                            _saltedPassword = Rfc2898DeriveBytes.Pbkdf2(
                                passwordBytes,
                                _serverFirstMessage.Salt,
                                _serverFirstMessage.Iterations,
                                hashAlgorithmName,
                                hashAlgorithmName == HashAlgorithmName.SHA256 ? 16 : 32);
                            _clientFinalMessage = new ClientFinalMessage("n,,"u8, _serverFirstMessage.Nonce);
                            var clientProof = _formatter.ClientProof(_saltedPassword, _clientFirstMessage!, _serverFirstMessage, _clientFinalMessage);
                            _clientFinalMessage.Proof = clientProof;
                        }
                        catch (Exception exc)
                        {
                            throw new SaslException("Client final message could not be created", exc);
                        }

                        CurrentState = State.ReceiveServerFinalMessage;

                        return _clientFinalMessage.ToBytes();
                    }
                case State.ReceiveServerFinalMessage:
                    {
                        var serverFinalMessage = new ServerFinalMessage(challenge);

                        if (!string.IsNullOrWhiteSpace(serverFinalMessage.Error))
                        {
                            throw new SaslException($"Sasl authentication using {_mechanism} failed with error {serverFinalMessage.Error}");
                        }

                        try
                        {
                            var serverKey = _formatter.ServerKey(_saltedPassword!);
                            var serverSignature = _formatter.ServerSignature(serverKey, _clientFirstMessage!, _serverFirstMessage!, _clientFinalMessage!);

                            if (!serverSignature.SequenceEqual(serverFinalMessage.ServerSignature))
                            {
                                throw new SaslException("Invalid server signature in server final message");
                            }
                        }
                        catch (Exception e) when (e is not SaslException)
                        {
                            throw new SaslException("Sasl server signature verification failed", e);
                        }

                        CurrentState = State.Complete;

                        return null;
                    }
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected challenge in Sasl client state {CurrentState}");
            }
        }
        catch (SaslException)
        {
            CurrentState = State.Failed;

            throw;
        }
    }

    /// <summary>
    /// Unwraps a byte array received from the server.
    /// This method can be called only after the authentication exchange has
    /// completed (i.e., when <see cref="ISaslClient.IsComplete"/> returns true) and only if
    /// the authentication exchange has negotiated integrity and/or privacy
    /// as the quality of protection; otherwise, an
    /// <see cref="Exception"/> is thrown.
    /// 
    /// <br/><br/>
    /// <b><paramref name="incoming"/></b> is the contents of the SASL buffer as defined in RFC 2222
    /// without the leading four octet field that represents the length.
    /// <b><paramref name="offset"/></b> and <b><paramref name="len"/></b> specify the portion of <b><paramref name="incoming"/></b>
    /// to use.
    /// 
    /// </summary>
    /// <param name="incoming">A non-null byte array containing the encoded bytes from the server.</param>
    /// <param name="offset">The starting position at <b><paramref name="incoming"/></b> of the bytes to use.</param>
    /// <param name="len">The number of bytes from <b><paramref name="incoming"/></b> to use.</param>
    /// <returns>A non-null byte array containing the decoded bytes.</returns>
    /// <exception cref="SaslException">if <b><paramref name="incoming"/></b> cannot be successfully unwrapped.</exception>
    /// <exception cref="Exception"> if the authentication exchange has not completed, or if the negotiated quality of protection
    /// has neither integrity nor privacy.
    /// </exception>
    public Span<byte> UnWrap(Span<byte> incoming, int offset, int len)
    {
        if (!IsComplete)
        {
            throw new Exception("Authentication exchange has not completed");
        }

        var buf = GC.AllocateUninitializedArray<byte>(len);
        incoming.Slice(offset, len).CopyTo(buf);

        return buf;
    }

    /// <summary>
    /// Wraps a byte array to be sent to the server.
    /// This method can be called only after the authentication exchange has
    /// completed (i.e., when <see cref="ISaslClient.IsComplete"/> returns true) and only if
    /// the authentication exchange has negotiated integrity and/or privacy
    /// as the quality of protection; otherwise, an <see cref="Exception"/> is thrown.
    /// 
    /// <br/><br/>
    /// The result of this method will make up the contents of the SASL buffer
    /// as defined in RFC 2222 without the leading four octet field that
    /// represents the length.
    /// <b><paramref name="offset"/></b> and <b><paramref name="len"/></b> specify the portion of <b><paramref name="outgoing"/></b>
    /// to use.
    /// 
    /// </summary>
    /// <param name="outgoing">A non-null byte array containing the bytes to encode.</param>
    /// <param name="offset">The starting position at <b><paramref name="outgoing"/></b> of the bytes to use.</param>
    /// <param name="len">The number of bytes from <b><paramref name="outgoing"/></b> to use.</param>
    /// <returns>A non-null byte array containing the encoded bytes.</returns>
    /// <exception cref="SaslException">if <b><paramref name="outgoing"/></b> cannot be successfully wrapped.</exception>
    /// <exception cref="Exception"> if the authentication exchange has not completed, or if the negotiated quality of protection
    /// has neither integrity nor privacy.
    /// </exception>
    public Span<byte> Wrap(Span<byte> outgoing, int offset, int len)
    {
        if (!IsComplete)
        {
            throw new Exception("Authentication exchange has not completed");
        }

        var buf = GC.AllocateUninitializedArray<byte>(len);
        outgoing.Slice(offset, len).CopyTo(buf);

        return buf;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _formatter.Dispose();
    }

    internal enum State
    {
        SendClientFirstMessage,
        ReceiveServerFirstMessage,
        ReceiveServerFinalMessage,
        Complete,
        Failed
    }
}

internal interface ISaslAuthStore
{
    string GetUserName();

    Dictionary<string, string> GetExtensions();

    Span<byte> GetPasswordAsBytes();
}