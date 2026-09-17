// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Protocol;
using NKafka.Clients.Producer;

namespace NKafka.Tests.Clients.Producer;

public sealed class ProducerErrorTests
{
    [Fact]
    public void LocalError_UsesClientErrorCode()
    {
        var error = new ProducerError(ErrorCodes.ClientError, ProducerLocalError.Cancelled);

        error.ErrorCode.Should().Be(ErrorCodes.ClientError);
        error.LocalError.Should().Be(ProducerLocalError.Cancelled);
    }

    [Fact]
    public void KafkaError_UsesNoLocalError()
    {
        var error = new ProducerError(ErrorCodes.NetworkException, ProducerLocalError.None);

        error.ErrorCode.Should().Be(ErrorCodes.NetworkException);
        error.LocalError.Should().Be(ProducerLocalError.None);
    }

    [Theory]
    [InlineData(ErrorCodes.None, ProducerLocalError.None)]
    [InlineData(ErrorCodes.ClientError, ProducerLocalError.None)]
    [InlineData(ErrorCodes.NetworkException, ProducerLocalError.TransportFailure)]
    public void InvalidErrorClassification_Throws(ErrorCodes code, ProducerLocalError localError)
    {
        FluentActions.Invoking(() => new ProducerError(code, localError))
            .Should()
            .Throw<ArgumentException>();
    }
}
