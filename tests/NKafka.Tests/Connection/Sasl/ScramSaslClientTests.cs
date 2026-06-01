using System.Text;

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Connection.Sasl;
using NKafka.Connection.Sasl.Messages;

namespace NKafka.Tests.Connection.Sasl;

public sealed class ScramSaslClientTests
{
    [Fact]
    public void EvaluateChallenge_CompletesFullExchange_ForScramSha256()
    {
        VerifyFullExchange(ScramMechanism.ScramSha256);
    }

    [Fact]
    public void EvaluateChallenge_CompletesFullExchange_ForScramSha512()
    {
        VerifyFullExchange(ScramMechanism.ScramSha512);
    }

    private static void VerifyFullExchange(ScramMechanism mechanism)
    {
        using var client = new ScramSaslClient(mechanism, new TestSaslAuthStore("user", "pencil"), NullLogger<ScramSaslClient>.Instance);
        using var formatter = new ScramFormatter(mechanism);

        var clientFirstBytes = client.EvaluateChallenge([]);
        var clientFirst = new ClientFirstMessage(clientFirstBytes);
        var serverFirst = new ServerFirstMessage(clientFirst.Nonce, "server-nonce", Convert.FromBase64String("W22ZaJ0SNY7soEsUEjb6gQ=="), 4096);

        var clientFinalBytes = client.EvaluateChallenge(Encoding.UTF8.GetBytes(serverFirst.ToMessage()));
        var clientFinal = new ClientFinalMessage(clientFinalBytes.ToArray());

        var saltedPassword = formatter.SaltedPassword("pencil", serverFirst.Salt, serverFirst.Iterations);
        var serverKey = formatter.ServerKey(saltedPassword);
        var serverSignature = formatter.ServerSignature(serverKey, clientFirst, serverFirst, clientFinal);
        var serverFinalBytes = Encoding.UTF8.GetBytes($"v={Convert.ToBase64String(serverSignature)}");

        client.EvaluateChallenge(serverFinalBytes);

        client.IsComplete.Should().BeTrue();
        client.MechanismName.Should().Be(mechanism.GetName());
    }

    private sealed class TestSaslAuthStore(string userName, string password): ISaslAuthStore
    {
        public string GetUserName()
        {
            return userName;
        }

        public Dictionary<string, string> GetExtensions()
        {
            return [];
        }

        public Span<byte> GetPasswordAsBytes()
        {
            return Encoding.UTF8.GetBytes(password);
        }
    }
}
