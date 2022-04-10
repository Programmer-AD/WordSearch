using System.Numerics;

namespace WordSearch.Logic.Primary
{
    internal static class DatabaseHelpers
    {
        public static byte GetDifference(Span<byte> charCounts1, Span<byte> charCounts2)
        {
            byte result = 0;

            var vectorSize = Vector<sbyte>.Count;
            var length = charCounts1.Length;
            var acceleratedLength = length / vectorSize * vectorSize;
            int i = 0;

            if (acceleratedLength > 0)
            {
                var resultVector = new Vector<byte>();
                for (; i < acceleratedLength; i += vectorSize)
                {
                    var slice1 = charCounts1.Slice(i, vectorSize);
                    var slice2 = charCounts2.Slice(i, vectorSize);
                    var vector1 = (Vector<sbyte>)new Vector<byte>(slice1);
                    var vector2 = (Vector<sbyte>)new Vector<byte>(slice2);
                    var absoluteDiff = (Vector<byte>)Vector.Abs(vector1 - vector2);
                    resultVector += absoluteDiff;
                }
                result += Vector.Dot(resultVector, Vector<byte>.One);
            }

            for (; i < length; i++)
            {
                result += (byte)Math.Abs(charCounts1[i] - charCounts2[i]);
            }

            return result;
        }
    }
}