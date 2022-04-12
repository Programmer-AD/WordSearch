using System.IO;
using WordSearch.Logic.IO;

namespace WordSearch.Logic.Tests.IO
{
    [TestFixture]
    public class FileIOTests
    {
        private Stream stream;
        private FileIO fileIO;

        [SetUp]
        public void SetUp()
        {
            stream = new MemoryStream();
            fileIO = new(stream);
        }

        [TearDown]
        public void TearDown()
        {
            fileIO.Dispose();
        }

        [TestCase(2)]
        [TestCase(4)]
        public void StreamPosition_get_ReturnsStreamPosition(int position)
        {
            stream.Position = position;

            fileIO.StreamPosition
                .Should().Be(position);
        }

        [TestCase(2)]
        [TestCase(4)]
        public void StreamPosition_set_SetsStreamPosition(int position)
        {
            fileIO.StreamPosition = position;

            stream.Position.Should().Be(position);
        }

        [TestCase(2)]
        [TestCase(4)]
        public void StreamLength_get_ReturnsStreamLength(int length)
        {
            stream.SetLength(length);

            fileIO.StreamLength
                .Should().Be(length);
        }

        [TestCase(2)]
        [TestCase(4)]
        public void StreamPosition_set_SetsStreamLength(int length)
        {
            fileIO.StreamLength = length;

            stream.Length.Should().Be(length);
        }

        [Test]
        public void Reader_HasValue()
        {
            fileIO.Reader
                .Should().NotBeNull();
        }

        [Test]
        public void Writer_HasValue()
        {
            fileIO.Writer
                .Should().NotBeNull();
        }
    }
}
