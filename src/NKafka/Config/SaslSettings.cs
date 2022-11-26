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

using NKafka.Resources;

namespace NKafka.Config;

/// <summary>
/// Sasl settings
/// </summary>
public class SaslSettings
{
    internal static readonly SaslSettings None = new SaslSettings(false);
    
    internal bool IsSet { get; }

    /// <summary>
    /// 
    /// </summary>
    public SaslMechanism Mechanism { get; set; }

    /// <summary>
    /// Version is the SASL Protocol Version to use 
    /// </summary>
    /// <remarks>
    /// Kafka > 1.x should use V1, except on Azure EventHub which use V0
    /// </remarks>
    public SaslVersion Version { get; set; } = SaslVersion.SaslHandshakeV1;

    /// <summary>
    /// 
    /// </summary>
    public OAuthBearerMethod OAuthBearerMethod { get; set; } = OAuthBearerMethod.Default;

    /// <summary>
    /// SASL username for use with the PLAIN and SASL-SCRAM-.. mechanism
    /// <p> <b>default:</b> ""</p>
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// SASL password for use with the PLAIN and SASL-SCRAM-.. mechanism
    /// <p> <b>default:</b> ""</p>
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Whether or not to send the Kafka SASL handshake first if enabled
    /// (defaults to true). You should only set this to false if you're using
    /// a non-Kafka SASL proxy.
    /// </summary>
    public bool Handshake { get; set; } = true;

    internal SaslSettings(bool isSet)
    {
        IsSet = isSet;
    }

    public SaslSettings()
        : this(true)
    {
    }

    /// <summary>
    /// Validates the settings and throws an exception if the settings are invalid or missing required ones
    /// </summary>
    internal void Validate()
    {
        var _ = Mechanism switch
        {
            SaslMechanism.Plain => SaslMechanism.Plain,
            SaslMechanism.ScarmSha256 => SaslMechanism.ScarmSha256,
            SaslMechanism.ScarmSha512 => SaslMechanism.ScarmSha512,
            SaslMechanism.OAuthBearer => SaslMechanism.OAuthBearer,
            SaslMechanism.Kerberos => SaslMechanism.Kerberos,
            _ => throw new ArgumentException(ExceptionMessages.SaslMechanismInvalid)
        };
    }
}