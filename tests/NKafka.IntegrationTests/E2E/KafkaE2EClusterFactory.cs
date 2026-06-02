using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;

namespace NKafka.IntegrationTests.E2E;

internal static class KafkaE2EClusterFactory
{
    public const string ZkPlaintextProfile = "zk-plaintext";
    public const string KraftPlaintextProfile = "kraft-plaintext";
    public const string KraftSaslPlainProfile = "kraft-sasl-plain";
    public const string KraftSaslScramSha256Profile = "kraft-sasl-scram256";
    public const string KraftSaslScramSha512Profile = "kraft-sasl-scram512";

    public static bool IsE2EEnabled => ReadBooleanEnvironment("NKAFKA_E2E_ENABLED", false);

    public static string CurrentProfileName => Environment.GetEnvironmentVariable("NKAFKA_E2E_PROFILE") ?? string.Empty;

    public static string CurrentKafkaVersion => Environment.GetEnvironmentVariable("NKAFKA_E2E_KAFKA_VERSION") ?? string.Empty;

    public static bool IsZkPlaintextProfileEnabled => IsSelectedProfile(ZkPlaintextProfile);

    public static bool IsKraftPlaintextProfileEnabled => IsSelectedProfile(KraftPlaintextProfile);

    public static bool IsKraftSaslPlainProfileEnabled => IsSelectedProfile(KraftSaslPlainProfile);

    public static bool IsKraftSaslScramSha256ProfileEnabled => IsSelectedProfile(KraftSaslScramSha256Profile);

    public static bool IsKraftSaslScramSha512ProfileEnabled => IsSelectedProfile(KraftSaslScramSha512Profile);

    public static Task<IKafkaCluster> CreateClusterAsync(ILoggerFactory? loggerFactory = null)
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

        _ = ReadRequiredEnvironment("NKAFKA_E2E_PROFILE");
        _ = ReadRequiredEnvironment("NKAFKA_E2E_KAFKA_VERSION");
        _ = ReadRequiredEnvironment("NKAFKA_E2E_BOOTSTRAP_SERVERS");

        var securityProtocol = ReadSecurityProtocol();
        if (securityProtocol is SecurityProtocols.PlainText)
        {
            return;
        }

        if (securityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
        {
            _ = ReadRequiredEnvironment("NKAFKA_E2E_SASL_MECHANISM");
            _ = ReadRequiredEnvironment("NKAFKA_E2E_SASL_USERNAME");
            _ = ReadRequiredEnvironment("NKAFKA_E2E_SASL_PASSWORD");
        }
    }

    private static bool IsSelectedProfile(string profileName)
        => IsE2EEnabled
           && string.Equals(CurrentProfileName, profileName, StringComparison.OrdinalIgnoreCase);

    private static SecurityProtocols ReadSecurityProtocol()
        => Enum.TryParse(
            Environment.GetEnvironmentVariable("NKAFKA_E2E_SECURITY_PROTOCOL") ?? nameof(SecurityProtocols.PlainText),
            ignoreCase: true,
            out SecurityProtocols protocol)
            ? protocol
            : SecurityProtocols.PlainText;

    private static SaslMechanism ReadSaslMechanism()
        => Enum.TryParse(
            ReadRequiredEnvironment("NKAFKA_E2E_SASL_MECHANISM"),
            ignoreCase: true,
            out SaslMechanism mechanism)
            ? mechanism
            : throw new InvalidOperationException(
                "Unsupported NKAFKA_E2E_SASL_MECHANISM value. Expected Plain, ScramSha256 or ScramSha512.");

    private static bool ReadBooleanEnvironment(string variableName, bool defaultValue)
        => bool.TryParse(Environment.GetEnvironmentVariable(variableName), out var value)
            ? value
            : defaultValue;

    private static string ReadRequiredEnvironment(string variableName)
        => Environment.GetEnvironmentVariable(variableName)
           ?? throw new InvalidOperationException(
               $"Kafka E2E profile requires the environment variable '{variableName}'.");
}
