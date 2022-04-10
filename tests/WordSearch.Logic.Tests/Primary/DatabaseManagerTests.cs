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
            DatabaseDirectory = "./test_dbs"
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
        public async Task CreateAsync_WhenDatabaseNameIsWrong_ThrowDatabaseWrongNameException(string dbName)
        {
            SetDbExists(false);

            await databaseManager.Invoking(x => x.CreateAsync(dbName, Chars))
                .Should().ThrowAsync<DatabaseWrongNameException>();
        }

        [Test]
        public async Task CreateAsync_WhenCharsIsNull_ThrowArgumentException()
        {
            SetDbExists(false);

            await databaseManager.Invoking(x => x.CreateAsync(DatabaseName, null))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task CreateAsync_WhenCharsIsEmpty_ThrowArgumentException()
        {
            SetDbExists(false);

            await databaseManager.Invoking(x => x.CreateAsync(DatabaseName, string.Empty))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task CreateAsync_WhenCharsContainsSameCharTwice_ThrowArgumentException()
        {
            SetDbExists(false);

            await databaseManager.Invoking(x => x.CreateAsync(DatabaseName, "1213"))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task CreateAsync_WhenDatabaseExists_ThrowDatabaseAlreadyExistsException()
        {
            SetDbExists(true);

            await databaseManager.Invoking(x => x.CreateAsync(DatabaseName, Chars))
                .Should().ThrowAsync<DatabaseAlreadyExistsException>();
        }

        [Test]
        public async Task CreateAsync_CreateWordsFile()
        {
            SetDbExists(false);
            MockFileIO(WordsFilePath);
            MockFileIO(CharsFilePath);

            await databaseManager.CreateAsync(DatabaseName, Chars);

            fileManagerMock.Verify(x => x.Create(CharsFilePath), Times.Once());
        }

        [Test]
        public async Task CreateAsync_InitWordsFile()
        {
            SetDbExists(false);
            var (_, _, wordsFileWriterMock) = MockFileIO(WordsFilePath);
            MockFileIO(CharsFilePath);

            await databaseManager.CreateAsync(DatabaseName, Chars);

            wordsFileWriterMock.Setup(x => x.WriteAsync(It.IsAny<string>()));
        }

        [Test]
        public async Task CreateAsync_CreateCharsFile()
        {
            SetDbExists(false);
            MockFileIO(WordsFilePath);
            MockFileIO(CharsFilePath);

            await databaseManager.CreateAsync(DatabaseName, Chars);

            fileManagerMock.Verify(x => x.Create(CharsFilePath), Times.Once());
        }

        [TestCaseSource(nameof(wrongDbNames))]
        public async Task DeleteAsync_WhenDatabaseNameIsWrong_ThrowDatabaseWrongNameException(string dbName)
        {
            SetDbExists(true);

            await databaseManager.Invoking(x => x.DeleteAsync(dbName))
                .Should().ThrowAsync<DatabaseWrongNameException>();
        }

        [Test]
        public async Task DeleteAsync_WhenDatabaseNotExists_ThrowDatabaseNotFoundException()
        {
            SetDbExists(false);

            await databaseManager.Invoking(x => x.DeleteAsync(DatabaseName))
                .Should().ThrowAsync<DatabaseNotFoundException>();
        }

        [Test]
        public async Task DeleteAsync_WhenCharsFileNotExists_DeletesOnlyWordsFile()
        {
            SetDbExists(true);

            fileManagerMock.Setup(x => x.Exists(WordsFilePath)).Returns(true);
            fileManagerMock.Setup(x => x.Exists(CharsFilePath)).Returns(false);

            await databaseManager.DeleteAsync(DatabaseName);

            fileManagerMock.Verify(x => x.Delete(WordsFilePath), Times.Once());
            fileManagerMock.Verify(x => x.Delete(CharsFilePath), Times.Never());
        }

        [Test]
        public async Task DeleteAsync_DeletesBothFiles()
        {
            SetDbExists(true);

            await databaseManager.DeleteAsync(DatabaseName);

            fileManagerMock.Verify(x => x.Delete(WordsFilePath), Times.Once());
            fileManagerMock.Verify(x => x.Delete(CharsFilePath), Times.Once());
        }

        [TestCaseSource(nameof(wrongDbNames))]
        public async Task GetAsync_WhenDatabaseNameIsWrong_ThrowDatabaseWrongNameException(string dbName)
        {
            SetDbExists(true);

            await databaseManager.Invoking(x => x.GetAsync(dbName))
                .Should().ThrowAsync<DatabaseWrongNameException>();
        }

        [Test]
        public async Task GetAsync_WhenDatabaseNotExists_ThrowDatabaseNotFoundException()
        {
            SetDbExists(false);

            await databaseManager.Invoking(x => x.GetAsync(DatabaseName))
                .Should().ThrowAsync<DatabaseNotFoundException>();
        }

        [Test]
        public async Task GetAsync_CallsDatabaseFactory()
        {
            SetDbExists(true);

            await databaseManager.GetAsync(DatabaseName);

            databaseFactoryMock.Verify(x => x.MakeDatabaseAsync(It.IsAny<string>(),
                It.IsAny<IFileIO>(), It.IsAny<IFileIO>()));
        }

        [TestCaseSource(nameof(wrongDbNames))]
        public async Task ExistsAsync_WhenDatabaseNameIsWrong_ThrowDatabaseWrongNameException(string dbName)
        {
            await databaseManager.Invoking(x => x.ExistsAsync(dbName))
                .Should().ThrowAsync<DatabaseWrongNameException>();
        }

        [Test]
        public async Task ExistsAsync_CallsFileManagerExistsMethod()
        {
            await databaseManager.ExistsAsync(DatabaseName);

            fileManagerMock.Verify(x => x.Exists(It.IsAny<string>()));
        }

        [Test]
        public async Task GetDbNamesAsync_ReturnsWordFileNamesWithoutExtension()
        {
            var fileFormat = DatabaseConstants.DatabaseWordFileExtension;
            var expected = new[] { "test", "tes_t2", "correct" };
            var fileNames = new[] {
                $"{expected[0]}{fileFormat}", "trash.word", "trash.char.ws",
                "trash.ws", $"{expected[1]}{fileFormat}", $"{expected[2]}{fileFormat}"
            };
            fileManagerMock.Setup(x => x.GetDirectoryFiles(It.IsAny<string>())).Returns(fileNames);

            var result = await databaseManager.GetDbNamesAsync();

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
