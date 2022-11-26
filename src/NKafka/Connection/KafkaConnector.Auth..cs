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
using NKafka.Resources;

namespace NKafka.Connection;

internal sealed partial class KafkaConnector
{
    private Task AuthenticateProcessAsync(CancellationToken token)
    {
        return _saslSettings.Version switch
        {
            SaslVersion.SaslHandshakeV0 => AuthenticateProcessV0Async(token),
            SaslVersion.SaslHandshakeV1 => AuthenticateProcessV1Async(token),

            // ReSharper disable once NotResolvedInText
            _ => throw new ArgumentOutOfRangeException("Sasl.Version", ExceptionMessages.SaslVersionInvalid)
        };

        // var saslHandshakeRequest = new SaslHandshakeRequestMessage();
        // var response = await InternalSendAsync<SaslHandshakeResponseMessage, SaslHandshakeRequestMessage>(saslHandshakeRequest, true, token);
        //
        // if (response.Code == ErrorCodes.None)
        // {
        //     var authenticateRequest = new SaslAuthenticateRequestMessage
        //     {
        //     };
        //     var r = await InternalSendAsync<SaslAuthenticateResponseMessage, SaslAuthenticateRequestMessage>(authenticateRequest, true, token);
        // }
    }

    private Task AuthenticateProcessV1Async(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private Task AuthenticateProcessV0Async(CancellationToken token)
    {
        return _saslSettings.Mechanism switch
        {
            SaslMechanism.Plain => AuthenticateSaslPlainV0Async(token),
            SaslMechanism.ScarmSha256 => AuthenticateSaslScarmV0Async(token),
            SaslMechanism.ScarmSha512 => AuthenticateSaslScarmV0Async(token),
            SaslMechanism.OAuthBearer => AuthenticateSaslOAuthV0Async(token),
            SaslMechanism.Kerberos => AuthenticateSaslKerberosV0Async(token),

            _ => throw new ArgumentException(ExceptionMessages.SaslMechanismInvalid)
        };
    }

    private Task AuthenticateSaslKerberosV0Async(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private Task AuthenticateSaslOAuthV0Async(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private Task AuthenticateSaslPlainV0Async(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private Task AuthenticateSaslScarmV0Async(CancellationToken token)
    {
        throw new NotImplementedException();
    }

   
}