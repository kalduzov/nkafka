using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;

namespace NKafka.IntegrationTests;

internal static class IntegrationClusterFactory
{
    public static bool IsSecurityScenarioEnabled =>
        ReadBooleanEnvironment("NKAFKA_IT_ENABLE_SECURITY_SCENARIOS", false)
        && ReadSecurityProtocol() is not SecurityProtocols.PlainText;

    public static bool IsScramSecurityScenarioEnabled =>
        IsSecurityScenarioEnabled
        && ReadSecurityProtocol() is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl
        && ReadSaslMechanism() is SaslMechanism.ScramSha256 or SaslMechanism.ScramSha512;

    public static bool IsPlainSaslSecurityScenarioEnabled =>
        IsSecurityScenarioEnabled
        && ReadSecurityProtocol() is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl
        && ReadSaslMechanism() is SaslMechanism.Plain;

    public static Task<IKafkaCluster> CreateClusterAsync(ILoggerFactory? loggerFactory = null)
    {
        var clusterConfig = BuildClusterConfigFromEnvironment();

        return clusterConfig.CreateCluster(loggerFactory ?? NullLoggerFactory.Instance);
    }

    private static ClusterConfig BuildClusterConfigFromEnvironment()
    {
        var securityProtocol = ReadSecurityProtocol();
        EnsureSecurityScenarioInputsAreValid(securityProtocol);
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = [ReadEnvironment("NKAFKA_IT_BOOTSTRAP_SERVERS", "localhost:29091")],
            SecurityProtocol = securityProtocol
        };

        if (securityProtocol is SecurityProtocols.Ssl or SecurityProtocols.SaslSsl)
        {
            // Integration environments often rely on self-signed certificates, so the harness must
            // be able to opt into encrypted transport without requiring extra certificate plumbing.
            clusterConfig.Ssl = new SslSettings
            {
                TrustServerCertificate = ReadBooleanEnvironment("NKAFKA_IT_TRUST_SERVER_CERTIFICATE", true)
            };
        }

        if (securityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
        {
            // Security-focused integration scenarios should be selectable through environment
            // variables so the same test body can run against plaintext or authenticated brokers.
            clusterConfig.Sasl = new SaslSettings
            {
                Mechanism = ReadSaslMechanism(),
                UserName = ReadRequiredEnvironment("NKAFKA_IT_SASL_USERNAME"),
                Password = ReadRequiredEnvironment("NKAFKA_IT_SASL_PASSWORD")
            };
        }

        return clusterConfig;
    }

    private static void EnsureSecurityScenarioInputsAreValid(SecurityProtocols securityProtocol)
    {
        if (!ReadBooleanEnvironment("NKAFKA_IT_ENABLE_SECURITY_SCENARIOS", false))
        {
            return;
        }

        if (securityProtocol is SecurityProtocols.PlainText)
        {
            throw new InvalidOperationException(
                "NKAFKA_IT_ENABLE_SECURITY_SCENARIOS=true requires NKAFKA_IT_SECURITY_PROTOCOL to be set to Ssl, SaslPlaintext or SaslSsl.");
        }

        if (securityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
        {
            _ = ReadRequiredEnvironment("NKAFKA_IT_SASL_MECHANISM");
            _ = ReadRequiredEnvironment("NKAFKA_IT_SASL_USERNAME");
            _ = ReadRequiredEnvironment("NKAFKA_IT_SASL_PASSWORD");
        }
    }

    private static SecurityProtocols ReadSecurityProtocol()
        => Enum.TryParse<SecurityProtocols>(
            ReadEnvironment("NKAFKA_IT_SECURITY_PROTOCOL", nameof(SecurityProtocols.PlainText)),
            ignoreCase: true,
            out var securityProtocol)
            ? securityProtocol
            : SecurityProtocols.PlainText;

    private static SaslMechanism ReadSaslMechanism()
        => Enum.TryParse<SaslMechanism>(
            ReadEnvironment("NKAFKA_IT_SASL_MECHANISM", nameof(SaslMechanism.Plain)),
            ignoreCase: true,
            out var mechanism)
            ? mechanism
            : SaslMechanism.Plain;

    private static bool ReadBooleanEnvironment(string variableName, bool defaultValue)
        => bool.TryParse(Environment.GetEnvironmentVariable(variableName), out var value)
            ? value
            : defaultValue;

    private static string ReadEnvironment(string variableName, string defaultValue)
        => Environment.GetEnvironmentVariable(variableName) ?? defaultValue;

    private static string ReadRequiredEnvironment(string variableName)
        => Environment.GetEnvironmentVariable(variableName)
           ?? throw new InvalidOperationException(
               $"Integration security scenario requires the environment variable '{variableName}'.");
}
