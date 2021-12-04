using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Microlibs.Kafka.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net50)]
    public class ConvertersBenchmarks
    {
        private static readonly byte[] _data =
        {
            0x00,
            0x00,
            0x00,
            0x01
        };

        private readonly Memory<byte> _memory = _data.AsMemory();

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct IntStruct
        {
            public IntStruct(byte data0, byte data1, byte data2, byte data3)
            {
                Value = 0;
                Data0 = data0;
                Data1 = data1;
                Data2 = data2;
                Data3 = data3;
            }

            [FieldOffset(0)]
            private readonly byte Data0;

            [FieldOffset(1)]
            private readonly byte Data1;

            [FieldOffset(2)]
            private readonly byte Data2;

            [FieldOffset(3)]
            private readonly byte Data3;

            [FieldOffset(0)]
            public readonly int Value;
        }

        [Benchmark(Baseline = true)]
        public int BitConverterByteArray()
        {
            return BitConverter.ToInt32(_data);
        }

        [Benchmark]
        public int StructConverterByteArray()
        {
            var struct1 = new IntStruct(_data[3], _data[2], _data[1], _data[0]);

            return struct1.Value;
        }

        [Benchmark]
        public int OperationConverterByteArray()
        {
            return _data[3] | (_data[2] << 8) | (_data[1] << 16) | (_data[0] << 24);
        }

        [Benchmark]
        public int SpanConverterByteArray()
        {
            var res = 0;

            for (var i = 0; i < _data.Length; i += 4)
            {
                res += MemoryMarshal.Cast<byte, int>(_data.AsSpan(i, 4))[0];
            }

            return res;
        }

        [Benchmark]
        public int BufferExtensionsToInt32()
        {
            return 0;//_memory.ToInt32();
        }
    }
}