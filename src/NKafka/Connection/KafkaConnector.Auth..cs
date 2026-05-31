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

using NKafka.Config;
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
        ISaslProvider saslProvider = _saslSettings.Mechanism switch
        {
            SaslMechanism.Plain => new SaslPlaintTextProvider(_saslSettings),
            SaslMechanism.OAuthBearer => new SaslOAuthBearerProvider(_saslSettings),

            _ => throw new ArgumentException(ExceptionMessages.SaslMechanismInvalid)
        };

        if (_saslSettings.Handshake)
        {
            var saslHandshakeRequest = new SaslHandshakeRequestMessage
            {
                Mechanism = saslProvider.Mechanism
            };

            var handshakeResponse = await ((IKafkaConnector)this).SendAsync<SaslHandshakeRequestMessage, SaslHandshakeResponseMessage>(
                saslHandshakeRequest,
                true,
                token);

            if (handshakeResponse.Code != ErrorCodes.None)
            {
                throw new ProtocolKafkaException(handshakeResponse.Code);
            }
        }

        // The final authenticate request is sent only after the broker accepts the
        // selected mechanism so that the session never mixes credentials across auth flows.
        var authenticateRequest = new SaslAuthenticateRequestMessage
        {
            AuthBytes = saslProvider.GetAuthData()
        };

        var authenticateResponse = await ((IKafkaConnector)this).SendAsync<SaslAuthenticateRequestMessage, SaslAuthenticateResponseMessage>(
            authenticateRequest,
            true,
            token);

        if (authenticateResponse.Code != ErrorCodes.None)
        {
            throw new ProtocolKafkaException(authenticateResponse.Code);
        }
    }

    // private async Task AuthenticateSaslScramV0Async(SaslMechanism saslMechanism, CancellationToken token)
    // {
    //     // handshake step
    //     var saslHandshakeRequest = new SaslHandshakeRequestMessage
    //     {
    //         Mechanism = SaslSettings.MechanismAsString(saslMechanism)
    //     };
    //
    //     var handshakeResponse = await ((IKafkaConnector)this).SendAsync<SaslHandshakeRequestMessage, SaslHandshakeResponseMessage>(
    //         saslHandshakeRequest,
    //         true,
    //         token);
    //
    //     if (handshakeResponse.Code != ErrorCodes.None)
    //     {
    //         throw new ProtocolKafkaException(handshakeResponse.Code, $"Не удалось начать процедуру аутентификации по механизму {saslMechanism}");
    //     }
    //
    //     // initial step
    //     var authenticateRequest = new SaslAuthenticateRequestMessage
    //     {
    //         AuthBytes = CreateSaslToken(Array.Empty<byte>(), true)
    //     };
    //     var authenticateResponse = await ((IKafkaConnector)this).SendAsync<SaslAuthenticateRequestMessage, SaslAuthenticateResponseMessage>(
    //         authenticateRequest,
    //         true,
    //         token);
    // }
    //
    // private byte[] CreateSaslToken(byte[] empty, bool b)
    // {
    //     return new byte[]
    //     {
    //     };
    // }
}
