//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright �  2023 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NKafka.Serialization;

namespace NKafka.Tests.Serialization;

public partial class SerializerDeserializerTests
{
    #region int serialization test

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "00000001")]
    [InlineData(int.MaxValue, "7FFFFFFF")]
    [InlineData(int.MinValue, "80000000")]
    public async Task SerializeIntAsync_Successful(int value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<int> serializer = new IntSerializer();
        var result = await serializer.SerializeAsync(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "00000001")]
    [InlineData(int.MaxValue, "7FFFFFFF")]
    [InlineData(int.MinValue, "80000000")]
    public void SerializeInt_Successful(int value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<int> serializer = new IntSerializer();
        var result = serializer.Serialize(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "00000001")]
    [InlineData(int.MaxValue, "7FFFFFFF")]
    [InlineData(int.MinValue, "80000000")]
    public async Task DeserializeIntAsync_Successful(int value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<int> deserializer = new IntDeserializer();
        var result = await deserializer.DeserializeAsync(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "00000001")]
    [InlineData(int.MaxValue, "7FFFFFFF")]
    [InlineData(int.MinValue, "80000000")]
    public void DeserializeInt_Successful(int value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<int> deserializer = new IntDeserializer();
        var result = deserializer.Deserialize(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void SerializeDeserializeInt_Successful(int value)
    {
        IAsyncSerializer<int> serializer = new IntSerializer();
        var serializeValue = serializer.Serialize(value);

        IAsyncDeserializer<int> deserializer = new IntDeserializer();
        var deserializeValue = deserializer.Deserialize(serializeValue);

        value.Should().Be(deserializeValue);
    }

    #endregion

    #region long serialization test

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "0000000000000001")]
    [InlineData(long.MaxValue, "7FFFFFFFFFFFFFFF")]
    [InlineData(long.MinValue, "8000000000000000")]
    public async Task SerializeLongAsync_Successful(long value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<long> serializer = new LongSerializer();
        var result = await serializer.SerializeAsync(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "0000000000000001")]
    [InlineData(long.MaxValue, "7FFFFFFFFFFFFFFF")]
    [InlineData(long.MinValue, "8000000000000000")]
    public void SerializeLong_Successful(long value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<long> serializer = new LongSerializer();
        var result = serializer.Serialize(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "0000000000000001")]
    [InlineData(long.MaxValue, "7FFFFFFFFFFFFFFF")]
    [InlineData(long.MinValue, "8000000000000000")]
    public async Task DeserializeLongAsync_Successful(long value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<long> deserializer = new LongDeserializer();
        var result = await deserializer.DeserializeAsync(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "0000000000000001")]
    [InlineData(long.MaxValue, "7FFFFFFFFFFFFFFF")]
    [InlineData(long.MinValue, "8000000000000000")]
    public void DeserializeLong_Successful(long value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<long> deserializer = new LongDeserializer();
        var result = deserializer.Deserialize(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void SerializeDeserializeLong_Successful(long value)
    {
        IAsyncSerializer<long> serializer = new LongSerializer();
        var serializeValue = serializer.Serialize(value);

        IAsyncDeserializer<long> deserializer = new LongDeserializer();
        var deserializeValue = deserializer.Deserialize(serializeValue);

        value.Should().Be(deserializeValue);
    }

    #endregion

    #region short serialization test

    [Theory]
    [InlineData(0, "0000")]
    [InlineData(1, "0001")]
    [InlineData(short.MaxValue, "7FFF")]
    [InlineData(short.MinValue, "8000")]
    public async Task SerializeShortAsync_Successful(short value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<short> serializer = new ShortSerializer();
        var result = await serializer.SerializeAsync(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "0000")]
    [InlineData(1, "0001")]
    [InlineData(short.MaxValue, "7FFF")]
    [InlineData(short.MinValue, "8000")]
    public void SerializeShort_Successful(short value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<short> serializer = new ShortSerializer();
        var result = serializer.Serialize(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "0000")]
    [InlineData(1, "0001")]
    [InlineData(short.MaxValue, "7FFF")]
    [InlineData(short.MinValue, "8000")]
    public async Task DeserializeShortAsync_Successful(short value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<short> deserializer = new ShortDeserializer();
        var result = await deserializer.DeserializeAsync(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0, "0000")]
    [InlineData(1, "0001")]
    [InlineData(short.MaxValue, "7FFF")]
    [InlineData(short.MinValue, "8000")]
    public void DeserializeShort_Successful(short value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<short> deserializer = new ShortDeserializer();
        var result = deserializer.Deserialize(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(short.MaxValue)]
    [InlineData(short.MinValue)]
    public void SerializeDeserializeShort_Successful(short value)
    {
        IAsyncSerializer<short> serializer = new ShortSerializer();
        var serializeValue = serializer.Serialize(value);

        IAsyncDeserializer<short> deserializer = new ShortDeserializer();
        var deserializeValue = deserializer.Deserialize(serializeValue);

        value.Should().Be(deserializeValue);
    }

    #endregion

    #region double serialization test

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "3ff0000000000000")]
    [InlineData(double.MaxValue, "7FEFFFFFFFFFFFFF")]
    public async Task SerializeDoubleAsync_Successful(double value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<double> serializer = new DoubleSerializer();
        var result = await serializer.SerializeAsync(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "3ff0000000000000")]
    [InlineData(double.MaxValue, "7FEFFFFFFFFFFFFF")]
    public void SerializeDouble_Successful(double value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<double> serializer = new DoubleSerializer();
        var result = serializer.Serialize(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "3ff0000000000000")]
    [InlineData(double.MaxValue, "7FEFFFFFFFFFFFFF")]
    public async Task DeserializeDoubleAsync_Successful(double value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<double> deserializer = new DoubleDeserializer();
        var result = await deserializer.DeserializeAsync(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0, "0000000000000000")]
    [InlineData(1, "3ff0000000000000")]
    [InlineData(double.MaxValue, "7FEFFFFFFFFFFFFF")]
    public void DeserializeDouble_Successful(double value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<double> deserializer = new DoubleDeserializer();
        var result = deserializer.Deserialize(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(double.MaxValue)]
    public void SerializeDeserializeDouble_Successful(double value)
    {
        IAsyncSerializer<double> serializer = new DoubleSerializer();
        var serializeValue = serializer.Serialize(value);

        IAsyncDeserializer<double> deserializer = new DoubleDeserializer();
        var deserializeValue = deserializer.Deserialize(serializeValue);

        value.Should().Be(deserializeValue);
    }

    #endregion

    #region float serialization test

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "3f800000")]
    [InlineData(float.MaxValue, "7F7FFFFF")]
    public async Task SerializeFloatAsync_Successful(float value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<float> serializer = new FloatSerializer();
        var result = await serializer.SerializeAsync(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "3f800000")]
    [InlineData(float.MaxValue, "7F7FFFFF")]
    public void SerializeFloat_Successful(float value, string serializeValueAsByteString)
    {
        var serializeValue = Convert.FromHexString(serializeValueAsByteString);
        IAsyncSerializer<float> serializer = new FloatSerializer();
        var result = serializer.Serialize(value);

        result.Should().BeEquivalentTo(serializeValue);
    }

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "3f800000")]
    [InlineData(float.MaxValue, "7F7FFFFF")]
    public async Task DeserializeFloatAsync_Successful(float value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<float> deserializer = new FloatDeserializer();
        var result = await deserializer.DeserializeAsync(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "3f800000")]
    [InlineData(float.MaxValue, "7F7FFFFF")]
    public void DeserializeFloat_Successful(float value, string deserializeValueAsByteString)
    {
        var deserializeValue = Convert.FromHexString(deserializeValueAsByteString);
        IAsyncDeserializer<float> deserializer = new FloatDeserializer();
        var result = deserializer.Deserialize(deserializeValue);

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(float.MaxValue)]
    public void SerializeDeserializeFloat_Successful(float value)
    {
        IAsyncSerializer<float> serializer = new FloatSerializer();
        var serializeValue = serializer.Serialize(value);

        IAsyncDeserializer<float> deserializer = new FloatDeserializer();
        var deserializeValue = deserializer.Deserialize(serializeValue);

        value.Should().Be(deserializeValue);
    }

    #endregion
}