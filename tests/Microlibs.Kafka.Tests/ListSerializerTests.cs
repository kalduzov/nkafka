using System.Collections.Generic;
using FluentAssertions;
using Microlibs.Kafka.Serialization;
using Xunit;

namespace Microlibs.Kafka.Tests;

public class ListSerializerTests
{
    [Fact]
    public void IntListSerializer_Successful()
    {
        var ser = new ListSerializer<int>(new IntegerSerializer());
        var data = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };
        var result = ser.Serialize(data);

        //serialization strategy flat len + null indexes len + data element len + elements  
        var size = 0x4 + 0x4 + 0x4 + sizeof(int) * data.Count;

        result.Length.Should().Be(size);
    }

    [Fact]
    public void StringListSerializer_WithNulls_Successful()
    {
        var ser = new ListSerializer<string>(new StringSerializer());
        var data = new List<string>
        {
            "test1",
            null!,
            "test3",
            null!,
            "test5"
        };
        var result = ser.Serialize(data);

        result.Length.Should().Be(43);
    }
}