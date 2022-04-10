using WordSearch.Logic.Exceptions.Database;
using WordSearch.Logic.Interfaces.Encoders;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Primary;

namespace WordSearch.Logic.Tests.Primary
{
    [TestFixture]
    public class DatabaseTests
    {
        private const string DatabaseName = "SomeName";
        private const string Chars = "abc123";
        private const string Word = "aboba1";
        private const byte MaxDifference = 2;

        private FileIOMock charsFileMocks;
        private FileIOMock wordsFileMocks;
        private Mock<IWordEncoder> wordEncoderMock;
        private Database database;

        [SetUp]
        public void SetUp()
        {
            charsFileMocks = GlobalHelpers.MockFileIO();
            wordsFileMocks = GlobalHelpers.MockFileIO();
            wordEncoderMock = new();
            wordEncoderMock.Setup(x => x.Chars).Returns(Chars);

            database = new(DatabaseName,
                charsFileMocks.IOMock.Object,
                wordsFileMocks.IOMock.Object,
                wordEncoderMock.Object);
        }

        [Test]
        public async Task AddAsync_WhenWordIsNull_ThrowsArgumentException()
        {
            string word = null;

            await database.Invoking(x => x.AddAsync(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task AddAsync_WhenWordIsEmpty_ThrowsArgumentException()
        {
            var word = string.Empty;

            await database.Invoking(x => x.AddAsync(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task AddAsync_WriteWordToWordsFile()
        {
            await database.AddAsync(Word);

            wordsFileMocks.WriterMock.Verify(x => x.WriteAsync(It.IsAny<string>()));
        }

        [Test]
        public async Task AddAsync_FlushesWordsFile()
        {
            await database.AddAsync(Word);

            wordsFileMocks.WriterMock.Verify(x => x.FlushAsync());
        }

        [Test]
        public async Task AddAsync_WritesBytesToCharsFile()
        {
            await database.AddAsync(Word);

            charsFileMocks.WriterMock.Verify(x => x.WriteAsync(It.IsAny<byte[]>()));
        }

        [Test]
        public async Task AddAsync_FlushesCharsFile()
        {
            await database.AddAsync(Word);

            charsFileMocks.WriterMock.Verify(x => x.FlushAsync());
        }

        [Test]
        public async Task AddAsync_CallsWordEncoderGetCharCounts()
        {
            await database.AddAsync(Word);

            wordEncoderMock.Verify(x => x.GetCharCounts(It.IsAny<string>()));
        }

        [Test]
        public async Task DeleteAsync_WhenWordIsNull_ThrowsArgumentException()
        {
            string word = null;

            await database.Invoking(x => x.DeleteAsync(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task DeleteAsync_WhenWordIsEmpty_ThrowsArgumentException()
        {
            var word = string.Empty;

            await database.Invoking(x => x.DeleteAsync(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task DeleteAsync_WhenWordNotFound_ThrowsWordNotFoundException()
        {
            await database.Invoking(x => x.DeleteAsync(Word))
                .Should().ThrowAsync<WordNotFoundException>();
        }

        [Test]
        public async Task DeleteAsync_WriteToWordsFile()
        {
            SetWordFound();

            await database.DeleteAsync(Word);

            wordsFileMocks.WriterMock.Verify(x => x.WriteAsync(It.IsAny<string>()));
        }

        [Test]
        public async Task DeleteAsync_WhenCharsRecordExists_WriteToCharsFile()
        {
            SetCharsFound();
            SetWordFound();

            await database.DeleteAsync(Word);

            charsFileMocks.WriterMock.Verify(x => x.WriteAsync(It.IsAny<byte[]>()));
        }

        [Test]
        public async Task DeleteAsync_WhenCharsRecordNotExists_NotWriteToCharsFile()
        {
            SetWordFound();

            await database.DeleteAsync(Word);

            charsFileMocks.WriterMock.Verify(x => x.WriteAsync(It.IsAny<byte[]>()), Times.Never());
        }

        [Test]
        public async Task DeleteAsync_WhenCharsRecordExists_UpdateCharsFileSize()
        {
            SetCharsFound();
            SetWordFound();

            await database.DeleteAsync(Word);

            charsFileMocks.IOMock.VerifySet(x => x.StreamLength = It.IsAny<long>());
        }

        [Test]
        public async Task DeleteAsync_WhenCharsRecordNotExists_NotUpdateCharsFileSize()
        {
            SetWordFound();

            await database.DeleteAsync(Word);

            charsFileMocks.IOMock.VerifySet(x => x.StreamLength = It.IsAny<long>(), Times.Never());
        }

        [Test]
        public async Task DeleteAsync_FlushesWordsFile()
        {
            SetWordFound();

            await database.DeleteAsync(Word);

            wordsFileMocks.WriterMock.Verify(x => x.FlushAsync());
        }

        [Test]
        public async Task DeleteAsync_FlushesCharsFile()
        {
            SetCharsFound();
            SetWordFound();

            await database.DeleteAsync(Word);

            charsFileMocks.WriterMock.Verify(x => x.FlushAsync());
        }

        [Test]
        public async Task GetWordsAsync_WhenWordIsNull_ThrowsArgumentException()
        {
            string word = null;

            await database.Invoking(x => x.GetWordsAsync(word, MaxDifference))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task GetWordsAsync_WhenWordIsEmpty_ThrowsArgumentException()
        {
            var word = string.Empty;

            await database.Invoking(x => x.GetWordsAsync(word, MaxDifference))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task GetWordsAsync_CallWordEncoderGetCharCountsOnce()
        {
            await database.GetWordsAsync(Word, MaxDifference);

            wordEncoderMock.Verify(x => x.GetCharCounts(It.IsAny<string>()), Times.Once());
        }

        private void SetWordFound()
        {
            wordsFileMocks.ReaderMock.Setup(x => x.GetStringAsync()).ReturnsAsync(Word);
            wordsFileMocks.IOMock.Setup(x => x.StreamLength).Returns(1);
        }

        private void SetCharsFound()
        {
            charsFileMocks.IOMock.Setup(x => x.StreamLength).Returns(1);
        }
    }
}
