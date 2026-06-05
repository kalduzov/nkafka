using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;

namespace NKafka.IntegrationTests.E2E;

internal static class KafkaE2EClusterFactory
{
    private const string ZkTopologyMode = "zk";
    private const string KraftTopologyMode = "kraft";

    private const string PlaintextSecurityProfile = "plaintext";
    private const string SslSecurityProfile = "ssl";
    private const string SaslPlainSecurityProfile = "sasl-plain";
    private const string SaslScramSha256SecurityProfile = "sasl-scram256";
    private const string SaslScramSha512SecurityProfile = "sasl-scram512";
    private const string SaslOAuthBearerSecurityProfile = "sasl-oauthbearer";
    private const string SaslSslPlainSecurityProfile = "sasl-ssl-plain";
    private const string SaslSslScramSha256SecurityProfile = "sasl-ssl-scram256";
    private const string SaslSslScramSha512SecurityProfile = "sasl-ssl-scram512";
    private const string SaslSslOAuthBearerSecurityProfile = "sasl-ssl-oauthbearer";

    internal static bool IsE2EEnabled => ReadBooleanEnvironment("NKAFKA_E2E_ENABLED", false);

    internal static string CurrentTopologyMode => ReadTopologyMode();

    internal static string CurrentSecurityProfile => ReadSecurityProfile();

    internal static string CurrentKafkaVersion => Environment.GetEnvironmentVariable("NKAFKA_E2E_KAFKA_VERSION") ?? string.Empty;

    internal static bool IsZkPlaintextProfileEnabled => IsSelected(ZkTopologyMode, PlaintextSecurityProfile);

    internal static bool IsKraftPlaintextProfileEnabled => IsSelected(KraftTopologyMode, PlaintextSecurityProfile);

    internal static bool IsKraftSaslPlainProfileEnabled => IsSelected(KraftTopologyMode, SaslPlainSecurityProfile);

    internal static bool IsKraftSaslScramSha256ProfileEnabled => IsSelected(KraftTopologyMode, SaslScramSha256SecurityProfile);

    internal static bool IsKraftSaslScramSha512ProfileEnabled => IsSelected(KraftTopologyMode, SaslScramSha512SecurityProfile);

    internal static bool IsKraftSaslOAuthBearerProfileEnabled => IsSelected(KraftTopologyMode, SaslOAuthBearerSecurityProfile);

    internal static Task<IKafkaCluster> CreateClusterAsync(ILoggerFactory? loggerFactory = null)
    {
        var clusterConfig = BuildClusterConfigFromEnvironment();

        return clusterConfig.CreateCluster(loggerFactory ?? NullLoggerFactory.Instance);
    }

    private static ClusterConfig BuildClusterConfigFromEnvironment()
    {
        EnsureE2EInputsAreValid();

        var securityProtocol = ReadSecurityProtocol();
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = [ReadRequiredEnvironment("NKAFKA_E2E_BOOTSTRAP_SERVERS")],
            SecurityProtocol = securityProtocol
        };

        if (securityProtocol is SecurityProtocols.Ssl or SecurityProtocols.SaslSsl)
        {
            // E2E environments often use self-signed certificates, so certificate trust
            // must stay configurable without changing the test bodies themselves.
            clusterConfig.Ssl = new SslSettings
            {
                TrustServerCertificate = ReadBooleanEnvironment("NKAFKA_E2E_TRUST_SERVER_CERTIFICATE", true)
            };
        }

        if (securityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
        {
            // Security-enabled E2E profiles must carry the full auth contract explicitly
            // so the selected profile does not silently degrade to placeholder defaults.
            clusterConfig.Sasl = new SaslSettings
            {
                Mechanism = ReadSaslMechanism(),
                UserName = ReadRequiredEnvironment("NKAFKA_E2E_SASL_USERNAME"),
                Password = ReadRequiredEnvironment("NKAFKA_E2E_SASL_PASSWORD")
            };
        }

        return clusterConfig;
    }

    private static void EnsureE2EInputsAreValid()
    {
        if (!IsE2EEnabled)
        {
            throw new InvalidOperationException("Kafka E2E cluster creation requires NKAFKA_E2E_ENABLED=true.");
        }

        _ = ReadTopologyMode();
        _ = ReadSecurityProfile();
        _ = ReadRequiredEnvironment("NKAFKA_E2E_KAFKA_VERSION");
        _ = ReadRequiredEnvironment("NKAFKA_E2E_BOOTSTRAP_SERVERS");

        var securityProtocol = ReadSecurityProtocol();
        if (securityProtocol is SecurityProtocols.PlainText or SecurityProtocols.Ssl)
        {
            return;
        }

        if (securityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
        {
            _ = ReadSaslMechanism();
            _ = ReadRequiredEnvironment("NKAFKA_E2E_SASL_USERNAME");
            _ = ReadRequiredEnvironment("NKAFKA_E2E_SASL_PASSWORD");
        }
    }

    private static bool IsSelected(string topologyMode, string securityProfile)
        => IsE2EEnabled
           && string.Equals(CurrentTopologyMode, topologyMode, StringComparison.OrdinalIgnoreCase)
           && string.Equals(CurrentSecurityProfile, securityProfile, StringComparison.OrdinalIgnoreCase);

    private static string ReadTopologyMode()
    {
        var explicitValue = Environment.GetEnvironmentVariable("NKAFKA_E2E_TOPOLOGY_MODE");
        if (!string.IsNullOrWhiteSpace(explicitValue))
        {
            return explicitValue;
        }

        return TryMapLegacyProfile(out var topologyMode, out _)
            ? topologyMode
            : ReadRequiredEnvironment("NKAFKA_E2E_TOPOLOGY_MODE");
    }

    private static string ReadSecurityProfile()
    {
        var explicitValue = Environment.GetEnvironmentVariable("NKAFKA_E2E_SECURITY_PROFILE");
        if (!string.IsNullOrWhiteSpace(explicitValue))
        {
            return explicitValue;
        }

        return TryMapLegacyProfile(out _, out var securityProfile)
            ? securityProfile
            : ReadRequiredEnvironment("NKAFKA_E2E_SECURITY_PROFILE");
    }

    private static SecurityProtocols ReadSecurityProtocol()
    {
        var explicitValue = Environment.GetEnvironmentVariable("NKAFKA_E2E_SECURITY_PROTOCOL");
        if (!string.IsNullOrWhiteSpace(explicitValue))
        {
            return Enum.TryParse(explicitValue, ignoreCase: true, out SecurityProtocols protocol)
                ? protocol
                : throw new InvalidOperationException(
                    "Unsupported NKAFKA_E2E_SECURITY_PROTOCOL value. Expected PlainText, Ssl, SaslPlaintext or SaslSsl.");
        }

        return ReadSecurityProfile() switch
        {
            PlaintextSecurityProfile => SecurityProtocols.PlainText,
            SslSecurityProfile => SecurityProtocols.Ssl,
            SaslPlainSecurityProfile => SecurityProtocols.SaslPlaintext,
            SaslScramSha256SecurityProfile => SecurityProtocols.SaslPlaintext,
            SaslScramSha512SecurityProfile => SecurityProtocols.SaslPlaintext,
            SaslOAuthBearerSecurityProfile => SecurityProtocols.SaslPlaintext,
            SaslSslPlainSecurityProfile => SecurityProtocols.SaslSsl,
            SaslSslScramSha256SecurityProfile => SecurityProtocols.SaslSsl,
            SaslSslScramSha512SecurityProfile => SecurityProtocols.SaslSsl,
            SaslSslOAuthBearerSecurityProfile => SecurityProtocols.SaslSsl,
            _ => throw new InvalidOperationException(
                "Unsupported NKAFKA_E2E_SECURITY_PROFILE value.")
        };
    }

    private static SaslMechanism ReadSaslMechanism()
    {
        var explicitValue = Environment.GetEnvironmentVariable("NKAFKA_E2E_SASL_MECHANISM");
        if (!string.IsNullOrWhiteSpace(explicitValue))
        {
            return Enum.TryParse(explicitValue, ignoreCase: true, out SaslMechanism mechanism)
                ? mechanism
                : throw new InvalidOperationException(
                    "Unsupported NKAFKA_E2E_SASL_MECHANISM value. Expected Plain, ScramSha256, ScramSha512 or OAuthBearer.");
        }

        return ReadSecurityProfile() switch
        {
            SaslPlainSecurityProfile => SaslMechanism.Plain,
            SaslScramSha256SecurityProfile => SaslMechanism.ScramSha256,
            SaslScramSha512SecurityProfile => SaslMechanism.ScramSha512,
            SaslOAuthBearerSecurityProfile => SaslMechanism.OAuthBearer,
            SaslSslPlainSecurityProfile => SaslMechanism.Plain,
            SaslSslScramSha256SecurityProfile => SaslMechanism.ScramSha256,
            SaslSslScramSha512SecurityProfile => SaslMechanism.ScramSha512,
            SaslSslOAuthBearerSecurityProfile => SaslMechanism.OAuthBearer,
            _ => throw new InvalidOperationException(
                "The selected NKAFKA_E2E_SECURITY_PROFILE does not imply a SASL mechanism.")
        };
    }

    private static bool TryMapLegacyProfile(out string topologyMode, out string securityProfile)
    {
        var legacyProfile = Environment.GetEnvironmentVariable("NKAFKA_E2E_PROFILE");
        if (string.IsNullOrWhiteSpace(legacyProfile))
        {
            topologyMode = string.Empty;
            securityProfile = string.Empty;
            return false;
        }

        switch (legacyProfile.ToLowerInvariant())
        {
            case "zk-plaintext":
                topologyMode = ZkTopologyMode;
                securityProfile = PlaintextSecurityProfile;
                return true;
            case "kraft-plaintext":
                topologyMode = KraftTopologyMode;
                securityProfile = PlaintextSecurityProfile;
                return true;
            case "kraft-sasl-plain":
                topologyMode = KraftTopologyMode;
                securityProfile = SaslPlainSecurityProfile;
                return true;
            case "kraft-sasl-scram256":
                topologyMode = KraftTopologyMode;
                securityProfile = SaslScramSha256SecurityProfile;
                return true;
            case "kraft-sasl-scram512":
                topologyMode = KraftTopologyMode;
                securityProfile = SaslScramSha512SecurityProfile;
                return true;
            case "kraft-sasl-oauthbearer":
                topologyMode = KraftTopologyMode;
                securityProfile = SaslOAuthBearerSecurityProfile;
                return true;
            default:
                topologyMode = string.Empty;
                securityProfile = string.Empty;
                return false;
        }
    }

    private static bool ReadBooleanEnvironment(string variableName, bool defaultValue)
        => bool.TryParse(Environment.GetEnvironmentVariable(variableName), out var value)
            ? value
            : defaultValue;

    private static string ReadRequiredEnvironment(string variableName)
        => Environment.GetEnvironmentVariable(variableName)
           ?? throw new InvalidOperationException(
               $"Kafka E2E profile requires the environment variable '{variableName}'.");
}
