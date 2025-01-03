using System.Runtime.CompilerServices;

namespace NKafka.Protocol.Buffers;

internal static class MathEx
{
    private const int _ARRAY_MEX_LENGTH = 0x7FFFFFC7;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NewArrayCapacity(int size)
    {
        var newSize = unchecked(size * 2);
        if ((uint)newSize > _ARRAY_MEX_LENGTH)
        {
            newSize = _ARRAY_MEX_LENGTH;
        }
        return newSize;
    }
}