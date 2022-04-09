using System.Xml.Linq;
using WordSearch.Logic.Exceptions.Database;
using WordSearch.Logic.Interfaces;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.Tests
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
                databaseConfig);
        }

        [TestCase("a name")]
        [TestCase("name+name")]
        [TestCase("Some/Name")]
        [TestCase("Some.Name")]
        [TestCase("")]
        public async Task CreateAsync_WhenDatabaseNameIsWrong_ThrowDatabaseWrongNameException(string dbName)
        {
            SetDbExists(false);

            await databaseManager.Invoking(x => x.CreateAsync(dbName, Chars))
                .Should().ThrowAsync<DatabaseWrongNameException>();
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

        [Test]
        public async Task CreateAsync_InitCharsFile()
        {
            SetDbExists(false);
            MockFileIO(WordsFilePath);
            var (_, _, charsFileWriterMock) = MockFileIO(CharsFilePath);

            await databaseManager.CreateAsync(DatabaseName, Chars);

            charsFileWriterMock.Setup(x => x.WriteAsync(It.IsAny<string>()));
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

        [Test]
        public async Task ExistsAsync_CallsFileManagerExists()
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

            result.Should().BeEquivalentTo(expected);
        }

        private void SetDbExists(bool exists)
        {
            fileManagerMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(exists);
        }

        private (Mock<IFileIO>, Mock<IFileReader>, Mock<IFileWriter>) MockFileIO(
            string setupFactoryForValue)
        {
            var result = GlobalHelpers.MockFileIO();

            var (fileIOMock, _, _) = result;
            fileIOFactoryMock.Setup(x => x.MakeFileIO(setupFactoryForValue)).Returns(fileIOMock.Object);

            return result;
        }
    }
}
