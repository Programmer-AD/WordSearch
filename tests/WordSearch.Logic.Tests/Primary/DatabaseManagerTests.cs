using Microsoft.Extensions.Options;
using WordSearch.Logic.Exceptions.DatabaseManager;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary;
using WordSearch.Logic.Primary;

namespace WordSearch.Logic.Tests.Primary
{
    [TestFixture]
    public class DatabaseManagerTests
    {
        private const string DatabaseName = "test_db";
        private const string Chars = "123";

        private static readonly DatabaseConfig databaseConfig = new()
        {
            DatabaseDirectoryPath = "./test_dbs"
        };

        private Mock<IFileManager> fileManagerMock;
        private Mock<IFileIOFactory> fileIOFactoryMock;
        private Mock<IDatabaseFactory> databaseFactoryMock;
        private DatabaseManager databaseManager;

        private string CharsFilePath => databaseManager.GetCharsFilePath(DatabaseName);
        private string WordsFilePath => databaseManager.GetWordsFilePath(DatabaseName);

        [SetUp]
        public void SetUp()
        {
            fileManagerMock = new();
            fileIOFactoryMock = new();
            databaseFactoryMock = new();

            databaseManager = new(
                fileManagerMock.Object,
                fileIOFactoryMock.Object,
                databaseFactoryMock.Object,
                Options.Create(databaseConfig));
        }

        [TestCaseSource(nameof(wrongDbNames))]
        public void Create_WhenDatabaseNameIsWrong_ThrowWrongDatabaseNameException(string dbName)
        {
            SetDbExists(false);

            databaseManager.Invoking(x => x.Create(dbName, Chars))
               .Should().Throw<WrongDatabaseNameException>();
        }

        [Test]
        public void Create_WhenCharsIsNull_ThrowWrongDatabaseCharsException()
        {
            SetDbExists(false);

            databaseManager.Invoking(x => x.Create(DatabaseName, null))
               .Should().Throw<WrongDatabaseCharsException>();
        }

        [Test]
        public void Create_WhenCharsIsEmpty_ThrowWrongDatabaseCharsException()
        {
            SetDbExists(false);

            databaseManager.Invoking(x => x.Create(DatabaseName, string.Empty))
               .Should().Throw<WrongDatabaseCharsException>();
        }

        [Test]
        public void Create_WhenCharsContainsSameCharTwice_ThrowWrongDatabaseCharsException()
        {
            SetDbExists(false);

            databaseManager.Invoking(x => x.Create(DatabaseName, "1213"))
               .Should().Throw<WrongDatabaseCharsException>();
        }

        [Test]
        public void Create_WhenDatabaseExists_ThrowDatabaseAlreadyExistsException()
        {
            SetDbExists(true);

            databaseManager.Invoking(x => x.Create(DatabaseName, Chars))
               .Should().Throw<DatabaseAlreadyExistsException>();
        }

        [Test]
        public void Create_CreateWordsFile()
        {
            SetDbExists(false);
            MockFileIO(WordsFilePath);
            MockFileIO(CharsFilePath);

            databaseManager.Create(DatabaseName, Chars);

            fileManagerMock.Verify(x => x.Create(CharsFilePath), Times.Once());
        }

        [Test]
        public void Create_WritesWordsFile()
        {
            SetDbExists(false);
            var (_, _, wordsFileWriterMock) = MockFileIO(WordsFilePath);
            MockFileIO(CharsFilePath);

            databaseManager.Create(DatabaseName, Chars);

            wordsFileWriterMock.Setup(x => x.Write(It.IsAny<string>()));
        }

        [Test]
        public void Create_FlushesWordsFile()
        {
            SetDbExists(false);
            var (_, _, wordsFileWriterMock) = MockFileIO(WordsFilePath);
            MockFileIO(CharsFilePath);

            databaseManager.Create(DatabaseName, Chars);

            wordsFileWriterMock.Setup(x => x.Flush());
        }

        [Test]
        public void Create_CreateCharsFile()
        {
            SetDbExists(false);
            MockFileIO(WordsFilePath);
            MockFileIO(CharsFilePath);

            databaseManager.Create(DatabaseName, Chars);

            fileManagerMock.Verify(x => x.Create(CharsFilePath), Times.Once());
        }

        [TestCaseSource(nameof(wrongDbNames))]
        public void Delete_WhenDatabaseNameIsWrong_ThrowWrongDatabaseNameException(string dbName)
        {
            SetDbExists(true);

            databaseManager.Invoking(x => x.Delete(dbName))
               .Should().Throw<WrongDatabaseNameException>();
        }

        [Test]
        public void Delete_WhenDatabaseNotExists_ThrowDatabaseNotFoundException()
        {
            SetDbExists(false);

            databaseManager.Invoking(x => x.Delete(DatabaseName))
               .Should().Throw<DatabaseNotFoundException>();
        }

        [Test]
        public void Delete_WhenCharsFileNotExists_DeletesOnlyWordsFile()
        {
            SetDbExists(true);

            fileManagerMock.Setup(x => x.Exists(WordsFilePath)).Returns(true);
            fileManagerMock.Setup(x => x.Exists(CharsFilePath)).Returns(false);

            databaseManager.Delete(DatabaseName);

            fileManagerMock.Verify(x => x.Delete(WordsFilePath), Times.Once());
            fileManagerMock.Verify(x => x.Delete(CharsFilePath), Times.Never());
        }

        [Test]
        public void Delete_DeletesBothFiles()
        {
            SetDbExists(true);

            databaseManager.Delete(DatabaseName);

            fileManagerMock.Verify(x => x.Delete(WordsFilePath), Times.Once());
            fileManagerMock.Verify(x => x.Delete(CharsFilePath), Times.Once());
        }

        [TestCaseSource(nameof(wrongDbNames))]
        public void Get_WhenDatabaseNameIsWrong_ThrowWrongDatabaseNameException(string dbName)
        {
            SetDbExists(true);

            databaseManager.Invoking(x => x.Get(dbName))
               .Should().Throw<WrongDatabaseNameException>();
        }

        [Test]
        public void Get_WhenDatabaseNotExists_ThrowDatabaseNotFoundException()
        {
            SetDbExists(false);

            databaseManager.Invoking(x => x.Get(DatabaseName))
               .Should().Throw<DatabaseNotFoundException>();
        }

        [Test]
        public void Get_CallsDatabaseFactory()
        {
            SetDbExists(true);

            databaseManager.Get(DatabaseName);

            databaseFactoryMock.Verify(x => x.MakeDatabase(It.IsAny<string>(),
                It.IsAny<IFileIO>(), It.IsAny<IFileIO>()));
        }

        [TestCaseSource(nameof(wrongDbNames))]
        public void Exists_WhenDatabaseNameIsWrong_ThrowWrongDatabaseNameException(string dbName)
        {
            databaseManager.Invoking(x => x.Exists(dbName))
               .Should().Throw<WrongDatabaseNameException>();
        }

        [Test]
        public void Exists_CallsFileManagerExistsMethod()
        {
            databaseManager.Exists(DatabaseName);

            fileManagerMock.Verify(x => x.Exists(It.IsAny<string>()));
        }

        [Test]
        public void GetDbNames_ReturnsWordFileNamesWithoutExtension()
        {
            var fileFormat = DatabaseConstants.DatabaseWordFileExtension;
            var expected = new[] { "test", "tes_t2", "correct" };
            var fileNames = new[] {
                $"{expected[0]}{fileFormat}", "trash.word", "trash.char.ws",
                "trash.ws", $"{expected[1]}{fileFormat}", $"{expected[2]}{fileFormat}"
            };
            fileManagerMock.Setup(x => x.GetDirectoryFiles(It.IsAny<string>())).Returns(fileNames);

            var result = databaseManager.GetDbNames();

            result.Should().Equal(expected);
        }

        private void SetDbExists(bool exists)
        {
            fileManagerMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(exists);
        }

        private FileIOMock MockFileIO(
            string setupFactoryForValue)
        {
            var result = GlobalHelpers.MockFileIO();

            var (fileIOMock, _, _) = result;
            fileIOFactoryMock.Setup(x => x.MakeFileIO(setupFactoryForValue)).Returns(fileIOMock.Object);

            return result;
        }

        private static readonly string[] wrongDbNames = new string[]
        {
            "a name", "name+name", "SomeName/", ".SomeName", "", null
        };
    }
}
