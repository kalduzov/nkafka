using System;
using System.IO;
using FluentAssertions;
using Microlibs.Kafka.Protocol;
using Xunit;

namespace Microlibs.Kafka.Tests;

public class SpanReaderTests
{
    #region Successful tests from simple buffer

    [Theory(DisplayName = "Read byte from simple buffer test ")]
    [InlineData(byte.MinValue)]
    [InlineData(byte.MaxValue)]
    public void ReadByte_FromSimpleBuffer_Successful(byte testValue)
    {
        var data = new[]
        {
            testValue
        };

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadByte();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read int from simple buffer test ")]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void ReadInt_FromSimpleBuffer_Successful(int testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadInt();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read uint from simple buffer test ")]
    [InlineData(uint.MinValue)]
    [InlineData(uint.MaxValue)]
    public void ReadUInt_FromSimpleBuffer_Successful(uint testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadUInt();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read short from simple buffer test ")]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    public void ReadShort_FromSimpleBuffer_Successful(short testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadShort();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read ushort from simple buffer test ")]
    [InlineData(ushort.MinValue)]
    [InlineData(ushort.MaxValue)]
    public void ReadUShort_FromSimpleBuffer_Successful(ushort testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadUShort();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read long from simple buffer test ")]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public void ReadLong_FromSimpleBuffer_Successful(long testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadLong();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read ulong from simple buffer test ")]
    [InlineData(ulong.MinValue)]
    [InlineData(ulong.MaxValue)]
    public void ReadULong_FromSimpleBuffer_Successful(ulong testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadULong();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read double from simple buffer test ")]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    public void ReadDouble_FromSimpleBuffer_Successful(double testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadDouble();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read boolean from simple buffer test ")]
    [InlineData(true)]
    [InlineData(false)]
    public void ReadBoolean_FromSimpleBuffer_Successful(bool testValue)
    {
        var data = BitConverter.GetBytes(testValue);
        Array.Reverse(data); //big endian required

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadBoolean();

        value.Should().Be(testValue);
    }

    [Fact(DisplayName = "Read boolean from simple buffer test where byte value > 1")]
    public void ReadBoolean_WhereByteValueMoreThenOne_Successful()
    {
        var data = new byte[]
        {
            0x05
        };

        var reader = new KafkaBufferReader(data);

        var value = reader.ReadBoolean();

        value.Should().Be(true);
    }

    [Theory(DisplayName = "Read unsigned varlong from simple buffer test ")]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(10, 10)]
    [InlineData(27, 27)]
    [InlineData(64, 64)]
    [InlineData(127, 127)]
    [InlineData(0b1010_1100_0000_0010, 300)]
    public void ReadUnsignedVarLong_FromSimpleBuffer_Successful(ulong variant, ulong testValue)
    {
        var data = BitConverter.GetBytes(variant);
        var variantBuffer = GetVariantBuffer(data);
        var reader = new KafkaBufferReader(variantBuffer);

        var value = reader.ReadUnsignedVarLong();

        value.Should().Be(testValue);
    }
    
    [Theory(DisplayName = "Read varlong from simple buffer test ")]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(3, -2)]
    public void ReadVarLong_FromSimpleBuffer_Successful(long variant, long testValue)
    {
        var data = BitConverter.GetBytes(variant);
        var variantBuffer = GetVariantBuffer(data);
        var reader = new KafkaBufferReader(variantBuffer);

        var value = reader.ReadVarLong();

        value.Should().Be(testValue);
    }

    private static byte[] GetVariantBuffer(byte[] data)
    {
        Array.Reverse(data);

        for (var i = 0; i < data.Length; i++)
        {
            if (data[i] != 0)
            {
                return data[i..];
            }
        }

        return new byte[]
        {
            0
        };
    }

    #endregion

    #region Tests who throw InvalidDataException

    [Fact(DisplayName = "Read byte from simple buffer test, but simple buffer to small ")]
    public void ReadByte_ThrowInvalidDataException_Successful()
    {
        //Can't use FluentAssertions because SpanReader is ref struct and the don't support in lambda expression 

        var data = Array.Empty<byte>();

        //var data = new byte[] {0x01}; //data for check code the test :)

        var reader = new KafkaBufferReader(data);

        try
        {
            var _ = reader.ReadByte();

            throw new Exception("wtf?");
        }
        catch (InvalidDataException)
        {
            //It's ok - test successful
        }
    }

    [Theory(DisplayName = "Read int from simple buffer test, but simple buffer to small ")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ReadInt_ThrowInvalidDataException_Successful(byte bufferLength)
    {
        //Can't use FluentAssertions because SpanReader is ref struct and the don't support in lambda expression 

        var data = new byte[bufferLength];

        //var data = new byte[4]; //data for check code the test :)

        var reader = new KafkaBufferReader(data);

        try
        {
            var _ = reader.ReadInt();

            throw new Exception("wtf?");
        }
        catch (InvalidDataException)
        {
            //It's ok - test successful
        }
    }

    #endregion
}