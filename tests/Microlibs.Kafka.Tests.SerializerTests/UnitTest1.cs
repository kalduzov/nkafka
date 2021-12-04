using System.Collections.Generic;
using Microlibs.Kafka.Serialization;
using Xunit;

namespace Microlibs.Kafka.Tests.SerializerTests;

public class ListSerializerTests
{
    [Fact]
    public void SimpleListSerializer_Successful()
    {
        var ser = new ListSerializer<int>(new IntegerSerializer());
        var result = ser.Serialize(
            new List<int>
            {
                1,
                2,
                3,
                4,
                5
            });
    }
}