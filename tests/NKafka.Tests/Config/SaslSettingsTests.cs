using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Tests.Config;

public sealed class SaslSettingsTests
{
    [Fact]
    public void Validate_WithPlainAndCredentials_Succeeds()
    {
        var settings = new SaslSettings
        {
            Mechanism = SaslMechanism.Plain,
            UserName = "test-user",
            Password = "test-password"
        };

        FluentActions.Invoking(() => settings.Validate()).Should().NotThrow();
    }

    [Fact]
    public void Validate_WithPlainAndMissingCredentials_ThrowsKafkaConfigException()
    {
        var settings = new SaslSettings
        {
            Mechanism = SaslMechanism.Plain
        };

        FluentActions.Invoking(() => settings.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(SaslSettings.Mechanism));
    }

    [Fact]
    public void Validate_WithOAuthBearer_ThrowsKafkaConfigException()
    {
        var settings = new SaslSettings
        {
            Mechanism = SaslMechanism.OAuthBearer
        };

        FluentActions.Invoking(() => settings.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(SaslSettings.Mechanism));
    }

    [Theory]
    [InlineData(SaslMechanism.ScramSha256)]
    [InlineData(SaslMechanism.ScramSha512)]
    public void Validate_WithScramAndCredentials_Succeeds(SaslMechanism mechanism)
    {
        var settings = new SaslSettings
        {
            Mechanism = mechanism,
            UserName = "test-user",
            Password = "test-password"
        };

        FluentActions.Invoking(() => settings.Validate()).Should().NotThrow();
    }

    [Fact]
    public void Validate_WithKerberos_ThrowsKafkaConfigException()
    {
        var settings = new SaslSettings
        {
            Mechanism = SaslMechanism.Kerberos
        };

        FluentActions.Invoking(() => settings.Validate())
            .Should()
            .Throw<KafkaConfigException>()
            .Which.OptionName.Should()
            .Be(nameof(SaslSettings.Mechanism));
    }
}
