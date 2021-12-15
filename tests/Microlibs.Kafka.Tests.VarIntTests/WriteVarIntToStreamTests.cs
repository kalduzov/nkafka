using System;
using System.IO;
using FluentAssertions;
using Microlibs.Kafka.Protocol;
using Xunit;

namespace Microlibs.Kafka.Tests.VarIntTests;

public class WriteVarIntToStreamTests
{
    [Theory]
    [InlineData(0x1, 0x1, 1)]
    [InlineData(0x0, 0x0, 1)]
    [InlineData(0x12C, 0x2AC, 2)]
    public void WriteVarUInt64Test(ulong value, ulong calculateValue, int countBytes)
    {
        using var stream = new MemoryStream(5);

        var count = stream.WriteVarUInt64(value);

        count.Should().Be(countBytes);

        var array = stream.ToArray();
        Array.Resize(ref array, 8);
        var checkValue = BitConverter.ToUInt64(array);
        checkValue.Should().Be(calculateValue);
    }
}