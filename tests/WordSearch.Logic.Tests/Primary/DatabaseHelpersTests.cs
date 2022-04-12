using System.Collections.Generic;
using System.Numerics;
using WordSearch.Logic.Primary;

namespace WordSearch.Logic.Tests.Primary
{
    [TestFixture]
    public class DatabaseHelpersTests
    {
        private static readonly int vectorSize = Vector<byte>.Count;

        [TestCaseSource(nameof(DataSource))]
        public void GetDifference_CalculatesCorrectly(byte[] array1, byte[] array2, byte expected)
        {
            var result = DatabaseHelpers.GetDifference(array1, array2);

            result.Should().Be(expected);
        }

        private static readonly IEnumerable<object[]> DataSource = new object[][]
        {
            GetDataRow(2, 0),
            GetDataRow(1, 2, (0, 2)),
            GetDataRow(vectorSize, 4, (0, 3), (vectorSize - 1, -1)),
            GetDataRow(vectorSize + 2, 4, (1, -1), (vectorSize - 1, 1), (vectorSize + 1, 2)),
            GetDataRow(2 * vectorSize, 3, (vectorSize - 1, -1), (2 * vectorSize - 1, 2)),
        };

        private static object[] GetDataRow(int length,
            byte expectedDiff,
            params (int index, sbyte delta)[] changes)
        {
            var bytes = GetBytes(length);
            var bytesCopy = new byte[length];
            Array.Copy(bytes, bytesCopy, length);
            foreach (var (index, delta) in changes)
            {
                if (delta >= 0)
                {
                    bytesCopy[index] += (byte)delta;
                }
                else
                {
                    bytesCopy[index] -= (byte)delta;
                }
            }
            return new object[] { bytes, bytesCopy, expectedDiff };
        }

        private static byte[] GetBytes(int length)
        {
            var bytes = new byte[length];
            for (var i = 0; i < length; i++)
            {
                bytes[i] = (byte)i;
            }
            return bytes;
        }
    }
}
