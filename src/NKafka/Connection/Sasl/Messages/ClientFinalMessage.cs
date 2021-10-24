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

internal class ClientFinalMessage: ScramMessage
{
    private static readonly Regex _messageFormat = new(
        $"c=(?<channel>{BASE64}),r=(?<nonce>{PRINTABLE}){EXTENSIONS},p=(?<proof>{BASE64})",
        RegexOptions.Compiled);

    public byte[] ChannelBinding { get; set; }

    public string Nonce { get; set; }

    public byte[] Proof { get; set; }

    public ClientFinalMessage(byte[] messageBytes)
    {
        var message = ToMessage(messageBytes);
        var matcher = _messageFormat.Match(message);

        if (!matcher.Success)
        {
            throw new SaslException("Invalid SCRAM client final message format: " + message);
        }

        ChannelBinding = Convert.FromBase64String(matcher.Groups["channel"].Value);
        Nonce = matcher.Groups["nonce"].Value;
        Proof = Convert.FromBase64String(matcher.Groups["proof"].Value);
    }

    public ClientFinalMessage(ReadOnlySpan<byte> channelBinding, string nonce)
    {
        ChannelBinding = channelBinding.ToArray();
        Nonce = nonce;
    }

    public string ClientFinalMessageWithoutProof()
    {
        return $"c={Convert.ToBase64String(ChannelBinding)},r={Nonce}";
    }

    public override string ToMessage()
    {
        return $"{ClientFinalMessageWithoutProof()},p={Convert.ToBase64String(Proof)}";
    }
}