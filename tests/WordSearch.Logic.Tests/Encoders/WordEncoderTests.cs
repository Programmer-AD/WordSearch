using WordSearch.Logic.Encoders;

namespace WordSearch.Logic.Tests.Encoders
{
    [TestFixture]
    public class WordEncoderTests
    {
        private const string Chars = "ab12";

        private WordEncoder wordEncoder;

        [SetUp]
        public void SetUp()
        {
            wordEncoder = new(Chars);
        }

        [Test]
        public void Constructor_SetsChars()
        {
            wordEncoder.Chars.Should().Be(Chars);
        }

        [Test]
        public void GetCharCounts_WhenWordIsNull_ThrowArgumentNullException()
        {
            string word = null;

            wordEncoder.Invoking(x => x.GetCharCounts(word))
                .Should().Throw<ArgumentNullException>();
        }

        [TestCase(Chars, new byte[] { 1, 1, 1, 1 })]
        [TestCase("aaaaa", new byte[] { 5, 0, 0, 0 })]
        [TestCase("aba1", new byte[] { 2, 1, 1, 0 })]
        [TestCase("221", new byte[] { 0, 0, 1, 2 })]
        [TestCase("", new byte[] { 0, 0, 0, 0 })]
        public void GetCharCounts_CountsCorrectly(string word, byte[] expected)
        {
            var result = wordEncoder.GetCharCounts(word);

            result.Should().Equal(expected);
        }

        [TestCase("999", new byte[] { 0, 0, 0, 0 })]
        [TestCase("_ax-b9", new byte[] { 1, 1, 0, 0 })]
        public void GetCharCounts_FiltersOtherChar(string word, byte[] expected)
        {
            var result = wordEncoder.GetCharCounts(word);

            result.Should().Equal(expected);
        }
    }
}
