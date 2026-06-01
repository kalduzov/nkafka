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
using NKafka.Exceptions;

namespace NKafka.Config;

/// <summary>
/// Sasl settings
/// </summary>
public record SaslSettings
{
    internal static readonly SaslSettings None = new(false);

    internal bool IsSet { get; }

    /// <summary>
    /// Gets or sets the SASL mechanism used for authentication.
    /// </summary>
    /// <value>
    /// The SASL mechanism used for authentication.
    /// </value>
    public SaslMechanism Mechanism { get; set; }

    /// <summary>
    /// Gets or sets the version of the SASL protocol.
    /// </summary>
    /// <value>
    /// The version of the SASL protocol.
    /// </value>
    public SaslVersion Version { get; set; } = SaslVersion.SaslHandshakeV1;

    /// <summary>
    /// Gets or sets the OAuth bearer method.
    /// </summary>
    /// <value>
    /// The OAuth bearer method.
    /// </value>
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
    /// Whether to send the Kafka SASL handshake first if enabled
    /// (defaults to true). You should only set this to false if you're using
    /// a non-Kafka SASL proxy.
    /// </summary>
    public bool Handshake { get; set; } = true;

    private SaslSettings(bool isSet)
    {
        IsSet = isSet;
    }

    /// <summary>
    /// Represents the SASL settings.
    /// </summary>
    public SaslSettings()
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

        _ = Mechanism switch
        {
            SaslMechanism.Plain => SaslMechanism.Plain,
            SaslMechanism.ScramSha256 => SaslMechanism.ScramSha256,
            SaslMechanism.ScramSha512 => SaslMechanism.ScramSha512,
            SaslMechanism.OAuthBearer => throw new KafkaConfigException(
                nameof(Mechanism),
                Mechanism,
                "OAUTHBEARER is not supported by the runtime authentication path."),
            SaslMechanism.Kerberos => throw new KafkaConfigException(
                nameof(Mechanism),
                Mechanism,
                "Kerberos/GSSAPI is not supported by the runtime authentication path."),
            _ => throw new KafkaConfigException(nameof(Mechanism), Mechanism, ExceptionMessages.SaslMechanismInvalid)
        };

        if (Mechanism is SaslMechanism.Plain or SaslMechanism.ScramSha256 or SaslMechanism.ScramSha512)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                throw new KafkaConfigException(
                    nameof(Mechanism),
                    Mechanism,
                    $"{Mechanism} authentication requires both UserName and Password.");
            }
        }
    }

    /// <summary>
    /// Returns the specified SaslMechanism as its string representation.
    /// </summary>
    /// <param name="mechanism">The SaslMechanism to convert to string.</param>
    /// <returns>The string representation of the specified SaslMechanism.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified SaslMechanism is not a valid value.</exception>
    public static string MechanismAsString(SaslMechanism mechanism)
    {
        return mechanism switch
        {
            SaslMechanism.Plain => "PLAIN",
            SaslMechanism.ScramSha256 => "SCRAM-SHA-256",
            SaslMechanism.ScramSha512 => "SCRAM-SHA-512",
            SaslMechanism.OAuthBearer => "OAUTHBEARER",
            SaslMechanism.Kerberos => "GSSAPI",
            _ => throw new ArgumentException(ExceptionMessages.SaslMechanismInvalid)
        };
    }
}
