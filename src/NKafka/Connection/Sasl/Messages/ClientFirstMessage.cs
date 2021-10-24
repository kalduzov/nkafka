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

internal class ClientFirstMessage: ScramMessage
{
    private static readonly Regex _messageFormat = new(
        $"n,(a=(?<authzid>{SASL_NAME}))?,{RESERVED}n=(?<saslname>{SASL_NAME}),r=(?<nonce>{PRINTABLE})(?<extensions>{EXTENSIONS})",
        RegexOptions.Compiled);

    public ScramExtensions Extensions { get; }

    public string AuthorizationId { get; }

    public string SaslName { get; }

    public string Nonce { get; }

    public string Gs2Header => $"n,{(string.IsNullOrEmpty(AuthorizationId) ? "" : $"a={AuthorizationId}")},";

    public ClientFirstMessage(Span<byte> messageBytes)
    {
        var message = ToMessage(messageBytes);
        var matcher = _messageFormat.Match(message);

        if (!matcher.Success)
        {
            throw new SaslException("Invalid SCRAM client first message format: " + message);
        }

        AuthorizationId = matcher.Groups["authzid"].Value;
        SaslName = matcher.Groups["saslname"].Value;
        Nonce = matcher.Groups["nonce"].Value;
        var extString = matcher.Groups["extensions"].Value;

        Extensions = extString.StartsWith(',') ? new ScramExtensions(extString[1..]) : new ScramExtensions();
    }

    public ClientFirstMessage(string saslName, string nonce, Dictionary<string, string> extensions)
    {
        SaslName = saslName;
        Nonce = nonce;
        AuthorizationId = string.Empty;
        Extensions = new ScramExtensions(extensions);
    }

    public override string ToMessage()
    {
        return $"{Gs2Header}{ClientFirstMessageBare()}";
    }

    internal string ClientFirstMessageBare()
    {
        var extensionStr = Utils.MkString(Extensions.Map, string.Empty, string.Empty, "=", ",");

        return string.IsNullOrWhiteSpace(extensionStr) ? $"n={SaslName},r={Nonce}" : $"n={SaslName},r={Nonce},{extensionStr}";
    }
}