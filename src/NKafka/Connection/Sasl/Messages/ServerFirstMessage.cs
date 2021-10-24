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

using System.Text.RegularExpressions;

namespace NKafka.Connection.Sasl.Messages;

internal class ServerFirstMessage: ScramMessage
{
    private static readonly Regex _messageFormat = new(
        $"{RESERVED}r=(?<nonce>{PRINTABLE}),s=(?<salt>{BASE64}),i=(?<iterations>[0-9]+){EXTENSIONS}",
        RegexOptions.Compiled);

    public string Nonce { get; set; }

    public byte[] Salt { get; set; }

    public int Iterations { get; set; }

    public ServerFirstMessage(Span<byte> messageBytes)
    {
        var message = ToMessage(messageBytes);
        var matcher = _messageFormat.Match(message);

        if (!matcher.Success)
        {
            throw new SaslException($"Invalid SCRAM server first message format: {message}");
        }

        if (!int.TryParse(matcher.Groups["iterations"].Value, out var iterations))
        {
            throw new SaslException("Invalid SCRAM server first message format: invalid iterations");
        }

        if (iterations <= 0)
        {
            throw new SaslException($"Invalid SCRAM server first message format: invalid iterations {iterations}");
        }

        Iterations = iterations;
        Salt = Convert.FromBase64String(matcher.Groups["salt"].Value);
        Nonce = matcher.Groups["nonce"].Value;
    }

    public ServerFirstMessage(string clientNonce, string serverNonce, byte[] salt, int iterations)
    {
        Nonce = $"{clientNonce}{serverNonce}";
        Salt = salt;
        Iterations = iterations;
    }

    public override string ToMessage()
    {
        return $"r={Nonce},s={Convert.ToBase64String(Salt)},i={Iterations}";
    }
}