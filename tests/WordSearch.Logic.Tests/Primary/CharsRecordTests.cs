using Newtonsoft.Json.Linq;
using WordSearch.Logic.Primary;

namespace WordSearch.Logic.Tests.Primary
{
    [TestFixture]
    public class CharsRecordTests
    {
        private const int CharsCountLength = 5;

        private byte[] buffer;
        private CharsRecord charsRecord;

        [SetUp]
        public void SetUp()
        {
            buffer = new byte[CharsRecord.GetByteSize(CharsCountLength)];
            charsRecord = new CharsRecord(buffer);
        }

        [Test]
        public void CharCounts_HasCorrectLength()
        {
            charsRecord.CharCounts.Length
                .Should().Be(CharsCountLength);
        }

        [Test]
        public void CharCounts_WritesToBuffer()
        {
            charsRecord.CharCounts.Span.Fill(byte.MaxValue);

            buffer.Should().Contain(byte.MaxValue);
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        public void WordPosition_StoresValueCorrectly(long value)
        {
            charsRecord.WordPosition = value;

            charsRecord.WordPosition.Should().Be(value);
        }

        [Test]
        public void WordPosition_WritesToBuffer()
        {
            charsRecord.WordPosition = long.MaxValue;

            buffer.Should().Contain(byte.MaxValue);
        }

        [TestCase(0)]
        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        public void WordPositionBytes_DontOverlapCharCounts(long value)
        {
            charsRecord.WordPositionBytes.Span.Overlaps(charsRecord.CharCounts.Span)
                .Should().BeFalse();
        }
    }
}
