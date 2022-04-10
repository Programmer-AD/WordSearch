using System.Numerics;

namespace WordSearch.Logic
{
    internal static class DatabaseHelpers
    {
        public static byte GetDifference(byte[] charCounts1, byte[] charCounts2)
        {
            byte result = 0;

            var vectorSize = Vector<sbyte>.Count;
            var length = GetSpanLength(vectorSize, charCounts1.Length);

            Span<byte> buffer1 = stackalloc byte[length];
            charCounts1.CopyTo(buffer1);

            Span<byte> buffer2 = stackalloc byte[length];
            charCounts2.CopyTo(buffer2);

            for (int i = 0; i < length; i += vectorSize)
            {
                var vector1 = GetVector(buffer1, i, vectorSize);
                var vector2 = GetVector(buffer2, i, vectorSize);
                var absoluteDiff = (Vector<byte>)Vector.Abs(vector1 - vector2);
                result += Vector.Dot(absoluteDiff, Vector<byte>.One);
            }

            return result;
        }

        private static int GetSpanLength(int vectorSize, int arrayLength)
        {
            var length = arrayLength / vectorSize;
            if (arrayLength % vectorSize != 0)
            {
                length++;
            }
            length *= vectorSize;
            return length;
        }

        private static Vector<sbyte> GetVector(Span<byte> buffer, int position, int vectorSize)
        {
            var slice = buffer.Slice(position, vectorSize);
            var vector = new Vector<sbyte>(slice);
            return vector;
        }
    }
}