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

using System.Net.Security;
using System.Security.Authentication;

namespace NKafka.Config;

/// <summary>
/// 
/// </summary>
public record SslSettings
{
    internal static readonly SslSettings None = new(false);

    internal bool IsSet { get; }

    /// <summary>
    /// The support SSL protocols 
    /// </summary>
    /// <remarks>TLS v1.3 by default</remarks>
    public SslProtocols Protocols { get; set; } = SslProtocols.Tls13;

    /// <summary>
    /// Whether to validate the revocation of the certificate.
    /// </summary>
    public bool CheckCertificateRevocation { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public RemoteCertificateValidationCallback? RemoteCertificateValidationCallback { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public bool TrustServerCertificate { get; set; }

    /// <summary>
    /// Location of a CA certificate for validate the server certificate
    /// </summary>
    public string? RootCertificate { get; set; }

    internal SslSettings(bool isSet)
    {
        IsSet = isSet;
    }

    public SslSettings()
        : this(true)
    {
    }

    /// <summary>
    /// Validates the settings and throws an exception if the settings are invalid or missing required ones
    /// </summary>
    internal void Validate()
    {
        if (this == None)
        {
            return;
        }
    }
}