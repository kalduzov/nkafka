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
/// Represents the SSL settings for establishing a secure connection.
/// </summary>
public record SslSettings
{
    /// <summary
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
    /// Gets or sets the callback used to validate the remote certificate in an SSL/TLS connection.
    /// </summary>
    /// <remarks>
    /// The RemoteCertificateValidationCallback is called during the SSL/TLS handshake process to
    /// validate the remote server's certificate. It is responsible for determining whether the
    /// certificate is trusted or not. The callback should return true if the certificate is trusted,
    /// and false otherwise.
    /// The signature of the RemoteCertificateValidationCallback delegate is as follows:
    /// <code>
    /// bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate,
    /// X509Chain chain, SslPolicyErrors sslPolicyErrors)
    /// </code>
    /// The <paramref name="sender"/> parameter is the object that triggered the validation request,
    /// typically a TcpClient or HttpClient object.
    /// The <paramref name="certificate"/> parameter is the remote server's certificate.
    /// The <paramref name="chain"/> parameter is the chain of certificate authorities that vouch for
    /// the authenticity of the certificate.
    /// The <paramref name="sslPolicyErrors"/> parameter indicates if any SSL/TLS policy errors
    /// were encountered during the validation. This can include errors such as the certificate not
    /// being trusted, or the common name not matching the host name.
    /// </remarks>
    public RemoteCertificateValidationCallback? RemoteCertificateValidationCallback { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the SSL/TLS certificate presented by the server should be trusted.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, the client will trust any SSL/TLS certificate presented by the server, regardless of its validity.
    /// When set to <c>false</c> (the default), the client will validate the SSL/TLS certificate using the certificate chain and trust store of the operating system to ensure its validity
    /// .
    /// Trusting an invalid or self-signed certificate may introduce security risks as the server may not be the expected entity.
    /// </remarks>
    public bool TrustServerCertificate { get; set; }

    /// <summary>
    /// Location of a CA certificate for validate the server certificate
    /// </summary>
    public string? RootCertificate { get; set; }

    private SslSettings(bool isSet)
    {
        IsSet = isSet;
    }

    /// <summary>
    /// Represents the SSL settings for a connection.
    /// </summary>
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