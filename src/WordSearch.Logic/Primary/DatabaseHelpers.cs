using System.Numerics;

namespace WordSearch.Logic.Primary
{
    internal static class DatabaseHelpers
    {
        public static byte GetDifference(byte[] charCounts1, byte[] charCounts2)
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
                    var vector1 = (Vector<sbyte>)new Vector<byte>(charCounts1, i);
                    var vector2 = (Vector<sbyte>)new Vector<byte>(charCounts2, i);
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