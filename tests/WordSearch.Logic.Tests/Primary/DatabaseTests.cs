using System.Collections.Generic;
using System.Threading;
using WordSearch.Logic.Interfaces.Encoders;
using WordSearch.Logic.Interfaces.Primary.Files;
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

        private Mock<ICharsFile> charsFileMock;
        private Mock<IWordsFile> wordsFileMock;
        private Mock<IWordEncoder> wordEncoderMock;
        private Database database;

        [SetUp]
        public void SetUp()
        {
            charsFileMock = new();
            wordsFileMock = new();
            wordEncoderMock = new();
            wordEncoderMock.Setup(x => x.Chars).Returns(Chars);

            database = new(DatabaseName,
                charsFileMock.Object,
                wordsFileMock.Object,
                wordEncoderMock.Object);
        }

        [Test]
        public async Task AddAsync_WhenWordIsNull_ThrowsArgumentException()
        {
            string word = null;

            await database.Invoking(x => x.Add(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task AddAsync_WhenWordIsEmpty_ThrowsArgumentException()
        {
            var word = string.Empty;

            await database.Invoking(x => x.Add(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task AddAsync_CallWordsFileAddAsync()
        {
            await database.Add(Word);
            
            wordsFileMock.Verify(x => x.Add(It.IsAny<string>()));
        }

        [Test]
        public async Task AddAsync_CallsWordEncoderGetCharCounts()
        {
            await database.Add(Word);

            wordEncoderMock.Verify(x => x.GetCharCounts(It.IsAny<string>()));
        }

        [Test]
        public async Task AddAsync_CallCharsFileAddAsync()
        {
            await database.Add(Word);

            charsFileMock.Verify(x => x.Add(It.IsAny<Action<CharsRecord>>()));
        }

        [Test]
        public async Task DeleteAsync_WhenWordIsNull_ThrowsArgumentException()
        {
            string word = null;

            await database.Invoking(x => x.Delete(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task DeleteAsync_WhenWordIsEmpty_ThrowsArgumentException()
        {
            var word = string.Empty;

            await database.Invoking(x => x.Delete(word))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task DeleteAsync_CallWordsFileDeleteAsyncWithCorrectParametr()
        {
            long wordPosition = 20;
            wordsFileMock.Setup(x => x.GetWordPosition(It.IsAny<string>()))
                .ReturnsAsync(wordPosition);
            long recordPosition = 100;
            charsFileMock.Setup(x => x.GetRecordPositionByWordPosition(It.IsAny<long>()))
                .ReturnsAsync(recordPosition);

            await database.Delete(Word);

            wordsFileMock.Verify(x => x.Delete(wordPosition));
        }

        [Test]
        public async Task DeleteAsync_CallCharsFileDeleteAsyncWithCorrectParam()
        {
            long wordPosition = 20;
            wordsFileMock.Setup(x => x.GetWordPosition(It.IsAny<string>()))
                .ReturnsAsync(wordPosition);
            long recordPosition = 100;
            charsFileMock.Setup(x => x.GetRecordPositionByWordPosition(It.IsAny<long>()))
                .ReturnsAsync(recordPosition);

            await database.Delete(Word);

            charsFileMock.Verify(x => x.Delete(recordPosition));
        }

        [Test]
        public async Task GetWordsAsync_WhenWordIsNull_ThrowsArgumentException()
        {
            string word = null;

            await database.Invoking(x => x.GetWords(word, MaxDifference))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task GetWordsAsync_WhenWordIsEmpty_ThrowsArgumentException()
        {
            var word = string.Empty;

            await database.Invoking(x => x.GetWords(word, MaxDifference))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task GetWordsAsync_CallsWordEncoderGetCharCountsOnce()
        {
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<CharsRecord>>();
            charsFileMock.Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(asyncEnumeratorMock.Object);

            await database.GetWords(Word, MaxDifference);

            wordEncoderMock.Verify(x => x.GetCharCounts(It.IsAny<string>()), Times.Once());
        }
    }
}
