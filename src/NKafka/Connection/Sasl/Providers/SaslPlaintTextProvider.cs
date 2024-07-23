// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2024 Aleksey Kalduzov. All rights reserved
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

using System.Text;

using NKafka.Config;

namespace NKafka.Connection.Sasl.Providers;

/// <summary>
/// https://datatracker.ietf.org/doc/html/rfc4616
/// </summary>
/// <remarks>
/// https://datatracker.ietf.org/doc/html/rfc4616
/// </remarks>
internal class SaslPlaintTextProvider(SaslSettings settings): ISaslProvider
{
    private const byte _NULL_VALUE = 0;

    private readonly byte[] _authcid = Encoding.UTF8.GetBytes(settings.UserName); //authentication identity
    private readonly byte[] _password = Encoding.UTF8.GetBytes(settings.Password); //password 
    private readonly byte[] _authzid = []; //authorization identity

    public string Mechanism => SaslSettings.MechanismAsString(SaslMechanism.Plain);

    public byte[] GetAuthData()
    {
        var buf = GC.AllocateArray<byte>(_authzid.Length + 1 + _password.Length + 1 + _authcid.Length);

        var offset = 0;

        Buffer.BlockCopy(_authzid, 0, buf, offset, _authzid.Length);
        offset += _authzid.Length;
        buf[offset++] = _NULL_VALUE;
        Buffer.BlockCopy(_authcid, 0, buf, offset, _authcid.Length);
        offset += _authcid.Length;
        buf[offset++] = _NULL_VALUE;
        Buffer.BlockCopy(_password, 0, buf, offset, _password.Length);

        return buf;
    }
}