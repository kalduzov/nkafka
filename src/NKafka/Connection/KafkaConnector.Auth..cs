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

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Connection.Sasl;
using NKafka.Connection.Sasl.Providers;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Resources;

namespace NKafka.Connection;

internal sealed partial class KafkaConnector
{
    private Task AuthenticateSaslSessionAsync(CancellationToken token)
    {
        return _saslSettings.Version switch
        {
            SaslVersion.SaslHandshakeV1 => AuthenticateSaslHandshakeV1Async(token),

            // ReSharper disable once NotResolvedInText
            _ => throw new ArgumentOutOfRangeException("Sasl.Version", ExceptionMessages.SaslVersionInvalid)
        };
    }

    private async Task AuthenticateSaslHandshakeV1Async(CancellationToken token)
    {
        using var authenticationSession = CreateSaslAuthenticationSession();
        _logger.StartSaslAuthenticationDebug(NodeId, authenticationSession.Mechanism);

        if (_saslSettings.Handshake)
        {
            var saslHandshakeRequest = new SaslHandshakeRequestMessage
            {
                Mechanism = authenticationSession.Mechanism
            };

            var handshakeResponse = await ((IKafkaConnector)this).SendAsync<SaslHandshakeRequestMessage, SaslHandshakeResponseMessage>(
                saslHandshakeRequest,
                true,
                token);

            if (handshakeResponse.Code != ErrorCodes.None)
            {
                throw new ProtocolKafkaException(handshakeResponse.Code);
            }

            if (handshakeResponse.Mechanisms.Count != 0
                && !handshakeResponse.Mechanisms.Contains(authenticationSession.Mechanism, StringComparer.Ordinal))
            {
                throw new ProtocolKafkaException(
                    ErrorCodes.UnsupportedSaslMechanism,
                    $"Broker does not advertise SASL mechanism {authenticationSession.Mechanism}.");
            }
        }

        await RunAuthenticateLoopAsync(authenticationSession, token);
    }

    private ISaslAuthenticationSession CreateSaslAuthenticationSession()
    {
        return _saslSettings.Mechanism switch
        {
            SaslMechanism.Plain => new SingleStageSaslAuthenticationSession(new SaslPlaintTextProvider(_saslSettings)),
            SaslMechanism.ScramSha256 => new ScramSaslAuthenticationSession(
                new ScramSaslClient(
                    ScramMechanism.ScramSha256,
                    new SaslSettingsAuthStore(_saslSettings),
                    _loggerFactory.CreateLogger<ScramSaslClient>())),
            SaslMechanism.ScramSha512 => new ScramSaslAuthenticationSession(
                new ScramSaslClient(
                    ScramMechanism.ScramSha512,
                    new SaslSettingsAuthStore(_saslSettings),
                    _loggerFactory.CreateLogger<ScramSaslClient>())),
            SaslMechanism.OAuthBearer => throw new NotSupportedException("OAUTHBEARER runtime authentication is not implemented."),
            _ => throw new ArgumentException(ExceptionMessages.SaslMechanismInvalid)
        };
    }

    private async Task RunAuthenticateLoopAsync(ISaslAuthenticationSession authenticationSession, CancellationToken token)
    {
        var authBytes = authenticationSession.CreateInitialRequest();

        while (true)
        {
            // Every mechanism uses the same authenticate exchange so that challenge-based
            // flows never bypass the connector's normal request/response lifecycle.
            var authenticateResponse = await ((IKafkaConnector)this).SendAsync<SaslAuthenticateRequestMessage, SaslAuthenticateResponseMessage>(
                new SaslAuthenticateRequestMessage
                {
                    AuthBytes = authBytes
                },
                true,
                token);

            if (authenticateResponse.Code != ErrorCodes.None)
            {
                throw new ProtocolKafkaException(authenticateResponse.Code);
            }

            if (!authenticationSession.TryContinue(authenticateResponse.AuthBytes, out authBytes))
            {
                return;
            }
        }
    }

    private interface ISaslAuthenticationSession: IDisposable
    {
        string Mechanism { get; }

        byte[] CreateInitialRequest();

        bool TryContinue(byte[] challenge, out byte[] nextRequest);
    }

    private sealed class SingleStageSaslAuthenticationSession(ISaslProvider provider): ISaslAuthenticationSession
    {
        public string Mechanism => provider.Mechanism;

        public byte[] CreateInitialRequest()
        {
            return provider.GetAuthData();
        }

        public bool TryContinue(byte[] challenge, out byte[] nextRequest)
        {
            nextRequest = [];

            return false;
        }

        public void Dispose()
        {
        }
    }

    private sealed class ScramSaslAuthenticationSession(ISaslClient saslClient): ISaslAuthenticationSession
    {
        public string Mechanism => saslClient.MechanismName;

        public byte[] CreateInitialRequest()
        {
            return saslClient.EvaluateChallenge([]).ToArray();
        }

        public bool TryContinue(byte[] challenge, out byte[] nextRequest)
        {
            var responseBytes = saslClient.EvaluateChallenge(challenge).ToArray();

            if (saslClient.IsComplete)
            {
                nextRequest = [];

                return false;
            }

            nextRequest = responseBytes;

            return true;
        }

        public void Dispose()
        {
            saslClient.Dispose();
        }
    }

    private sealed class SaslSettingsAuthStore(SaslSettings settings): ISaslAuthStore
    {
        public string GetUserName()
        {
            return settings.UserName;
        }

        public Dictionary<string, string> GetExtensions()
        {
            return [];
        }

        public Span<byte> GetPasswordAsBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes(settings.Password);
        }
    }
}
