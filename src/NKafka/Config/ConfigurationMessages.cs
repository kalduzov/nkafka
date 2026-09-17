// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace NKafka.Config;

internal static class ConfigurationMessages
{
    internal static string CommonConfig_ClientDisposeTimeoutInvalid => Get("CommonConfig_ClientDisposeTimeoutInvalid");
    internal static string CommonConfig_ReceiveBufferInvalid => Get("CommonConfig_ReceiveBufferInvalid");
    internal static string CommonConfig_ValueMustBePositive => Get("CommonConfig_ValueMustBePositive");
    internal static string CommonConfig_ValueMustNotBeNegative => Get("CommonConfig_ValueMustNotBeNegative");
    internal static string ProducerConfig_LingerInvalid => Get("ProducerConfig_LingerInvalid");
    internal static string ProducerConfig_DeliveryTimeoutTooShort => Get("ProducerConfig_DeliveryTimeoutTooShort");
    internal static string ProducerConfig_AcksInvalid => Get("ProducerConfig_AcksInvalid");
    internal static string ProducerConfig_CompressionInvalid => Get("ProducerConfig_CompressionInvalid");
    internal static string PartitionerConfig_Invalid => Get("PartitionerConfig_Invalid");
    internal static string ClusterConfig_MetadataUpdateTimeoutInvalid => Get("ClusterConfig_MetadataUpdateTimeoutInvalid");
    internal static string ClusterConfig_ClusterInitTimeoutInvalid => Get("ClusterConfig_ClusterInitTimeoutInvalid");
    internal static string ConsumerConfig_ChannelSizeInvalid => Get("ConsumerConfig_ChannelSizeInvalid");
    internal static string ConsumerConfig_GroupIdRequired => Get("ConsumerConfig_GroupIdRequired");
    internal static string ConsumerConfig_AutoCommitIntervalInvalid => Get("ConsumerConfig_AutoCommitIntervalInvalid");
    internal static string ConsumerConfig_PartitionAssignorsRequired => Get("ConsumerConfig_PartitionAssignorsRequired");
    internal static string ConsumerConfig_PartitionAssignorsNotUnique => Get("ConsumerConfig_PartitionAssignorsNotUnique");
    internal static string ConsumerConfig_HeartbeatInvalid => Get("ConsumerConfig_HeartbeatInvalid");
    internal static string ConsumerConfig_MaxPollIntervalInvalid => Get("ConsumerConfig_MaxPollIntervalInvalid");
    internal static string SaslSettings_OAuthBearerUnsupported => Get("SaslSettings_OAuthBearerUnsupported");
    internal static string SaslSettings_KerberosUnsupported => Get("SaslSettings_KerberosUnsupported");
    internal static string SaslSettings_CredentialsRequired => Get("SaslSettings_CredentialsRequired");

    private static string Get(string name)
    {
        return NKafka.Resources.ConfigExceptionMessages.ResourceManager.GetString(name)!;
    }
}
