using System.Collections.Generic;
using WordSearch.Logic.Exceptions.WordsFile;
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
        public void Add_WhenWordIsNull_ThrowsWrongWordException()
        {
            string word = null;

            database.Invoking(x => x.Add(word))
               .Should().Throw<WrongWordException>();
        }

        [Test]
        public void Add_WhenWordIsEmpty_ThrowsWrongWordException()
        {
            var word = string.Empty;

            database.Invoking(x => x.Add(word))
               .Should().Throw<WrongWordException>();
        }

        [Test]
        public void Add_CallWordsFileAdd()
        {
            database.Add(Word);

            wordsFileMock.Verify(x => x.Add(It.IsAny<string>()));
        }

        [Test]
        public void Add_CallsWordEncoderGetCharCounts()
        {
            database.Add(Word);

            wordEncoderMock.Verify(x => x.GetCharCounts(It.IsAny<string>()));
        }

        [Test]
        public void Add_CallCharsFileAdd()
        {
            database.Add(Word);

            charsFileMock.Verify(x => x.Add(It.IsAny<Action<CharsRecord>>()));
        }

        [Test]
        public void Delete_WhenWordIsNull_ThrowsArgumentException()
        {
            string word = null;

            database.Invoking(x => x.Delete(word))
               .Should().Throw<WrongWordException>();
        }

        [Test]
        public void Delete_WhenWordIsEmpty_ThrowsWrongWordException()
        {
            var word = string.Empty;

            database.Invoking(x => x.Delete(word))
               .Should().Throw<WrongWordException>();
        }

        [Test]
        public void Delete_CallWordsFileDeleteWithCorrectParametr()
        {
            long wordPosition = 20;
            wordsFileMock.Setup(x => x.GetWordPosition(It.IsAny<string>()))
                .Returns(wordPosition);
            long recordPosition = 100;
            charsFileMock.Setup(x => x.GetRecordPositionByWordPosition(It.IsAny<long>()))
                .Returns(recordPosition);

            database.Delete(Word);

            wordsFileMock.Verify(x => x.Delete(wordPosition));
        }

        [Test]
        public void Delete_CallCharsFileDeleteWithCorrectParam()
        {
            long wordPosition = 20;
            wordsFileMock.Setup(x => x.GetWordPosition(It.IsAny<string>()))
                .Returns(wordPosition);
            long recordPosition = 100;
            charsFileMock.Setup(x => x.GetRecordPositionByWordPosition(It.IsAny<long>()))
                .Returns(recordPosition);

            database.Delete(Word);

            charsFileMock.Verify(x => x.Delete(recordPosition));
        }

        [Test]
        public void GetWords_WhenWordIsNull_ThrowsWrongWordException()
        {
            string word = null;

            database.Invoking(x => x.GetWords(word, MaxDifference))
               .Should().Throw<WrongWordException>();
        }

        [Test]
        public void GetWords_WhenWordIsEmpty_ThrowsWrongWordException()
        {
            var word = string.Empty;

            database.Invoking(x => x.GetWords(word, MaxDifference))
               .Should().Throw<WrongWordException>();
        }

        [Test]
        public void GetWords_CallsWordEncoderGetCharCountsOnce()
        {
            var enumeratorMock = new Mock<IEnumerator<CharsRecord>>();
            charsFileMock.Setup(x => x.GetEnumerator())
                .Returns(enumeratorMock.Object);

            database.GetWords(Word, MaxDifference);

            wordEncoderMock.Verify(x => x.GetCharCounts(It.IsAny<string>()), Times.Once());
        }
    }
}
