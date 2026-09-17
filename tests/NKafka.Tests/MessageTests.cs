// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace NKafka.Tests;

public sealed class MessageTests
{
    [Fact]
    public void Message_PreservesNullAndEmptyKeyAndValue()
    {
        var tombstone = new Message(null, null);
        var empty = new Message(Array.Empty<byte>(), Array.Empty<byte>());

        tombstone.Key.Should().BeNull();
        tombstone.Value.Should().BeNull();
        empty.Key.Should().NotBeNull().And.BeEmpty();
        empty.Value.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Headers_Add_PreservesNullAndEmptyValues()
    {
        var headers = new Headers(null);

        headers.Add("null", null);
        headers.Add("empty", Array.Empty<byte>());

        headers[0].Value.Should().BeNull();
        headers[1].Value.Should().NotBeNull().And.BeEmpty();
    }
}
